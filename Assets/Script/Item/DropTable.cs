using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DropItemPercent
{
    public ItemData itemData;
    [Range(0f, 1f)] public float percent;
}

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable Objects/DropTable")]
public class DropTable : ScriptableObject
{
    public List<DropItemPercent> droptable = new();

    public ItemData Drop()
    {
        float randTemp = Random.value;
        float cumulative = 0f;

        foreach(var item in droptable)
        {
            cumulative += item.percent;

            // 지정된 확률 이하의 값이라면 아이템 코드 반환.
            if(randTemp < cumulative) return item.itemData;
        }

        return null;
    }
}
