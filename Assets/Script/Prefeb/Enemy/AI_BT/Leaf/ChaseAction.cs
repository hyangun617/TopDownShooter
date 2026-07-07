using UnityEngine;

public class ChaseAction : LeafNode
{
    private Enemy _self;

    public ChaseAction(Blackboard blackboard) : base(blackboard) { }

    public override NodeState Tick()
    {
        if(!blackboard.TryGetValue<Enemy>(BlackboardKeys.Self, out Enemy self))
        {
            return NodeState.Failure;
        }
        _self = self;

        if(!blackboard.TryGetValue<Transform>(BlackboardKeys.Target, out Transform target))
        {
            return NodeState.Failure;
        }

        // 사거리 내라면 종료
        float dist = Vector3.Distance(self.transform.position, target.position);
        if(dist <= self.AttackRange)
        {
            return NodeState.Success;
        }
        
        // 플레이어의 이동 메서드 호출
        self.MoveToward(target.position, self.MoveSpeed);
        return NodeState.Running;
    }

    public override void Cancel()
    {
        base.Cancel();

        _self.StopMoving();
    }
}