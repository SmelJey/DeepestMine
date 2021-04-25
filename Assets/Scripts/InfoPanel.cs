using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private RectTransform actionListPanel;
    [SerializeField] private Button buttonPrefab;

    private void Awake() {
        gameObject.SetActive(false);
    }

    public void ShowSelectedObjectsInfo(List<ISelectable> selectedObject) {
        if (selectedObject.Count == 0) {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        title.text = selectedObject.First().Name;

        foreach (Transform child in actionListPanel.transform) {
            Destroy(child.gameObject);
        }

        var infoBuilder = new StringBuilder();
        var btnTransform = buttonPrefab.GetComponent<RectTransform>();
        float curY = -btnTransform.rect.height / 2;
        
        foreach (var selected in selectedObject) {
            infoBuilder.Append(selected.GetInfo() + "\n");

            var actions = selected.GetActionList();
            foreach (var action in actions) {
                var btn = Instantiate(buttonPrefab, actionListPanel);
                var curRect = btn.GetComponent<RectTransform>();
                curRect.anchoredPosition = new Vector2(0, curY);
                curY += btnTransform.rect.height;

                btn.GetComponentInChildren<Text>().text = action.Description;
                btn.onClick.AddListener(action.Callback);
            }
        }
 
        description.text = infoBuilder.ToString();
    }

    // public string Title {
    //     get => title.text;
    //     set => title.text = value;
    // }
    //
    // public string Description {
    //     get => description.text;
    //     set => description.text = value;
    // }
}
