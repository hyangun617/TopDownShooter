using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private TimeBoard timeBoard;
    [SerializeField] private WeaponState weaponState;

    // 구독 및 해제용 필드
    private PlayerAttack boundPlayerAttack;

    public void ReloadBind(PlayerAttack playerAttack)
    {
        boundPlayerAttack = playerAttack;

        boundPlayerAttack.OnReloadStart += weaponState.HandleReloadStart;
        boundPlayerAttack.OnReloadProgress += weaponState.HandleReloadProgress;
        boundPlayerAttack.OnReloadComplete += weaponState.changeCurrentAmmo;

        weaponState.changeCurrentAmmo(boundPlayerAttack.CurrentAmmo);
    }

    private void OnDisable()
    {
        if (boundPlayerAttack != null && weaponState != null)
        {
            boundPlayerAttack.OnReloadStart -= weaponState.HandleReloadStart;
            boundPlayerAttack.OnReloadProgress -= weaponState.HandleReloadProgress;
            boundPlayerAttack.OnReloadComplete -= weaponState.changeCurrentAmmo;
        }
    }
}
