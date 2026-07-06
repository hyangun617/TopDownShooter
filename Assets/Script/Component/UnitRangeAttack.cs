using UnityEngine;
using System.Collections;

public class UnitRangeAttack : MonoBehaviour, IAttackable
{
    // IAttackable 인터페이스 멤버 
    public float AttackDamage { get; set; }
    public float AttackRange { get; set; }
    public float AttackDelay { get; set; }

    // 공격 시작 지점
    public Transform firePoint;

    // 공격 대상 레이어 마스크
    private LayerMask targetLayerMask;

    // 공격 여부
    private bool isAttacking = false;

    // SFX
    [SerializeField] private AudioClip attackSFX;

    public void PlayAttack()
    {
        // 원거리 공격 메서드
        StartCoroutine(OnRangeAttacking());
        Debug.Log("Range Attack!");
    }

    private IEnumerator OnRangeAttacking()
    {
        isAttacking = true;
        yield return new WaitForSeconds(AttackDelay);
        isAttacking = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(isAttacking)
        {
            Gizmos.color = Color.red;
            Vector3 start = firePoint.position;
            Vector3 end = start + transform.forward * AttackRange;
            Gizmos.DrawLine(start, end);    
        }        
    }
#endif
}