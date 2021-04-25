using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resources", menuName = "LevelGeneration/Resource", order = 0)]
public class Resource : ScriptableObject {
    [SerializeField] private GameObject orePrefab;
    [SerializeField] private int minYSpawn;
    [SerializeField] private float spawnChance = 0.05f;
    [SerializeField] private int minVein = 1;
    [SerializeField] private int maxVein = 5;
    
    [SerializeField] private int resourcePerHp = 2;

    public GameObject InstantiateOre(Vector3 position, Transform host) {
        var obj = Instantiate(orePrefab, position, Quaternion.identity, host);
        var hp = obj.GetComponent<HpComponent>();
        hp.OnHit += (hpComponent, args) => {
            SessionManager.Instance.AddResource(new List<ResourceEntry> {new ResourceEntry(this, args.Damage * resourcePerHp)});
        };

        return obj;
    }

    public int MinYSpawn => minYSpawn;
    public float SpawnChance => spawnChance;
    public int MinVein => minVein;
    public int MaxVein => maxVein;
}
