
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EnemyManager : MonoBehaviour {
    [Serializable]
    private class EnemyInfo {
        public Enemy EnemyPrefab;
        public float SpawningChance = 0.5f;
        public int minimalDifficulty = 0;
    }

    private int myDifficulty = 0;
    [SerializeField] private int hordeDifficultyCoef = 240;
    [SerializeField] private List<EnemyInfo> enemyTypes;
    private Random myRandom;

    [SerializeField] private int myHordeCnt = 5;
    private float myDifficultyMultiplier = 1;
    
    private void Start() {
        StartCoroutine(DifficultyLoop(.5f));
        myRandom = new Random((uint) DateTimeOffset.Now.ToUnixTimeSeconds());
    }

    IEnumerator DifficultyLoop(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);

            int rem = myDifficulty % hordeDifficultyCoef;
            if (rem < hordeDifficultyCoef * 5 / 6) {
                Spawn();
            }
            
            myDifficulty++;
            SessionManager.Instance.Score = myDifficulty;
            if (myDifficulty % hordeDifficultyCoef == 0) {
                myDifficultyMultiplier *= 1f + myDifficulty * 1f / 3000;
                SpawnHorde(myHordeCnt);
                Debug.Log($"Horde: {myHordeCnt}");
                myHordeCnt++;
            }
        }
    }

    public void SpawnHorde(int cnt) {
        int x = myRandom.NextInt(10, LevelGenerator.MapWidth - 10) - LevelGenerator.MapWidth / 2;
        int y = myRandom.NextInt((int) (LevelGenerator.MapChunkLength * 0.75f), LevelGenerator.MapChunkLength - 5);
        
        foreach (var enemy in enemyTypes) {
            for (int i = 0; i < cnt; i++) {
                if (enemy.minimalDifficulty <= myDifficulty && myRandom.NextFloat() < enemy.SpawningChance * myDifficultyMultiplier) {
                    int dx = myRandom.NextInt(-5, 6);
                    int dy = myRandom.NextInt(-2, 3);

                    var enemyObj = Instantiate(enemy.EnemyPrefab, new Vector3(x + dx, -y + dy, 0), Quaternion.identity, transform);
                    enemyObj.name = enemy.EnemyPrefab.name;
                }
            }
            
        }
    }
    
    public void Spawn() {
        foreach (var enemy in enemyTypes) {
            if (enemy.minimalDifficulty <= myDifficulty && myRandom.NextFloat() < enemy.SpawningChance * myDifficultyMultiplier) {
                int x = myRandom.NextInt(1, LevelGenerator.MapWidth - 1) - LevelGenerator.MapWidth / 2;
                int y = myRandom.NextInt((int) (LevelGenerator.MapChunkLength * 0.75f), LevelGenerator.MapChunkLength);

                var enemyObj = Instantiate(enemy.EnemyPrefab, new Vector3(x, -y, 0), Quaternion.identity, transform);
                enemyObj.name = enemy.EnemyPrefab.name;
            }
        }
    }
}
