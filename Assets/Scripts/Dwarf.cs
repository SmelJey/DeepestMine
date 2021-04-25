using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class Dwarf : MonoBehaviour, ISelectable {
    [SerializeField] private int movementSpeed = 7;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private SpriteRenderer weaponRenderer;
    [SerializeField] private LayerMask autoAttackMask;

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
    
    private HashSet<GameObject> myTargets;
    
    private void Awake() {
        mySeeker = GetComponent<Seeker>();
        myAttackComponent = GetComponent<AttackComponent>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        
        weaponRenderer.sprite = myAttackComponent.Weapon.ToolSprite;

        myTargets = new HashSet<GameObject>();
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
            float closestDist = 1000f;
            GameObject closestTarget = null;
            
            foreach (var enemy in myTargets.ToList()) {
                if (enemy == null) {
                    myTargets.Remove(enemy);
                    continue;
                }

                var dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < closestDist) {
                    closestDist = dist;
                    closestTarget = enemy;
                }
            }

            if (closestTarget != null) {
                var isMelee = myAttackComponent.Weapon.AttackRange < 2;
                var mask = isMelee ? myAttackComponent.CollisionMask : myAttackComponent.AttackMask;
                var hit = Physics2D.Raycast(transform.position, closestTarget.transform.position - transform.position, 1000,
                                            mask);
                if (hit.collider != null && hit.collider.gameObject == closestTarget) {
                    Vector2 moveDirection = closestTarget.transform.position - transform.position; 
                    if (moveDirection != Vector2.zero) 
                    {
                        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
            
                    if (hit.distance < myAttackComponent.Weapon.AttackRange) {
                        myAttackComponent.TryAttackTarget(closestTarget);
                    } else if (isMelee) {
                        Move(moveDirection.normalized);
                    }
                    
                    return;
                }
            } 
            
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
            if (upgrade.Tool.NeedFactory && !SessionManager.Instance.CheckFactories()) {
                continue;
            }
            
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
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (autoAttackMask.value == (autoAttackMask.value | (1 << other.gameObject.layer))) {
            myTargets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (myTargets.Contains(other.gameObject)) {
            myTargets.Remove(other.gameObject);
        }
    }

    public bool IsUpdated() => isUpdated;
}
