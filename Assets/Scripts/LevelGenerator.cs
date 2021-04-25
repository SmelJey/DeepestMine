using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = Unity.Mathematics.Random;

public class LevelGenerator {
    public LevelGenerator(LevelGeneratorPreset preset) {
        myPreset = preset;
        Array.Sort(preset.Resources, (left, right) => (int)((left.GetSpawnChance(left.MinYSpawn) * (-left.MinYSpawn) - right.GetSpawnChance(right.MinYSpawn) * (-right.MinYSpawn)) * 1000));
        Debug.Log("test");
    }

    private LevelGeneratorPreset myPreset;
    private Random myMapRandom;
    private Random myOreRandom;

    public enum TileType {
        Wall,
        Floor
    }

    private const int StartRadius = 3;
    
    public const int MapWidth = 100;
    public const int MapChunkLength = 100;
    private const int ChunkPreGeneration = 1;

    private List<TileType[]> levelMap;

    private int myLastChunk = 0;

    private void InstantiateIndestructibleWall(Vector3 position, Transform host) {
        var obj = Object.Instantiate(myPreset.WallPrefab[0], position, Quaternion.identity, host);
        Object.Destroy(obj.GetComponent<HpComponent>());
    }
    
    public HqComponent InitLevel(GameObject levelHost) {
        myMapRandom = new Random((uint)DateTimeOffset.Now.ToUnixTimeMilliseconds());
        myOreRandom = new Random((uint)DateTimeOffset.Now.ToUnixTimeMilliseconds());
        
        levelMap = new List<TileType[]>();
        for (int i = -myPreset.BorderSize; i < 0; i++) {
            for (int j = -myPreset.BorderSize; j < MapWidth + myPreset.BorderSize; j++) {
                InstantiateIndestructibleWall(IdxToWorldPos(i, j), levelHost.transform);
            }
        }

        GenerateLevel(levelHost, ChunkPreGeneration);
        
        for (int i = myLastChunk * MapChunkLength; i < myLastChunk * MapChunkLength + myPreset.BorderSize; i++) {
            for (int j = -myPreset.BorderSize; j < MapWidth + myPreset.BorderSize; j++) {
                InstantiateIndestructibleWall(IdxToWorldPos(i, j), levelHost.transform);
            }
        }

        return Object.Instantiate(myPreset.HqPrefab, IdxToWorldPos(StartRadius, MapWidth / 2), Quaternion.identity, levelHost.transform);
    }

    public void GenerateLevel(GameObject levelHost, int chunkCount) {
        for (int i = 0; i < chunkCount; i++) {
            GenerateChunk(levelHost);
        }
    }

    private void GenerateChunk(GameObject levelHost) {
        int curY = myLastChunk * MapChunkLength;
        int endY = (myLastChunk + 1) * MapChunkLength;
        List<TileType[]> bufferMap = new List<TileType[]>();

        int exitCnt = myMapRandom.NextInt(1, myPreset.MaxExitPerChunk + 1);
        int[] exitsX = new int[exitCnt];
        int[] exitsWidth = new int[exitCnt];
        for (int i = 0; i < exitCnt; i++) {
            exitsX[i] = myMapRandom.NextInt(0, MapWidth);
            exitsWidth[i] = myMapRandom.NextInt(1, myPreset.MaxExitWidth + 1);
        }

        for (int i = curY; i < endY; i++) {
            bufferMap.Add(new TileType[MapWidth]);
            levelMap.Add(new TileType[MapWidth]);
            for (int j = 0; j < MapWidth; j++) {
                levelMap[i][j] = (myMapRandom.NextFloat() > myPreset.InitialWallChance ? TileType.Floor : TileType.Wall);
            }
        }

        levelMap.Add(new TileType[MapWidth]);
        for (int j = 0; j < MapWidth; j++) {
            levelMap.Last()[j] = TileType.Wall;
            for (int exit = 0; exit < exitCnt; exit++) {
                if (j >= exitsX[exit] && j <= exitsX[exit] + exitsWidth[exit]) {
                    levelMap.Last()[j] = TileType.Floor;
                    break;
                }
            }    
        }

        for (int round = 0; round < myPreset.AutomatonRounds; round++) {
            for (int i = curY; i < endY; i++) {
                for (int j = 0; j < MapWidth; j++) {
                    if (i <= StartRadius * 2 && j >= MapWidth / 2 - StartRadius && j <= MapWidth / 2 + StartRadius) {
                        bufferMap[i - curY][j] = TileType.Floor;
                        continue;
                    }
                    
                    int wallCnt = GetNeighboursWallCount(j, i);
                    if (levelMap[i][j] == TileType.Floor) {
                        bufferMap[i - curY][j] = (wallCnt >= myPreset.WallCntToWall ? TileType.Wall : TileType.Floor);
                    } else {
                        bufferMap[i - curY][j] = (8 - wallCnt >= myPreset.FloorCntToFloor ? TileType.Floor : TileType.Wall);
                    }
                }
            }

            for (int i = 0; i < bufferMap.Count; i++) {
                for (int j = 0; j < MapWidth; j++) {
                    levelMap[i + curY][j] = bufferMap[i][j];
                }
            }
        }

        for (int i = curY; i < endY; i++) {
            for (int j = - myPreset.BorderSize; j < MapWidth + myPreset.BorderSize; j++) {
                if (j < 0 || j >= MapWidth) {
                    InstantiateIndestructibleWall(IdxToWorldPos(i, j), levelHost.transform);
                    continue;
                }

                if (levelMap[i][j] == TileType.Wall) {
                    bool isWall = true;
                    foreach (var res in myPreset.Resources) {
                        if (res.MinYSpawn <= i && myOreRandom.NextFloat() < res.GetSpawnChance(i)) {
                            res.InstantiateOre(IdxToWorldPos(i, j), levelHost.transform);
                            isWall = false;
                            break;
                        }
                    }

                    if (isWall) {
                        var obj = Object.Instantiate(myPreset.WallPrefab[0], IdxToWorldPos(i, j), Quaternion.identity, levelHost.transform);
                        obj.GetComponent<HpComponent>().OnDeath += (component, args) => {
                            SessionManager.Instance.StartCoroutine(UpdateGraph(1));
                        };
                    }
                }
            }
        }
        
        levelMap.RemoveAt(levelMap.Count - 1);
        myLastChunk++;
    }

    public static IEnumerator UpdateGraph(int delay) {
        yield return null;
        AstarPath.active.Scan();
    }

    private Vector3 IdxToWorldPos(int i, int j) {
        return new Vector3(j - MapWidth / 2, -i, 0);
    }
    
    private int GetNeighboursWallCount(int xj, int yi) {
        int cnt = 0;
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (i == 0 && j == 0) {
                    continue;
                }

                if (xj + j < 0 || xj + j >= MapWidth || yi + i < 0 || yi + i >= levelMap.Count) {
                    cnt++;
                    continue;
                }

                if (levelMap[yi + i][xj + j] == TileType.Wall) {
                    cnt++;
                }
            }
        }

        return cnt;
    }
}