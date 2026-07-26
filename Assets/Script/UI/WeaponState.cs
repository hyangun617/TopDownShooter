using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponState : MonoBehaviour
{
    [SerializeField] private TMP_Text currentAmmo;
    [SerializeField] private TMP_Text hasAmmo;
    [SerializeField] private Image image;
    [SerializeField] private GameObject ReloadObj;
    [SerializeField] private Image ReloadGauge;

    public void changeHasAmmo(int value)
    {
        hasAmmo.text = value.ToString();
    }

    public void HandleReloadStart()
    {
        ReloadObj.gameObject.SetActive(true);
        ReloadGauge.fillAmount = 0f;
    }

    public void HandleReloadProgress(float progress)
    {
        ReloadGauge.fillAmount = progress;
    }

    public void changeCurrentAmmo(int value)
    {
        currentAmmo.text = value.ToString();
        ReloadObj.gameObject.SetActive(false);
    }

    public void changeImage(Image image)
    {
        this.image = image;
    }
}
