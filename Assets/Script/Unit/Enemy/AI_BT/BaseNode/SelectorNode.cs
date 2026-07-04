public class SelectorNode : CompositeNode
{
    private int lastRunningIndex = -1;

    public SelectorNode(params INode[] nodes) : base(nodes) { }

    public override NodeState Tick()
    {
        int previousRunningIndex = lastRunningIndex;

        NodeState result = Evaluate(out int currentRunningIndex);

        if (previousRunningIndex >= 0 && previousRunningIndex != currentRunningIndex)
        {
            children[previousRunningIndex].Cancel();
        }

        lastRunningIndex = currentRunningIndex;
        return result;
    }

    private NodeState Evaluate(out int runningIndex)
    {
        runningIndex = -1;

        for (int i = 0; i < children.Count; ++i)
        {
            NodeState result = children[i].Tick();

            if (result == NodeState.Success)
                return NodeState.Success;

            if (result == NodeState.Running)
            {
                runningIndex = i;
                return NodeState.Running;
            }
        }

        return NodeState.Failure;
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