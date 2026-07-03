public abstract class EnemyBaseState<T> : IEnemyState where T : Enemy
{
    // 해당 상태를 사용하는 객체 참조.
    protected readonly T enemy;

    // 상태 전환을 위한 객체 참조.
    protected readonly EnemyStateMachine<T> stateMachine;

    public EnemyBaseState(T enemy, EnemyStateMachine<T> stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}