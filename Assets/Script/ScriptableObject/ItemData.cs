using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    Equipment,
    Consumable
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Common")]
    [SerializeField] private int id;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;  // 고정 문구만, 수치는 UI에서 자동 생성
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject dropVisual;         // 드랍된 아이템의 외형
    [SerializeField] private ItemType itemType;
    [SerializeField] private int maxStack = 1;

    [Header("Type Specific")]
    public WeaponData weapon;               // itemType == Equipment 일 때만 유효
    [SerializeReference, SubclassSelector] public List<EffectBase> EffectList;     // itemType == Consumable 일 때만 유효

    public int Id => id;
    public string Name => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public GameObject DropVisual => dropVisual;
    public ItemType ItemType => itemType;
    public int MaxStack => maxStack;


#if UNITY_EDITOR
    void OnValidate()
    {
        // 타입에 맞지 않는 데이터 정리
        if(itemType == ItemType.Equipment) maxStack = 1;
    }
#endif
}
