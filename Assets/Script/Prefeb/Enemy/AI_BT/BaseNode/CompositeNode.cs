using System.Collections.Generic;

public abstract class CompositeNode : INode
{
    // 여러개의 자식 노드를 담을 컨테이너
    protected List<INode> children = new List<INode>();

    public CompositeNode(params INode[] nodes)
    {
        children.AddRange(nodes);
    }

    public void AddChild(INode child)
    {
        children.Add(child);
    }

    public abstract NodeState Tick();

    public virtual void Cancel() { }
}