using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    public float damage;
    public float speed;
    public float range;
    public bool Piercing = false;
}
