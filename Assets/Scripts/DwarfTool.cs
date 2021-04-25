﻿using UnityEngine;

[CreateAssetMenu(fileName = "DwarfTool", menuName = "LevelGeneration/DwarfTool", order = 0)]
public class DwarfTool : ScriptableObject {
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private bool canAttackRocks;
    [SerializeField] private Sprite toolSprite;

    public float AttackRange => attackRange;
    public int AttackDamage => attackDamage;
    public float AttackCooldown => attackCooldown;
    public bool CanAttackRocks => canAttackRocks;
    public Sprite ToolSprite => toolSprite;
}
