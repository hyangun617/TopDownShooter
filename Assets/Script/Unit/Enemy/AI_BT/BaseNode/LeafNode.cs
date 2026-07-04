public abstract class LeafNode : INode
{
    protected Blackboard blackboard;

    protected LeafNode(Blackboard blackboard) { this.blackboard = blackboard; }

    public abstract NodeState Tick();

    public virtual void Cancel() { }
}