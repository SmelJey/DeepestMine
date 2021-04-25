using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceEntry {
    public ResourceEntry() { }
    
    public ResourceEntry(Resource res, int amount) {
        resource = res;
        cost = amount;
    }
    
    [SerializeField] private Resource resource;
    [SerializeField] private int cost;

    public Resource ResourceType => resource;

    public int Cost => cost;
}

[CreateAssetMenu(fileName = "Building", menuName = "LevelGeneration/Building", order = 0)]
public class Building : ScriptableObject {
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<ResourceEntry> costs;

    public List<ResourceEntry> Costs => costs;
    public GameObject Prefab => prefab;
}
