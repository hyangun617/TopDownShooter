using UnityEngine;

public class WaitAction : LeafNode
{
    private float waitDuration;               // 정지 시간
    private float elapsedTime = 0f;           // 경과 시간
    private bool isWaiting = false;           // 정지 여부

    public WaitAction(Blackboard blackboard, float waitDuration) : base(blackboard)
    {
        this.waitDuration = waitDuration;
    }

    public override NodeState Tick()
    {
        // 이미 정지 여부 확인
        // 정지 여부가 아니라면 정지.
        if(!isWaiting)
        {
            isWaiting = true;
            elapsedTime = 0f;
        }

        // 경과 시간 누적.
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= waitDuration)
        {
            isWaiting = false;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}