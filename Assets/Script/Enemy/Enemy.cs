using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    private float maxHp = 100;
    private float currentHp = 100;
    private float attackPoint = 10;
    private float defensePoint = 10;
    private float movementSpeed = 10f;
    private float attackDelay = 0.5f;
    private float attackRange = 10f;
    private EnemyType enemyType = EnemyType.Melee;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int value)
    {

    }
}
