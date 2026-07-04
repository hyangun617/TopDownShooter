public class SequenceNode : CompositeNode
{
    // 지난 프레임에 Running인 자식의 인덱스
    private int lastRunningIndex = -1;

    // 모든 자식이 Success여야 Success.
    // 하나라도 Failure이면 즉시 Failure.
    public SequenceNode(params INode[] nodes) : base(nodes) { }

    public override NodeState Tick()
    {
        int previousRunningIndex = lastRunningIndex;

        NodeState result = Evaluate(out int currentRunningIndex);

        // 취소 판단은 여기 딱 한 곳에서만 처리
        if (previousRunningIndex >= 0 && previousRunningIndex != currentRunningIndex)
        {
            children[previousRunningIndex].Cancel();
        }

        lastRunningIndex = currentRunningIndex;
        
        return result;
    }

    // 순수하게 결과만 계산하는 노드
    // 취소는 신경쓰지 않음
    private NodeState Evaluate(out int runningIndex)
    {
        runningIndex = -1;

        for(int i = 0; i < children.Count; ++i)
        {
            NodeState result = children[i].Tick();

            if(result == NodeState.Failure)
            {
                return NodeState.Failure;
            }

            if(result == NodeState.Running)
            {
                runningIndex = i;
                return NodeState.Running;
            }
        }

        return NodeState.Success;
    }

    public override void Cancel()
    {
        base.Cancel();

        if(lastRunningIndex >= 0)
        {
            children[lastRunningIndex].Cancel();
            lastRunningIndex = -1;
        }
    }
}