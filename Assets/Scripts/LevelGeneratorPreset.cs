using UnityEngine;

[CreateAssetMenu(fileName = "LevelGeneratorAsset", menuName = "LevelGeneration/LevelGeneratorPreset", order = 1)]
public class LevelGeneratorPreset : ScriptableObject {
    [SerializeField] private float initialWallChance = 0.4f;
    [SerializeField] private int wallCntToWall = 4;
    [SerializeField] private int floorCntToFloor = 4;
    [SerializeField] private int automatonRounds = 5;
    
    [SerializeField] private HqComponent hqPrefab;
    [SerializeField] private GameObject dwarfPrefab;
    [SerializeField] private GameObject[] wallPrefab;
    [SerializeField] private GameObject[] orePrefab;
    
    [SerializeField] private int maxExitPerChunk = 4;
    [SerializeField] private int maxExitWidth = 5;
    [SerializeField] private int borderSize = 4;

    public float InitialWallChance => initialWallChance;
    public int WallCntToWall => wallCntToWall;
    public int FloorCntToFloor => floorCntToFloor;
    public int AutomatonRounds => automatonRounds;

    
    public HqComponent HqPrefab => hqPrefab;
    public GameObject DwarfPrefab => dwarfPrefab;
    public GameObject[] WallPrefab => wallPrefab;
    public GameObject[] OrePrefab => orePrefab;

    public int MaxExitPerChunk => maxExitPerChunk;
    public int MaxExitWidth=> maxExitWidth;
    public int BorderSize => borderSize;
}
