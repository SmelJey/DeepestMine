using UnityEngine;

[CreateAssetMenu(fileName = "LevelGeneratorAsset", menuName = "LevelGeneration/LevelGeneratorPreset", order = 1)]
public class LevelGeneratorPreset : ScriptableObject {
    [SerializeField] private float initialWallChance = 0.4f;
    [SerializeField] private int myWallCntToWall = 4;
    [SerializeField] private int myFloorCntToFloor = 4;
    [SerializeField] private int myAutomatonRounds = 5;
    [SerializeField] private GameObject myHqPrefab;
    [SerializeField] private GameObject myDwarfPrefab;
    [SerializeField] private GameObject[] myWallPrefab;
    [SerializeField] private GameObject[] myOrePrefab;

    public float InitialWallChance => initialWallChance;
    public int WallCntToWall => myWallCntToWall;
    public int FloorCntToFloor => myFloorCntToFloor;
    public int AutomatonRounds => myAutomatonRounds;

    
    public GameObject HqPrefab => myHqPrefab;
    public GameObject DwarfPrefab => myDwarfPrefab;
    public GameObject[] WallPrefab => myWallPrefab;
    public GameObject[] OrePrefab => myOrePrefab;
}
