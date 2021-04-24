using UnityEngine;

public class SessionManager : MonoBehaviour {
    [SerializeField] private LevelGeneratorPreset levelGeneratorPreset;
    
    private LevelGenerator myLevelGenerator;

    private void Awake() {
        var levelHost = new GameObject("Level");

        myLevelGenerator = new LevelGenerator(levelGeneratorPreset);
        myLevelGenerator.InitLevel(levelHost);
    }
}