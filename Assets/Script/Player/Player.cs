using UnityEngine;

// 플레이어 유닛. 체력 / 이동 / 애니메이션은 Unit 의 공용 컴포넌트를 사용하고,
// 입력 기반 동작은 PlayerController, 공격은 PlayerAttack 이 담당한다.
public class Player : Unit
{
    private PlayerController playerController;
    private PlayerAttack playerAttack;

    [Header("기본 설정 (Defaults)")]
    [SerializeField] private float maxHp = 100f;                    // 최대 체력
    [SerializeField] private float attackDamage = 10f;              // 기본 데미지
    [SerializeField] private float attackDelay = 0.5f;              // 사격 딜레이
    [SerializeField] private float attackRange = 50f;               // 사거리.
    [SerializeField] private float moveSpeed = 10f;                 // 이동 속도

    protected override void Awake()
    {
        base.Awake();

        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponent<PlayerAttack>();

        health.Initialize(maxHp);
        health.OnDeath += HandleDeath;

        playerController.Initialize(moveSpeed);

        playerAttack.AttackDamage = attackDamage;
        playerAttack.AttackDelay = attackDelay;
        playerAttack.AttackRange = attackRange;
    }

    // 플레이어 사망 : 입력을 막고 게임을 종료 상태로 전환.
    private void HandleDeath()
    {
        playerController.enabled = false;
        playerAttack.enabled = false;

        GameManager.Instance.ChangeState(GameState.GameOver);
    }
}
