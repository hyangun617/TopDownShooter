using System;
using System.Collections.Generic;

public class BehaviorTreeBuilder
{
    private Blackboard blackboard;                                  // Leaf 노드들의 생성자에 넘겨줘야 하므로 Blackboard를 들고 있어야 함.
    private Stack<INode> parentStack = new Stack<INode>();          // 괄호 짝 맞추기 구조를 위한 Stack. 
    private INode rootNode;                                         // 최종적으로 완성된 트리의 최상위 노드. Build() 호출 시 이 노드를 반환함.

    
    // 생성자
    public BehaviorTreeBuilder(Blackboard blackboard)   { this.blackboard = blackboard; }

    // 부모 노드에 연결
    private void AttachToParent(INode node)
    {
        // 스택이 비어있다면 해당 노드가 루트
        if(parentStack.Count == 0)
        {
            rootNode = node;
            return;
        }
    }

    // 컴포사이트 노드 추가
    public BehaviorTreeBuilder Sequence()
    {
        var node = new SequenceNode();
        AttachToParent(node);
        parentStack.Push(node);
        return this;
    }

    // 셀렉터 노드 추가
    public BehaviorTreeBuilder Selector()
    {
        var node = new SelectorNode();
        AttachToParent(node);
        parentStack.Push(node);
        return this;
    }

    // 데코레이터 노드 추가
    public BehaviorTreeBuilder Decorator(DecoratorNode decoratorNode)
    {
        AttachToParent(decoratorNode);
        parentStack.Push(decoratorNode);
        return this;
    }

    // 괄호 닫기. 
    // Composite/Decorator 메서드 호출 수와 End 메서드의 호출 수가 같아야 함.
    public BehaviorTreeBuilder End()
    {
        parentStack.Pop();
        return this;
    }

    // 리프 노드 추가.
    public BehaviorTreeBuilder Leaf(LeafNode leafNode)
    {
        AttachToParent(leafNode);
        return this;
    }

    public INode Build()
    {
        if(parentStack.Count != 0)
            throw new InvalidOperationException("BT Builder: End() 호출 안 된 Composite/Decorator 노드가 존재합니다.");

        return rootNode;
    }
}   