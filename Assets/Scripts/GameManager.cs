using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void StartGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void ToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
    
    public int LastScore { get; set; }
}