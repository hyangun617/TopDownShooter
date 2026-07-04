using UnityEngine;

public class AttackAction : LeafNode
{
    // 공격 딜레이 설정
    private float attackDelay;
    private float attackStartTime = -1f;        // -1은 공격 중 아님을 의미.

    public AttackAction(Blackboard blackboard, float attackDelay) : base(blackboard)
    {
        this.attackDelay = attackDelay;
    }

    public override NodeState Tick()
    {
        if(!blackboard.TryGetValue<RangeEnemy>(BlackboardKeys.Self, out RangeEnemy self))
        {
            return NodeState.Failure;
        }

        // 객체의 이동을 중지
        self.StopMoving();

        if(attackStartTime < 0f)
        {
            // 객체의 공격 함수 실행.
            self.RangeAttack();
            attackStartTime = Time.time;
        }

        if(Time.time - attackStartTime >= attackDelay)
        {
            attackStartTime = -1f;      // 다음 공격을 위해 리셋
            return NodeState.Success;    // 딜레이 끝. -> 다시 판단 가능.
        }

        return NodeState.Running;
    }

    public override void Cancel()
    {
        base.Cancel();
        attackStartTime = -1;
    }
}