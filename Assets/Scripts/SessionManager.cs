using UnityEngine;

public class SessionManager : MonoBehaviour {
    [SerializeField] private LevelGeneratorPreset levelGeneratorPreset;
    
    private LevelGenerator myLevelGenerator;

    private void Awake() {
        var levelHost = new GameObject("Level");

        myLevelGenerator = new LevelGenerator(levelGeneratorPreset);
        
        var hq = myLevelGenerator.InitLevel(levelHost);
        hq.GetComponent<HpComponent>().OnDeath += (hpComponent, args) => {
            Lose();
        };
    }

    public void Lose() {
        GameManager.Instance.ToMenu();
    }
}