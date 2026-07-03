using UnityEngine;

// RangeEnemy의 Player 탐지 로직. 
// OverlapSphere + RayCast로 탐지
public class IsPlayerDetectedCondition : LeafNode
{
    private Blackboard blackboard;

    public IsPlayerDetectedCondition(Blackboard blackboard) { this.blackboard = blackboard; }

    public override NodeState Tick()
    {
        
    }
}