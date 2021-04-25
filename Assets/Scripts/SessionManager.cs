using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour {
    public static SessionManager Instance {
        get;
        private set;
    }
    
    [SerializeField] private LevelGeneratorPreset levelGeneratorPreset;
    [SerializeField] private Building[] buildings;
    [SerializeField] private RectTransform buildPanel;
    [SerializeField] private RectTransform upperPanel;
    
    [SerializeField] private BuildingBtn buildingBtnPrefab;
    [SerializeField] private RectTransform resourceLabelPrefab;
    [SerializeField] private PlayerControl playerControl;

    private Dictionary<Resource, int> myResourceCounts;
    private Dictionary<Resource, Text> myResourceLabels;
    private LevelGenerator myLevelGenerator;

    public Dictionary<Resource, int> ResourceCount => myResourceCounts;

    public bool CheckCost(List<ResourceEntry> cost) {
        foreach (var entry in cost) {
            if (entry.Cost > myResourceCounts[entry.ResourceType]) {
                return false;
            }
        }

        return true;
    }

    public bool Buy(List<ResourceEntry> cost) {
        if (!CheckCost(cost)) {
            return false;
        }
        
        foreach (var entry in cost) {
            myResourceCounts[entry.ResourceType] -= entry.Cost;
        }
        
        OnResourceChange();
        return true;
    }

    public void AddResource(List<ResourceEntry> res) {
        foreach (var entry in res) {
            myResourceCounts[entry.ResourceType] += entry.Cost;
        }
    }
    
    private void Awake() {
        if (Instance != null) {
            Destroy(Instance);
        }
        Instance = this;
        
        var levelHost = new GameObject("Level");

        myLevelGenerator = new LevelGenerator(levelGeneratorPreset);

        myResourceCounts = new Dictionary<Resource, int>();
        myResourceLabels = new Dictionary<Resource, Text>();
        
        float curX = 20 + resourceLabelPrefab.rect.width / 2;
        foreach (var res in levelGeneratorPreset.Resources) {
            myResourceCounts.Add(res, 100);
            var resourceLabel = Instantiate(resourceLabelPrefab, upperPanel);
            myResourceLabels.Add(res, resourceLabel.GetComponent<Text>());
            resourceLabel.anchoredPosition = new Vector2(curX, 0);
            curX += resourceLabelPrefab.rect.width;
        }
        
        OnResourceChange();

        var hq = myLevelGenerator.InitLevel(levelHost);
        hq.GetComponent<HpComponent>().OnDeath += (hpComponent, args) => {
            Lose();
        };

        var btnRect = buildingBtnPrefab.GetComponent<RectTransform>();
        curX = btnRect.rect.width / 2;
        
        foreach (var building in buildings) {
            var btn = Instantiate(buildingBtnPrefab, buildPanel);
            var curRect = btn.GetComponent<RectTransform>();
            curRect.anchoredPosition = new Vector2(curX, 0);
            curX += btnRect.rect.width;

            btn.Title = building.name;
            btn.Description = "";
            foreach (var res in building.Costs) {
                btn.Description += $"{res.ResourceType.name} : {res.Cost} \n";
            }

            btn.OnClick.AddListener(() => {
                playerControl.SelectBuilding(building);
            });
        }
        
        AstarPath.active.Scan();
    }


    private void OnResourceChange() {
        foreach (var entry in myResourceLabels) {
            entry.Value.text = $"{entry.Key.name} : {myResourceCounts[entry.Key]}";
        }
    }

    public void Lose() {
        GameManager.Instance.ToMenu();
    }
}