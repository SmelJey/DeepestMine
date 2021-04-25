﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class HpComponent : MonoBehaviour, ISelectable {
    [SerializeField] private int myMaxHp;
    [SerializeField] private int myHp;

    private bool isUpdated = true;
    private bool myIsUpdated;

    public int MaxHp => myMaxHp;
    public int Hp => myHp;
    public bool DestroyOnDeath { get; set; } = true;

    public void GetHit(int dmg) {
        var args = new HitArgs(Math.Min(dmg, myHp));
        myHp -= dmg;
        OnHit?.Invoke(this, args);
        isUpdated = true;
        
        if (myHp <= 0) {
            OnDeath?.Invoke(this, args);

            if (DestroyOnDeath) {
                Destroy(gameObject);
            }
        }
    }

    public delegate void HitHandler(HpComponent component, HitArgs hitArgs);

    public class HitArgs {
        public HitArgs(int dmg) {
            Damage = dmg;
        }

        public int Damage {
            get;
            private set;
        }
    }
    
    public event HitHandler OnDeath;
    public event HitHandler OnHit;

    public string Name => gameObject.name;

    public string GetInfo() {
        isUpdated = false;
        return $"HP: {myHp} / {myMaxHp}";
    }

    public void OnRightClick(Vector2 mousePosition) { }
    
    public List<ActionInfo> GetActionList() {
        return new List<ActionInfo>();
    }

    public bool IsUpdated() => isUpdated;
}
