using UnityEngine;
using UnityEngine.UI;

// 부모 계층의 UnitHealth 를 구독해 체력 게이지를 갱신한다. (Player / Enemy 공용)
public class HpBarController : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;

    private UnitHealth health;

    // UnitHealth.Initialize 가 구독자를 초기화하므로, 모든 Awake 가 끝난 뒤인 Start 에서 구독한다.
    private void Start()
    {
        health = GetComponentInParent<UnitHealth>();
        if (health == null) return;

        health.OnHpChanged += Refresh;
        Refresh(health.CurrentHp, health.MaxHp);
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnHpChanged -= Refresh;
    }

    private void Refresh(float currentHp, float maxHp)
    {
        gaugeImage.fillAmount = maxHp > 0f ? currentHp / maxHp : 0f;
    }
}
