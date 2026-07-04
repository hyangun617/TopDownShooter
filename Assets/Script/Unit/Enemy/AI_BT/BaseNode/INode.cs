public interface INode
{
    NodeState Tick();

    // 인터럽트 당했을 때 정지 호출
    void Cancel();
}