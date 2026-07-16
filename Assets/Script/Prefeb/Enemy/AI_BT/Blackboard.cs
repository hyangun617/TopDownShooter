using System.Collections.Generic;

public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public void SetValue(string key, object value)
    {
        data[key] = value;
    }


    // <T>의 형태의 값을 key 값을 이용해 탐색 및 가져옴. 없으면 T 객체의 기본 값을 반환.
    // 주의 : 타입을 잘못 지정해서 값을 꺼낼 경우 InvalidCastException 오류 발생.
    public T GetValue<T>(string key)
    {
        if(data.TryGetValue(key, out object value))
        {
            // key에 해당하는 값이 존재한다면 T 타입으로 강제 캐스팅 후 반환.
            return (T)value;
        }
        return default(T);
    }

    // key를 통해 가져온 값이 T 타입인 경우 true를 반환.
    public bool TryGetValue<T>(string key, out T value)
    {
        if(data.TryGetValue(key, out object obj) && obj is T typedValue)
        {
            value = typedValue;
            return true;
        }
        value = default(T);
        return false;
    }

    // key에 해당하는 값이 Dictionary에 존재하는지 확인.
    public bool HasValue(string key)
    {
        return data.ContainsKey(key);
    }

    // key에 해당하는 값 삭제.
    public void RemoveValue(string key)
    {
        data.Remove(key);
    }

    // 초기화 메서드
    public void Initialize()
    {
        data.Clear();
    }
}