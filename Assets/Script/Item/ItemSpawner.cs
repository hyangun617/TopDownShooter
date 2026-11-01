using System;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemObject itemPrefab;

    void OnEnable() => Enemy.OnAnyEnemyDeath += OnEnemyDeath;
    void OnDisable() => Enemy.OnAnyEnemyDeath -= OnEnemyDeath;

    private void OnEnemyDeath(Enemy enemy)
    {
        if(enemy.DropTable == null) return;

        ItemData data = enemy.DropTable.Drop();
        if(data == null) return;

        Spawn(data, enemy.transform.position);
    }

    private ItemObject Spawn(ItemData data, Vector3 position)
    {
        ItemObject item = Instantiate(itemPrefab, position, Quaternion.identity);
        item.SetItemData(data);
        return item;
    }
}
