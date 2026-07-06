using UnityEngine;

public interface IAttackable
{
    float AttackDamage { get; set; }
    float AttackRange { get; set; }
    float AttackDelay { get; set; }

    public void PlayAttack(Vector3 dir);
}