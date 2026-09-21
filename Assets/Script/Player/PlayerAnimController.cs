using UnityEngine;

// UnitAnimController 에 플레이어 전용 동작(이동 블렌딩, 재장전, 무기별 애니메이션)을 추가한 컴포넌트.
public class PlayerAnimController : UnitAnimController
{
    private RuntimeAnimatorController defaultController;

    // 플레이어 애니메이터의 파라미터
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int MoveForwardHash = Animator.StringToHash("Forward");
    private static readonly int MoveRightHash = Animator.StringToHash("Right");
    private static readonly int IsMovedHash = Animator.StringToHash("IsMoved");
    private static readonly int IsReloadHash = Animator.StringToHash("IsReload");
    private static readonly int IsShootHash = Animator.StringToHash("IsShoot");

    private int upperBodyLayerIndex;

    // 애니메이션 보간 값.
    private const float DampTime = 0.1f;

    // 공용 동작(SetMoveState / AttackTrigger)이 플레이어 파라미터를 사용하도록 교체.
    protected override int MoveParam => IsMovedHash;
    protected override int AttackParam => IsShootHash;

    protected override void Awake()
    {
        base.Awake();

        upperBodyLayerIndex = animator.GetLayerIndex("Upper Body");
        defaultController = animator.runtimeAnimatorController;
    }

    private void Start()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
    }

    public void UpdateMoveParams(Vector3 localMove, float moveSpeedNormalized, bool isMoved)
    {
        animator.SetFloat(SpeedHash, moveSpeedNormalized, DampTime, Time.deltaTime);
        animator.SetFloat(MoveForwardHash, localMove.z, DampTime, Time.deltaTime);
        animator.SetFloat(MoveRightHash, localMove.x, DampTime, Time.deltaTime);
        SetMoveState(isMoved);
    }

    // 재장전 애니메이션을 재생하고, 그 길이를 반환.
    public float OnReload()
    {
        animator.SetTrigger(IsReloadHash);
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(upperBodyLayerIndex);
        return info.length / animator.speed;
    }

    public void EquipWeaponAnimation(WeaponData weaponData)
    {
        animator.runtimeAnimatorController =
            weaponData != null && weaponData.upperBodyOverride != null
                ? weaponData.upperBodyOverride
                : defaultController;

        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
    }
}
