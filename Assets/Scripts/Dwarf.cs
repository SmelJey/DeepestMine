﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class Dwarf : MonoBehaviour, ISelectable {
    [SerializeField] private int movementSpeed = 7;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private SpriteRenderer weaponRenderer;

    [SerializeField] private List<DwarfToolCost> upgrades;

    [Serializable]
    private class DwarfToolCost {
        public DwarfTool Tool;
        public ResourceEntry Cost;
    }

    private AttackComponent myAttackComponent;
    private Path myPath;
    private GameObject myTarget;
    private int myCurrentWaypoint = 0;
    private float nextWaypointDistance = 0.25f;
    private Seeker mySeeker;
    private bool isUpdated = true;
    private Rigidbody2D myRigidbody2D;
    
    private void Awake() {
        mySeeker = GetComponent<Seeker>();
        myAttackComponent = GetComponent<AttackComponent>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        
        weaponRenderer.sprite = myAttackComponent.Weapon.ToolSprite;
    }

    public string Name => gameObject.name;

    public string GetInfo() {
        isUpdated = false;
        return $"Tool: {myAttackComponent.Weapon.name}";
    }

    public void OnRightClick(Vector2 mousePosition) {
        var hit = Physics2D.Raycast((Vector3) mousePosition + Vector3.back , Vector3.forward, 2, layerMask);
        if (hit.collider == null) {
            myTarget = null;
        } else {
            myTarget = hit.collider.gameObject;
        }

        mySeeker.CancelCurrentPathRequest();
        mySeeker.StartPath(transform.position, mousePosition, path => {
            if (path.error) {
                Debug.Log(path.errorLog, this);
                return;
            }

            myPath = path;
            myCurrentWaypoint = 0;
        });
    }

    private void Update() {
        if (myTarget != null) {
            myAttackComponent.TryAttackTarget(myTarget);
        }
        
        if (myPath == null || myAttackComponent.IsAttacking) {
            myRigidbody2D.velocity = Vector2.zero;
            return;
        }
        
        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector3.Distance(transform.position, myPath.vectorPath[myCurrentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (myCurrentWaypoint + 1 < myPath.vectorPath.Count) {
                    myCurrentWaypoint++;
                } else {
                    myPath = null;
                    break;
                }
            } else {
                break;
            }
        }

        if (myPath != null) {
            Vector2 moveDirection = myPath.vectorPath[myCurrentWaypoint] - transform.position; 
            Move(moveDirection.normalized);
        }
    }
    
    private void Move(Vector2 direction) {
        if (direction != Vector2.zero) 
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        myRigidbody2D.velocity = direction * movementSpeed;
    }

    public List<ActionInfo> GetActionList() {
        var actions = new List<ActionInfo>();
        foreach (var upgrade in upgrades) {
            actions.Add(new ActionInfo($"Cost: {upgrade.Cost.ResourceType} :: {upgrade.Cost.Cost}", () => {
                if (myAttackComponent.Weapon.name != upgrade.Tool.name && SessionManager.Instance.Buy(new List<ResourceEntry> {upgrade.Cost})) {
                    myAttackComponent.Weapon = upgrade.Tool;
                    weaponRenderer.sprite = myAttackComponent.Weapon.ToolSprite;
                    isUpdated = true;
                }
            }));
        }

        return actions;
    }

    public bool IsUpdated() => isUpdated;
}
