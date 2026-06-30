using System;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    [SerializeField] private int _id;
    [SerializeField] private float _maxHp;
    [SerializeField] private float _attackPoint;
    [SerializeField] private float _defensePoint;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _attackRange;
    [SerializeField] private EnemyType _enemyType;

    public int Id => _id;
    public float MaxHp => _maxHp;
    public float AttackPoint => _attackPoint;
    public float DefensePoint => _defensePoint;
    public float MovementSpeed => _movementSpeed;
    public float AttackDelay => _attackDelay;
    public float AttackRange => _attackRange;
    public EnemyType EnemyType => _enemyType;
}