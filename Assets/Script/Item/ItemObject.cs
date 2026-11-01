using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Transform visualRoot;


    void OnTriggerEnter(Collider other)
    {
        // 충돌체가 플레이어인 경우.
        if(other.CompareTag("Player"))
        {
            Debug.Log($"[ItemObject] {gameObject.name} 아이템 충돌 및 적용.");

            // 소비 아이템인 경우.
            if(itemData.ItemType == ItemType.Consumable)
            {
                foreach(var effect in itemData.EffectList)
                {
                    effect.Apply(other.gameObject);
                }
            }

            Destroy(gameObject);
        }
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;

        InitializeItem();
    }

    private void InitializeItem()
    {
        if(itemData == null)
        {
            Debug.LogWarning($"[ItemObject] {gameObject.name}에 itemData(이)가 존재하지 않습니다.");
            return;
        }
        else if(itemData.DropVisual == null)
        {
            Debug.LogWarning($"[ItemObject] {gameObject.name}에 아이템 외형 정보(DropVisual)(이)가 존재하지 않습니다.");
            return;
        }

        gameObject.name = itemData.name + "_Object";

        // 기존 외형 제거
        for(int i = visualRoot.childCount - 1; i >= 0; --i)
        {
            Destroy(visualRoot.GetChild(i).gameObject);
        }

        var visual = Instantiate(itemData.DropVisual, visualRoot);
        FitColliderToVisual(visual);
    }

    // 충돌체 크기를 외형에 맞추는 메서드
    private void FitColliderToVisual(GameObject visual)
    {
        var renderers = visual.GetComponentsInChildren<Renderer>();
        if(renderers.Length == 0) return;

        // 모든 Renderer의 월드 범위를 합침.
        Bounds bounds = renderers[0].bounds;
        for(int i = 1; i < renderers.Length; ++i)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // 월드 기준 범위 -> ItemObject 로컬 기준으로 변환
        var col = GetComponent<BoxCollider>();
        col.center = transform.InverseTransformPoint(bounds.center);
        Vector3 scale = transform.lossyScale;
        col.size = new Vector3(bounds.size.x / scale.x, bounds.size.y / scale.y, bounds.size.z / scale.z);
        col.size += Vector3.one * 0.3f;
    }
}
