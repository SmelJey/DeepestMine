using UnityEngine;
using UnityEngine.UI;

public class BuildingBtn : MonoBehaviour {
    [SerializeField] private Text title;
    [SerializeField] private Text description;

    private Button myButton;

    private void Awake() {
        myButton = GetComponent<Button>();
    }

    public Button.ButtonClickedEvent OnClick {
        get => myButton.onClick;
        set => myButton.onClick = value;
    }
    
    public string Title {
        get => title.text;
        set => title.text = value;
    }

    public string Description {
        get => description.text;
        set => description.text = value;
    }
}
