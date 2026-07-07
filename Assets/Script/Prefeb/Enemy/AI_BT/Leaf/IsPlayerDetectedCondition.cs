using UnityEngine;

// RangeEnemy의 Player 탐지 로직. 
// OverlapSphere + RayCast로 탐지
public class IsPlayerDetectedCondition : LeafNode
{
    public IsPlayerDetectedCondition(Blackboard blackboard) : base(blackboard) { }

    public override NodeState Tick()
    {
        // 필수 데이터 조회. 하나라도 없으면 판단이 불가하므로 Failure
        if(!blackboard.TryGetValue<Enemy>(BlackboardKeys.Self, out Enemy self))
        {
            return NodeState.Failure;
        }
        // ====================================================================== //

        // OverlapSphere로 범위 내 후보 탐색
        Collider[] hits = Physics.OverlapSphere(self.transform.position, self.DetectRange, self.TargetLayerMask);

        if(hits.Length == 0)
            return NodeState.Failure;   // 범위 내에 아무도 없음.

        Transform candidate = hits[0].transform;  // 가장 처음 맞은 객체의 transform을 가져옴.

        // RayCast로 시야 차단 여부 확인
        Vector3 direction = candidate.position - self.transform.position;

        if(blackboard.TryGetValue<LayerMask>(BlackboardKeys.ObstacleLayerMask, out LayerMask obstacleLayer))
        {
            if(Physics.Raycast(self.transform.position, direction.normalized, direction.magnitude, obstacleLayer))
            {
                return NodeState.Failure;       // 장애물로 인해 시야 차단 -> Failure
            }
        }

        // 탐지 성공
        // 다른 노드가 사용할 수 있도록 Blackboard에 기록.
        blackboard.SetValue(BlackboardKeys.Target, candidate);
        return NodeState.Success;
    }
}