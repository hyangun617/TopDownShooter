using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class HUDController : UIView
{
    public override bool IsModal => false;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreBoard;

    [Header("Timer")]
    [SerializeField] private TMP_Text timerBoard;

    [Header("Weapon HUD")]
    [SerializeField] private TMP_Text currentAmmo;
    [SerializeField] private TMP_Text maxAmmo;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private GameObject reloadPanel;
    [SerializeField] private Image reloadGauge;

    [Header("Button")]
    [SerializeField] private Button PauseButton;


    private float addScoreTime = 1f;

    void Update()
    {
        SetTimer();
        SetScoreBoard();
        
        addScoreTime -= Time.deltaTime;
        if(addScoreTime <= 0f)
        {
            GameManager.Instance.SetScore(GameManager.Instance.Score + 1);
            addScoreTime = 1f;
        }
    }

    void OnEnable()
    {
        PauseButton.onClick.AddListener(OnPauseButton);
    }

    void OnDisable()
    {
        PauseButton.onClick.RemoveListener(OnPauseButton);
    }

    #region Weapon HUD
    private PlayerAttack playerAttack;

    public void Bind(PlayerAttack attacker)
    {
        if (attacker == null)
            return;
        
        Unbind();

        playerAttack = attacker;
        playerAttack.OnAmmoChanged += ChangeCurrentAmmo;
        playerAttack.OnReloadStart += HandleReloadStart;
        playerAttack.OnReloadProgress += HandleReloadProgress;
        playerAttack.OnReloadComplete += ChangeCurrentAmmo;

        ChangeCurrentAmmo(playerAttack.CurrentAmmo);
        UpdateMaxAmmo(playerAttack.MaxAmmo);
    }

    public void Unbind()
    {
        if (playerAttack == null)
            return;

        playerAttack.OnAmmoChanged -= ChangeCurrentAmmo;
        playerAttack.OnReloadStart -= HandleReloadStart;
        playerAttack.OnReloadProgress -= HandleReloadProgress;
        playerAttack.OnReloadComplete -= ChangeCurrentAmmo;

        playerAttack = null;
    }

    public void UpdateMaxAmmo(int value)
    {
        if (maxAmmo != null)
            maxAmmo.text = value.ToString();
    }

    public void HandleReloadStart()
    {
        if (reloadPanel != null)
            reloadPanel.SetActive(true);

        if (reloadGauge != null)
            reloadGauge.fillAmount = 0f;
    }

    public void HandleReloadProgress(float progress)
    {
        if (reloadGauge != null)
            reloadGauge.fillAmount = progress;
    }

    public void ChangeCurrentAmmo(int value)
    {
        if (currentAmmo != null)
            currentAmmo.text = value.ToString();

        if (reloadPanel != null)
            reloadPanel.SetActive(false);
    }

    public void SetWeaponIcon(Sprite sprite)
    {
        if (weaponIcon != null)
            weaponIcon.sprite = sprite;
    }
#endregion

#region Timer

    private float currTime = 0f;
    private int min;
    private int sec;
    private int mili;

    private void SetTimer()
    {
        currTime += Time.deltaTime;

        min = Mathf.FloorToInt(currTime / 60f);
        sec = Mathf.FloorToInt(currTime % 60f);
        mili = Mathf.RoundToInt(currTime % 1f * 100f) % 100;

        timerBoard.SetText("{0:00}:{1:00}:{2:00}", min, sec, mili);
    }

#endregion

#region Score

    private void SetScoreBoard()
    {
        scoreBoard.SetText(GameManager.Instance.Score.ToString());
    }

#endregion

#region Button

    private void OnPauseButton()
    {
        UIManager.Instance.OpenAsync<MenuView>().Forget(ex => Debug.LogException(ex));
    }

#endregion





}
