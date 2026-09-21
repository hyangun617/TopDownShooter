using UnityEngine;

// Player와 Enemy가 공유하는 유닛 베이스.
// 체력 / 이동 / 애니메이션 / 공격 컴포넌트를 한 곳에서 캐싱하고, 자주 쓰는 기능을 위임 메서드로 제공한다.
[RequireComponent(typeof(UnitHealth), typeof(UnitController), typeof(UnitAnimController))]
public abstract class Unit : MonoBehaviour
{
    protected UnitHealth health;
    protected UnitController controller;
    protected UnitAnimController animController;
    protected UnitAttack attack;            // 공격 컴포넌트가 없는 유닛도 있을 수 있음.

    public UnitHealth Health => health;
    public Vector3 Position => controller.Position;

    protected virtual void Awake()
    {
        health = GetComponent<UnitHealth>();
        controller = GetComponent<UnitController>();
        animController = GetComponent<UnitAnimController>();     // 파생 클래스(PlayerAnimController)도 반환됨.
        attack = GetComponent<UnitAttack>();
    }

    // 위임 메서드
    // Controller
    public void MoveToward(Vector3 targetPosition, float speed) => controller.MoveToward(targetPosition, speed);
    public void StopMoving() => controller.StopMoving();
    public void Rotate(Vector3 dir) => controller.Rotate(dir);

    // Animator
    public void SetMoveState(bool state) => animController.SetMoveState(state);
    public void AttackTrigger() => animController.AttackTrigger();
    public void DeathTrigger() => animController.DeathTrigger();
}
