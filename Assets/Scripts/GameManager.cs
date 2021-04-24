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
        // var curScene = SceneManager.GetActiveScene();
        // var loading = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        // loading.allowSceneActivation = false;
        // loading.completed += operation => {
        //     var unloading = SceneManager.UnloadSceneAsync(curScene);
        //     operation.allowSceneActivation = true;
        // };
    }

    public void ToMenu() {
        SceneManager.LoadScene("MainMenu");
        // var curScene = SceneManager.GetActiveScene();
        // var loading = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        // loading.allowSceneActivation = false;
        // loading.completed += operation => {
        //     var unloading = SceneManager.UnloadSceneAsync(curScene);
        //     operation.allowSceneActivation = true;
        // };
    }
}