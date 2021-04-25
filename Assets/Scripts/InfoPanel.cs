using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    [SerializeField] private Text title;
    [SerializeField] private Text description;

    public void ShowSelectedObjectsInfo(List<ISelectable> selectedObject) {
        if (selectedObject.Count == 0) {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        title.text = selectedObject.First().Name;

        var infoBuilder = new StringBuilder();
        foreach (var selected in selectedObject) {
            infoBuilder.Append(selected.GetInfo() + "\n");
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
