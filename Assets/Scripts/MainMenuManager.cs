using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private Text lastScoreLabel;

    public void StartBtn() {
        GameManager.Instance.StartGame();
    }

    private void Start() {
        if (GameManager.Instance.LastScore != 0) {
            lastScoreLabel.text = $"Last Score: {GameManager.Instance.LastScore}";
        } else {
            lastScoreLabel.text = "";
        }
    }
}
