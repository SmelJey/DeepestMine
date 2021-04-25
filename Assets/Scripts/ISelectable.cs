using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionInfo {
    public ActionInfo(string desc, UnityAction cb) {
        Description = desc;
        Callback = cb;
    }

    public string Description { get; }
    
    public UnityAction Callback { get; }
}

public interface ISelectable {
    string Name { get; }
    
    string GetInfo();
    
    void OnRightClick(Vector2 mousePosition);

    List<ActionInfo> GetActionList();

    bool IsUpdated();
}
