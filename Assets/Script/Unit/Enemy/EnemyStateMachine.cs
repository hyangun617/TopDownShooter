using System;
using System.Collections.Generic;

public class EnemyStateMachine<T> where T : Enemy
{
    public IEnemyState CurrentState { get; private set; }

    /// <summary>
    ///  상태 머신이 각 상태들을 직접 소유함.
    /// </summary>
    private readonly Dictionary<Type, IEnemyState> states = new();

    // 각 상태 등록
    public void Register(IEnemyState state)
    {
        states[state.GetType()] = state;
    }

// 상태 초기화 메서드
    public void Initialize<TState>() where TState : IEnemyState
        => ChangeState<TState>();

// 상태 전환 메서드
    public void ChangeState<TState>() where TState : IEnemyState
    {
        CurrentState?.Exit();
        CurrentState = states[typeof(TState)];
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}