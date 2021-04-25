using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class Dwarf : MonoBehaviour, ISelectable {
    [SerializeField] private int movementSpeed = 5;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private DwarfTool currentTool;

    [SerializeField] private List<DwarfToolCost> upgrades;

    [Serializable]
    private class DwarfToolCost {
        public DwarfTool Tool;
        public ResourceEntry Cost;
    }
    
    private Path myPath;
    private GameObject myTarget;
    private int myCurrentWaypoint = 0;
    private float nextWaypointDistance = 0.25f;
    private Seeker mySeeker;
    private bool isAttacking = false;
    private bool isUpdated = true;
    
    private void Awake() {
        mySeeker = GetComponent<Seeker>();
    }

    public string Name => gameObject.name;

    public string GetInfo() {
        isUpdated = false;
        return $"Tool: {currentTool.name}";
    }

    public void OnRightClick(Vector2 mousePosition) {
        var hit = Physics2D.Raycast((Vector3) mousePosition + Vector3.back , Vector3.forward, 2, layerMask);
        if (hit.collider == null) {
            myTarget = null;
        } else {
            myTarget = hit.collider.gameObject;
        }

        mySeeker.StartPath(transform.position, mousePosition, path => {
            if (!path.error) {
                Debug.Log(path.errorLog, this);
            }

            myPath = path;
            myCurrentWaypoint = 0;
        });
    }

    private void TryAttackTarget() {
        Debug.DrawRay(transform.position, (myTarget.transform.position - transform.position).normalized, Color.red);
        var hit = Physics2D.Raycast(transform.position,
                                    (myTarget.transform.position - transform.position).normalized, currentTool.AttackRange, layerMask);

        if (hit.collider != null && hit.collider.gameObject == myTarget) {
            var obj = hit.collider.gameObject;
            var hitComp = obj.GetComponent<HpComponent>();
            if (hitComp != null) {
                if (obj.CompareTag("Wall") && !currentTool.CanAttackRocks) {
                    return;
                }

                if (Vector3.Distance(transform.position, obj.transform.position) < currentTool.AttackRange) {
                    StartCoroutine(Attack(hitComp));
                }
            }
        }
    }
    
    private void Update() {
        if (myTarget != null) {
            TryAttackTarget();
        }
        
        if (myPath == null || isAttacking) {
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
            transform.Translate((myPath.vectorPath[myCurrentWaypoint] - transform.position).normalized * movementSpeed
                                * Time.deltaTime * (Vector2.Distance(transform.position, myPath.vectorPath.Last()) < 0.5 ? 0.25f : 1));
        }

    }

    IEnumerator Attack(HpComponent component) {
        if (isAttacking) {
            yield break;
        }

        isAttacking = true;
        component.GetHit(currentTool.AttackDamage);

        yield return new WaitForSeconds(currentTool.AttackCooldown);
        isAttacking = false;
    }

    public List<ActionInfo> GetActionList() {
        return new List<ActionInfo>();
    }

    public bool IsUpdated() => isUpdated;
}
