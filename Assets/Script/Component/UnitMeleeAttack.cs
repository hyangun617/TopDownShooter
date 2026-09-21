using UnityEngine;

public class UnitMeleeAttack : UnitAttack
{
    // 공격 좌우 폭, 높이 
    [Header("Range")]
    [SerializeField] private float attackWidth = 1f;
    [SerializeField] private float attackHeight = 1f;

    // 재사용 버퍼
    private readonly Collider[] result = new Collider[10];

    private void Awake()
    {
        TargetLayerMask = LayerMask.GetMask("Player");
    }

    // 공격 실행 메서드. (애니메이션 이벤트에서 Enemy를 통해 호출됨)
    public override void PlayAttack()
    {
        StartCooldown();

        PlayAttackSfx(worldPosition: transform.position, clipVolume: 0.5f, pitch: 1.5f);

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

        int count = Physics.OverlapBoxNonAlloc(center, halfExtents, result, rotation, TargetLayerMask);
        for (int i = 0; i < count; i++)
        {
            if (result[i].TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.TakeDamage(AttackDamage);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 공격 범위(box) 표시
        GetAttackBox(out Vector3 center, out Quaternion rotation, out Vector3 halfExtents);

        Gizmos.color = IsCoolingDown ? Color.red : Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}
