public class SelectorNode : CompositeNode
{
    public SelectorNode(params INode[] nodes) : base(nodes) { }

    public override NodeState Tick()
    {
        foreach(var child in children)
        {
            NodeState result = child.Tick();

            if(result == NodeState.Success) return NodeState.Success;
            if(result == NodeState.Running) return NodeState.Running;

            // Failure인 경우 다음 자식으로 넘어감.
        }

        return NodeState.Failure;
    }
}