
public interface IEnemyState
{
    // 이 상태로 진입할 때, 최초 1회 실행.
    void Enter();

    // 매 프레임 실행
    void Update();

    // 이 상태를 종료할 때 1회 실행.
    void Exit();
}
