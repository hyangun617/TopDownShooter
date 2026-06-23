using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTB", menuName = "Scriptable Objects/EnemyTB")]
public class EnemyTB : ScriptableObject
{
    public List<EnemyData> enemyList = new List<EnemyData>();
}
