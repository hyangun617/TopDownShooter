public class SequenceNode : CompositeNode
{
    // 모든 자식이 Success여야 Success.
    // 하나라도 Failure이면 즉시 Failure.
    public SequenceNode(params INode[] nodes) : base(nodes) { }

    public override NodeState Tick()
    {
        // 각 노드를 순회하며 검사함.
        foreach(var child in children)
        {
            NodeState result = child.Tick();

            if(result == NodeState.Failure) return NodeState.Failure;
            if(result == NodeState.Running) return NodeState.Running;

            // NodeState.Success면 다음 자식으로 계속 진행.
        }

        // foreach문을 통과했다 -> 모든 자식 노드의 실행 성공.
        return NodeState.Success;
    }
}