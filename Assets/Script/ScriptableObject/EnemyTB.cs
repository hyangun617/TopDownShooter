using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTB", menuName = "Scriptable Objects/EnemyTB")]
public class EnemyTB : ScriptableObject
{
    public List<EnemyData> enemyList = new List<EnemyData>();

    // 캐싱된 리스트
    private Dictionary<int, EnemyData> _table;

    public void Init()
    {
        // 리스트의 데이터를 Dictionary로 변환하여 캐싱
        _table = enemyList.ToDictionary(enemy => enemy.Id);
    }

    public EnemyData GetEnemyDataById(int id)
    {
        if(_table == null) Init();
        return _table.TryGetValue(id, out var data) ? data : null;
    }
}
