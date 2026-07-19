using UnityEngine;

// 공격 노드가 얼마나 자주 실행될 수 있는지 체크용 노드
public class CheckAttackDelay : LeafNode
{
    private float attackdelay;
    private float lastExecutionTime = -Mathf.Infinity;
    public CheckAttackDelay(Blackboard blackboard, float attackdelay) : base(blackboard)
    {
        this.attackdelay = attackdelay;
    }

    public override NodeState Tick()
    {
        if(!blackboard.TryGetValue<RangeEnemy>(BlackboardKeys.Self, out var self))
        {
            return NodeState.Failure;
        }

        // 아직 쿨 다운 중이라면 실행하지 않음.
        if(Time.time - lastExecutionTime < attackdelay)
        {
            return NodeState.Running;
        }

        lastExecutionTime = Time.time;

        return NodeState.Success;
    }
}