using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    private Text lastScoreLabel;
    
    private void Awake() {
        lastScoreLabel = GetComponent<Text>();
    }

    private void Start() {
        if (GameManager.Instance.LastScore != 0) {
            lastScoreLabel.text = $"Last Score: {GameManager.Instance.LastScore}";
        } else {
            lastScoreLabel.text = "";
        }
    }
}
