using System;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    [SerializeField] private int id;
    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    [SerializeField] private float attackPoint;
    [SerializeField] private float defensePoint;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackRange;
    [SerializeField] private EnemyType enemyType;
}