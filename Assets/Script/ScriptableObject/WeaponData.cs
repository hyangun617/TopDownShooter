using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public string weaponName;

    [Header("Animation")]
    public AnimatorOverrideController upperBodyOverride;    // 없으면 기본 Handgun 애니메이션 사용.

    [Header("Model")]
    public GameObject weaponPrefab;
    public Vector3 gripPositionOffset;
    public Vector3 gripRotationOffset;
    public Vector3 gripScaleOffset;

    [Header("SFX")]
    public AudioClip fireSFX;
    public AudioClip reloadSFX;
    public AudioClip equipSFX;

    [Header("Stats")]
    public float damage;
    public float fireRate;
    public float range;
    public bool Piercing;
    public int magazineSize;
}
