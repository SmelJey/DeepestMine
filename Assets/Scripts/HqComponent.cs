using System;
using UnityEngine;

public class HqComponent : MonoBehaviour, ISelectable {
    public string Name => gameObject.name;

    public string GetInfo() { return String.Empty; }

    public void OnRightClick() { }
}
