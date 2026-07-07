using Unity.Mathematics;
using UnityEngine;

public class BehaviorTree
{
    // 실제 객체 안에서 실행되도록 하는 러너.
    
    private Blackboard blackboard;
    private INode rootNode;

    // AI 실행 여부
    public bool IsPaused { get; private set; } = false;

    // Tick 갱신 시간.
    private float elapsedTime = 0f;         // 경과 시간
    private float delayTime = 0.5f;         // 지연 시간, 기본 값 0.5f

    public BehaviorTree(INode rootNode, Blackboard blackboard)
    {
        this.rootNode = rootNode;
        this.blackboard = blackboard;
    }

    // 매 프레임 외부 Update에서 호출.
    public NodeState Tick()
    {
        NodeState result = NodeState.Success;

        elapsedTime += Time.deltaTime;

        // 성능을 위해 딜레이 설정.
        if(elapsedTime >= delayTime)
        {
            result = rootNode.Tick();    
            elapsedTime = 0f;
        }

        return result;        
    }

    // 강제 실행 취소
    public void Cancel()
    {
        rootNode.Cancel();
    }

    public void SetDelay(float delayTime)
    {
        this.delayTime = delayTime;
    }

    // BT 실행 및 정지 메서드
    public void Play() => IsPaused = false;
    public void Pause()
    {
        if (IsPaused) return;

        IsPaused = true;
        rootNode.Cancel();
    }

    public Blackboard GetBlackboard() => blackboard;
}