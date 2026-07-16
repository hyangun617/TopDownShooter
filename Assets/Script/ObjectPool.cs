using System;
using System.Collections.Generic;

public class ObjectPool<T>
{
    private readonly Queue<T> pool = new Queue<T>();

    private readonly Func<T> createFunc;        // 생성자 함수.
    private readonly Action<T> actionGet;       // 꺼낼 때 동작할 함수
    private readonly Action<T> actionRelease;   // 넣을 때 동작할 함수.

    public ObjectPool(Func<T> createFunc, Action<T> actionGet, Action<T> actionRelease, int InitSize)
    {
        this.createFunc = createFunc;
        this.actionGet = actionGet;
        this.actionRelease = actionRelease;

        // 기본적으로 InitSize 만큼 객체 생성.
        for(int i = 0; i < InitSize; ++i)
        {
            T obj = createFunc();
            actionRelease?.Invoke(obj);
            pool.Enqueue(obj);
        }
    }

    // 객체를 가져올 메서드
    public T Get()
    {
        T obj = pool.Count > 0 
            ? pool.Dequeue()
            : createFunc();

        actionGet?.Invoke(obj);
        return obj;
    }

    // 객체를 반환하는 메서드
    public void Release(T obj)
    {
        if(obj == null)
        {
            MyGame.Utility.Debugger.Log($"Object {typeof(T)} : {obj} Object Pool Release Fail : null reference");
            return;
        }

        actionRelease?.Invoke(obj);
        pool.Enqueue(obj);
    }

    public void Clear(Action<T> onDestroy = null)
    {
        while(pool.Count > 0)
        {
            T obj = pool.Dequeue();
            onDestroy?.Invoke(obj);
        }
    }
}