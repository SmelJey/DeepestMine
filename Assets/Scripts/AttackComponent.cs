﻿
using System.Collections;
using UnityEngine;

public class AttackComponent : MonoBehaviour {
    public bool IsAttacking { get; private set; }
    public DwarfTool Weapon;

    public LayerMask CollisionMask;
    [SerializeField] private LayerMask attackMask;
    
    public void TryAttackTarget(GameObject target) {
        Debug.DrawRay(transform.position, (target.transform.position - transform.position).normalized, Color.red);
        var hit = Physics2D.Raycast(transform.position,
                                    (target.transform.position - transform.position).normalized, Weapon.AttackRange, CollisionMask);

        if (hit.collider != null && hit.collider.gameObject == target) {
            var obj = hit.collider.gameObject;
            var hitComp = obj.GetComponent<HpComponent>();
            if (hitComp != null) {
                if (attackMask.value != (attackMask.value | (1 << obj.layer)) || obj.CompareTag("Wall") && !Weapon.CanAttackRocks) {
                    return;
                }

                if (hit.distance < Weapon.AttackRange) {
                    StartCoroutine(Attack(hitComp));
                }
            }
        }
    }
    
    private IEnumerator Attack(HpComponent component) {
        if (IsAttacking) {
            yield break;
        }

        Vector2 moveDirection = component.transform.position - transform.position; 
        if (moveDirection != Vector2.zero) 
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        IsAttacking = true;
        component.GetHit(Weapon.AttackDamage, gameObject);

        yield return new WaitForSeconds(Weapon.AttackCooldown);
        IsAttacking = false;
    }
}