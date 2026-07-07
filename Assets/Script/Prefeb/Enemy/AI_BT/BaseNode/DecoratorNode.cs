
public abstract class DecoratorNode : INode
{
    // Decorator노드는 자식이 하나이기에 멤버로 참조함.
    protected INode child;

    public DecoratorNode(INode child)
    {
        this.child = child;
    }

    // Builder 전용 - 자식 없이 생성 후 나중에 SetChild
    protected DecoratorNode() { }

    public void SetChild(INode child)
    {
        this.child = child;
    }

    public INode GetChild()
    {
        return child;
    }

    public abstract NodeState Tick();

    public virtual void Cancel()
    {
        child.Cancel();
    }
}