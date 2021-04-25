using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private int movementSpeed = 15;

    private AttackComponent myAttackComponent;
    private Seeker mySeeker;
    private Path myPath;
    private int myCurrentWaypoint = 0;
    private float nextWaypointDistance = 0.5f;

    private HashSet<GameObject> myTargets;
    private Rigidbody2D myRigidbody2D;
    
    private void Awake() {
        myAttackComponent = GetComponent<AttackComponent>();
        mySeeker = GetComponent<Seeker>();
        myTargets = new HashSet<GameObject>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        mySeeker.StartPath(transform.position, SessionManager.Instance.HQPosition, path => {
            if (!path.error) {
                Debug.Log(path.errorLog, this);
            }

            myPath = path;
            myCurrentWaypoint = 0;
        });
    }

    private void Update() {
        GameObject closestTarget = null;
        float closestDist = 1e5f;
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
            var hit = Physics2D.Raycast(transform.position, closestTarget.transform.position - transform.position, 1000,
                                        myAttackComponent.CollisionMask);
            if (hit.collider != null && hit.collider.gameObject == closestTarget) {
                Vector2 moveDirection = closestTarget.transform.position - transform.position; 
                if (moveDirection != Vector2.zero) 
                {
                    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    //moveDirection = Vector2.right;
                }
            
                if (hit.distance < myAttackComponent.Weapon.AttackRange) {
                    myAttackComponent.TryAttackTarget(closestTarget);
                } else {
                    Move(moveDirection.normalized);
                }
                return;
            }
        }

        if (myAttackComponent.IsAttacking) {
            myRigidbody2D.velocity = Vector2.zero;
            return;
        }

        if (myPath == null || myPath.vectorPath.Count == 0 || Vector2.Distance(myPath.vectorPath[myCurrentWaypoint], transform.position) > 2) {
            myPath = null;

            if (mySeeker.GetCurrentPath().PipelineState > PathState.Processing) {
                mySeeker.StartPath(transform.position, SessionManager.Instance.HQPosition, path => {
                    if (path.error) {
                        Debug.Log(path.errorLog, this);
                        return;
                    }

                    myPath = path;
                    myCurrentWaypoint = 0;
                });
            }

            var hqHit = Physics2D.Raycast(transform.position, (Vector3)SessionManager.Instance.HQPosition - transform.position, 1000,
                                          myAttackComponent.CollisionMask);
            if (hqHit.collider != null) {
                myAttackComponent.TryAttackTarget(hqHit.collider.gameObject);
            }

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
                    return;
                }
            } else {
                break;
            }
        }

        Vector2 moveDir = myPath.vectorPath[myCurrentWaypoint] - transform.position; 
        Move(moveDir.normalized);
    }

    private void Move(Vector2 direction) {
        if (direction != Vector2.zero) 
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        myRigidbody2D.velocity = direction * movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        myTargets.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (myTargets.Contains(other.gameObject)) {
            myTargets.Remove(other.gameObject);
        }
    }
}
