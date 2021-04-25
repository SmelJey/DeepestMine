using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HqComponent : MonoBehaviour, ISelectable {
    public string Name => gameObject.name;

    public string GetInfo() { return String.Empty; }

    public void OnRightClick() { }

    [SerializeField] private List<ResourceEntry> cost;
    [SerializeField] private float increasingCoef;
    [SerializeField] private GameObject dwarfPrefab;
    
    public List<ActionInfo> GetActionList() {
        var costString = new StringBuilder();
        costString.Append("Recruit dwarf:\n");
        foreach (var entry in cost) {
            costString.Append($"{entry.ResourceType.name} : {entry.Cost}\n");
        }
        
        
        return new List<ActionInfo> { new ActionInfo(costString.ToString(), () => {
            var entryList = cost.ToList();
            
            if (!SessionManager.Instance.Buy(entryList)) {
                return;
            }

            cost.Clear();
            cost = entryList.Select(it => new ResourceEntry(it.ResourceType, (int) (it.Cost * increasingCoef)))
                            .ToList();

            Instantiate(dwarfPrefab, transform.position + new Vector3(0, -2, 0), Quaternion.identity);
        })};
    }
}
