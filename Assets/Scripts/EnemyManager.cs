
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
    [SerializeField] private int hordeDifficultyCoef = 90;
    [SerializeField] private List<EnemyInfo> enemyTypes;
    private Random myRandom;
    
    private void Start() {
        StartCoroutine(DifficultyLoop(.5f));
        myRandom = new Random((uint) DateTimeOffset.Now.ToUnixTimeSeconds());
    }

    IEnumerator DifficultyLoop(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            
            Spawn();
            myDifficulty++;
        }
    }

    public void Spawn() {
        foreach (var enemy in enemyTypes) {
            if (myRandom.NextFloat() < enemy.SpawningChance) {
                int x = myRandom.NextInt(1, LevelGenerator.MapWidth - 1) - LevelGenerator.MapWidth / 2;
                int y = myRandom.NextInt((int) (LevelGenerator.MapChunkLength * 0.75f), LevelGenerator.MapChunkLength);

                Instantiate(enemy.EnemyPrefab, new Vector3(x, -y, 0), Quaternion.identity, transform);
            }
        }
    }
}
