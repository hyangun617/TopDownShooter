using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;
    public float currentFill = 1.0f;
    public float speed = 5.0f;

    private Player target;
    private float maxHp;
    [SerializeField] private float currentHp;

    private void Awake()
    {
        // 플레이어가 소환되면 이벤트를 연결하는 메서드 연결.
        PlayerManager.GetPlayerObjAfterSpawned += LinkedEvent;
    }

    private void OnDisable()
    {
        target.onTakeDamage -= ChangeHpBar;
    }

    private void ChangeHpBar(float value)
    {
        currentHp -= value;
        gaugeImage.fillAmount = currentHp / maxHp;
    }

    // 플레이어가 스폰되면 이벤트를 연결함.
    private void LinkedEvent(GameObject player)
    {
        if(player.TryGetComponent<Player>(out var value))
        {
            target = value;
            target.onTakeDamage += ChangeHpBar;
            maxHp = target.MaxHp;
            currentHp = target.MaxHp;
        }

        PlayerManager.GetPlayerObjAfterSpawned -= LinkedEvent;
    }
}
