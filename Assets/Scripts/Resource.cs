using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resources", menuName = "LevelGeneration/Resource", order = 0)]
public class Resource : ScriptableObject {
    [Serializable]
    private class SpawnChance {
        public int MinY;
        public float Chance;
    }
    
    [SerializeField] private GameObject orePrefab;
    [SerializeField] private List<SpawnChance> spawnChances;
    [SerializeField] private int minVein = 1;
    [SerializeField] private int maxVein = 5;
    
    [SerializeField] private int resourcePerHp = 2;

    public GameObject InstantiateOre(Vector3 position, Transform host) {
        var obj = Instantiate(orePrefab, position, Quaternion.identity, host);
        var hp = obj.GetComponent<HpComponent>();
        hp.OnHit += (hpComponent, args) => {
            if (args.Attacker.CompareTag("Player")) {
                SessionManager.Instance.AddResource(new List<ResourceEntry> {new ResourceEntry(this, args.Damage * resourcePerHp)});
            }
        };
        hp.OnDeath += (component, args) => SessionManager.Instance.StartCoroutine(LevelGenerator.UpdateGraph(1));;

        return obj;
    }

    public int MinYSpawn => spawnChances.Count > 0 ? spawnChances[0].MinY : 1000;

    public float GetSpawnChance(int y) {
        float chance = 0;
        foreach (var entry in spawnChances) {
            if (entry.MinY > y) {
                break;
            }

            chance = entry.Chance;
        }

        return chance;
    }
    
    public int MinVein => minVein;
    public int MaxVein => maxVein;
}
