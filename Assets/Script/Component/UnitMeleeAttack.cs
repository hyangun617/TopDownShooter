using UnityEngine;
using System.Collections;

public class UnitMeleeAtack : MonoBehaviour, IAttackable
{
    // IAttackable
    public float AttackDamage { get; set; }
    public float AttackRange { get; set; }
    public float AttackDelay { get; set; }

    // 공격 좌우 폭, 높이 
    [SerializeField] private float attackWidth = 1f;
    [SerializeField] private float attackHeight = 1f;

    // 공격 대상 레이어 마스크
    private LayerMask targetLayerMask; 

    // 재사용 버퍼
    Collider[] result = new Collider[10];

    // 공격 중 여부
    private bool isAttacking = false;

    void Awake()
    {
        targetLayerMask = LayerMask.GetMask("Player");
    }

    // 공격 실행 메서드.
    public void PlayAttack(Vector3 dir)
    {
        MyGame.Utility.Debugger.Log($"{name}'s Attack!");

        // 타겟을 향해 회전
        transform.rotation = Quaternion.LookRotation(dir);

        // 기즈모 박스 그리기
        StartCoroutine(ShowAttackGizmo());

        // 실제 데미지 판정.
        CheckMeleeHit();
    }

    // 공격 판정에 쓸 박스 정보를 계산하는 공용 함수.
    private void GetAttackBox(out Vector3 center, out Quaternion rotation, out Vector3 halfExtents)
    {
        rotation = transform.rotation;
        center = transform.position + transform.forward * (AttackRange * 0.5f);
        halfExtents = new Vector3(attackWidth * 0.5f, attackHeight * 0.5f, AttackRange * 0.5f);
    }

    // 실제 공격 판정
    private void CheckMeleeHit()
    {
        GetAttackBox(out Vector3 center, out Quaternion rotation, out Vector3 halfExtents);
        
         int count = Physics.OverlapBoxNonAlloc(center, halfExtents, result, rotation, targetLayerMask);
        for (int i = 0; i < count; i++)
        {
            if(result[i].TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.TakeDamage(AttackDamage);
            }
        }
    }

    private IEnumerator ShowAttackGizmo()
    {
        isAttacking = true;
        yield return new WaitForSeconds(AttackDelay);
        isAttacking = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // 공격 범위(box) 표시
        GetAttackBox(out Vector3 center, out Quaternion rotation, out Vector3 halfExtents);

        Gizmos.color = isAttacking? Color.red : Color.yellow;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}