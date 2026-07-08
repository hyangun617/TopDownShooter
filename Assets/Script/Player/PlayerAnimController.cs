using UnityEngine;

[RequireComponent(typeof(PlayerAnimController))]
public class PlayerAnimController : MonoBehaviour
{
    private Animator animator;
    private int upperBodyLayerIndex;
    private RuntimeAnimatorController defaultController;

    // 애니메이션 파라미터
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int MoveForwardHash = Animator.StringToHash("Forward");
    private static readonly int MoveRightHash = Animator.StringToHash("Right");
    private static readonly int IsMovedHash = Animator.StringToHash("IsMoved");

    // 애니메이션 보간 값.
    private const float DampTime = 0.1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        upperBodyLayerIndex = animator.GetLayerIndex("Upper Body");
        defaultController = animator.runtimeAnimatorController;
    }

    void Start()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
    }

    public void UpdateMoveParams(Vector3 localMove, float moveSpeedNormalized, bool isMoved)
    {
            // 애니메이터 파라미터 수정
            animator.SetFloat(SpeedHash, moveSpeedNormalized, DampTime, Time.deltaTime);
            animator.SetFloat(MoveForwardHash, localMove.z, DampTime, Time.deltaTime);
            animator.SetFloat(MoveRightHash, localMove.x, DampTime, Time.deltaTime);
            animator.SetBool(IsMovedHash, isMoved);
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
