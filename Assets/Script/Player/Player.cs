using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    // 플레이어의 컴포넌트 클래스 참조
    private PlayerController playerController;
    private PlayerAttack playerAttack;

    [Header("기본 설정 (Defaults)")]
    [SerializeField] private float maxHp = 100f;                    // 최대 체력
    [SerializeField] private float currentHp;                       // 현재 체력
    [SerializeField] private float attackDamage = 10f;               // 기본 데미지
    [SerializeField] private float attackDelay = 0.5f;                 // 사격 딜레이
    [SerializeField] private float attackRange = 50f;               // 사거리.
    [SerializeField] private float moveSpeed = 10f;                 // 이동 속도

    // 멤버에 접근하는 프로퍼티
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public float AttackDamage => attackDamage;
    public float AttackDelay => attackDelay;
    public float AttackRange => attackRange;
    public float MoveSpeed => moveSpeed;

    void Awake()
    {
        // 컴포넌트 읽어오기
        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponent<PlayerAttack>();

        playerController.MoveSpeed = MoveSpeed;

        playerAttack.AttackDamage = attackDamage;
        playerAttack.AttackDelay = attackDelay;
        playerAttack.AttackRange = attackRange;

        currentHp = maxHp;
    }

    // 데미지를 입는 메서드.
    public void TakeDamage(float value)
    {
        Debug.Log($"Player Take Damage! : {value}");

        currentHp -= value;

        // 체력이 0 이하라면 사망처리.
        if(currentHp <= 0)
        {
            OnDead();
        }
    }

    // 플레이어 사망.
    // 게임 일시 정지.
    private void OnDead()
    {
        Debug.Log("Player is Dead!");
        
        // 컨트롤러와 슈터 비활성화
        playerController.enabled = false;
        playerAttack.enabled = false;
        
        // 게임 상태를 변경
        GameManager.Instance.ChangeState(GameState.GameOver);
    }
}