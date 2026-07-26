using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private PlayerAnimController playerAnimController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Transform weaponSocket;

    public WeaponData WeaponData => currentWeapon;

    private GameObject currentWeaponInstance;

    void Start()
    {
        if(playerAnimController == null)
        {
            playerAnimController = GetComponent<PlayerAnimController>();
        }
        if(playerAttack == null)
        {
            playerAttack = GetComponent<PlayerAttack>();
        }

        if(currentWeapon != null)
        {
            EquipWeapon(currentWeapon);
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        // 애니메이션 교체
        playerAnimController.EquipWeaponAnimation(newWeapon);

        playerAttack.SetWeaponData(newWeapon);

        // 기존 모델 제거
        if(currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }

        // 새 모델 장착
        if(newWeapon.weaponPrefab != null)
        {
            currentWeaponInstance = Instantiate(newWeapon.weaponPrefab, weaponSocket);
            currentWeaponInstance.transform.localPosition = newWeapon.gripPositionOffset;
            currentWeaponInstance.transform.localRotation = Quaternion.Euler(newWeapon.gripRotationOffset);
            currentWeaponInstance.transform.localScale = newWeapon.gripScaleOffset;        
            
            WeaponFirePoint marker = currentWeaponInstance.GetComponentInChildren<WeaponFirePoint>();
            if(marker != null)
            {
                playerAttack.SetFirePoint(marker.transform);
            }
        }

        // 장착 SFX
        if(newWeapon.equipSFX != null)
        {
            GameManager.Instance.SoundMgr.PlaySfx(newWeapon.equipSFX, followTarget: this.transform);
        }

        currentWeapon = newWeapon;
    }
}
