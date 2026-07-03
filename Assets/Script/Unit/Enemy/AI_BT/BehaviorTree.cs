public class BehaviorTree
{
    // 실제 객체 안에서 실행되도록 하는 러너.
    
    private Blackboard blackboard;
    private INode rootNode;

    public BehaviorTree(INode rootNode, Blackboard blackboard)
    {
        this.rootNode = rootNode;
        this.blackboard = blackboard;
    }

    // 매 프레임 외부 Update에서 호출.
    public NodeState Tick()
    {
        return rootNode.Tick();
    }

    public Blackboard GetBlackboard() => blackboard;
}