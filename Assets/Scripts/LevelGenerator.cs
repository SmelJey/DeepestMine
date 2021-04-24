using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class LevelGenerator {
    public LevelGenerator(LevelGeneratorPreset preset) {
        myPreset = preset;
    }

    private LevelGeneratorPreset myPreset;
    private Random myRandom;

    public enum TileType {
        Wall,
        Floor
    }

    private const int StartRadius = 3;
    
    private const int MapWidth = 100;
    private const int MapChunkLength = 100;
    private const int ChunkPreGeneration = 3;

    private const int MaxExitPerChunk = 4;
    private const int MaxExitWidth = 5;

    private const int BorderSize = 4;
    
    private List<TileType[]> levelMap;

    private int myLastChunk = 0;

    public void InitLevel(GameObject levelHost) {
        myRandom = new Random(2);
        levelMap = new List<TileType[]>();
        GenerateLevel(levelHost, ChunkPreGeneration);
    }

    public void GenerateLevel(GameObject levelHost, int chunkCount) {
        for (int i = -BorderSize; i < 0; i++) {
            for (int j = -BorderSize; j < MapWidth + BorderSize; j++) {
                Object.Instantiate(myPreset.WallPrefab[0], IdxToWorldPos(i, j), Quaternion.identity, levelHost.transform);
            }
        }
        
        for (int i = 0; i < chunkCount; i++) {
            GenerateChunk(levelHost);
        }
    }

    private void GenerateChunk(GameObject levelHost) {
        int curY = myLastChunk * MapChunkLength;
        int endY = (myLastChunk + 1) * MapChunkLength;
        List<TileType[]> bufferMap = new List<TileType[]>();

        int exitCnt = myRandom.NextInt(1, MaxExitPerChunk + 1);
        int[] exitsX = new int[exitCnt];
        int[] exitsWidth = new int[exitCnt];
        for (int i = 0; i < exitCnt; i++) {
            exitsX[i] = myRandom.NextInt(0, MapWidth);
            exitsWidth[i] = myRandom.NextInt(1, MaxExitWidth + 1);
        }

        for (int i = curY; i < endY; i++) {
            bufferMap.Add(new TileType[MapWidth]);
            levelMap.Add(new TileType[MapWidth]);
            for (int j = 0; j < MapWidth; j++) {
                levelMap[i][j] = (myRandom.NextFloat() > myPreset.InitialWallChance ? TileType.Floor : TileType.Wall);
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
            for (int j = 0 - BorderSize; j < MapWidth + BorderSize; j++) {
                if (j < 0 || j >= MapWidth || levelMap[i][j] == TileType.Wall) {
                    Object.Instantiate(myPreset.WallPrefab[0], IdxToWorldPos(i, j), Quaternion.identity, levelHost.transform);
                }
            }
        }
        
        levelMap.RemoveAt(levelMap.Count - 1);
        myLastChunk++;
    }

    private Vector3 IdxToWorldPos(int i, int j) {
        return new Vector3(j - MapWidth / 2, 0 - i, 0);
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