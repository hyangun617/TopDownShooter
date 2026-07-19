using UnityEngine;

public class AttackAction : LeafNode
{
    public AttackAction(Blackboard blackboard) : base(blackboard)
    {

    }

    public override NodeState Tick()
    {
        if(!blackboard.TryGetValue<RangeEnemy>(BlackboardKeys.Self, out RangeEnemy self))
        {
            return NodeState.Failure;
        }
        if(!blackboard.TryGetValue<Transform>(BlackboardKeys.Target, out Transform target))
        {
            return NodeState.Failure;
        }

        // 객체의 이동을 중지
        self.SetMoveState(false);
        self.StopMoving();

        // 타겟에게로의 방향 계산
        Vector3 dir = target.position - self.Position;
        dir.y = 0f;
        dir.Normalize();

        self.Rotate(dir);

        self.AttackTrigger();

        return NodeState.Success;
    }

    public override void Cancel()
    {
        base.Cancel();
    }
}