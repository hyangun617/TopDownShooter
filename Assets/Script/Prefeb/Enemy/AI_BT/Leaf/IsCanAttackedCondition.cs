
using UnityEngine;

// 공격 사거리 검사 노드
public class IsCanAttackedCondition : LeafNode
{
    public IsCanAttackedCondition(Blackboard blackboard) : base(blackboard) { }

    public override NodeState Tick()
    {
        if(!blackboard.TryGetValue<Enemy>(BlackboardKeys.Self, out Enemy self))
        {
            return NodeState.Failure;
        }
        if(!blackboard.TryGetValue<Transform>(BlackboardKeys.Target, out Transform target))
        {
            return NodeState.Failure;
        }

        float dist = Vector3.Distance(self.transform.position, target.position);
        // 사거리 내라면 성공
        if(dist <= self.AttackRange)
        {
            self.SetMoveState(false);
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}