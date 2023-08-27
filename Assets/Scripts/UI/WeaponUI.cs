using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TMP_Text countItems;

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().setActiveWeaponEvent.OnSetActiveWeapon += WeaponEvent_OnWeaponChanged;         
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().setActiveWeaponEvent.OnSetActiveWeapon -= WeaponEvent_OnWeaponChanged;
    }

    private void WeaponEvent_OnWeaponChanged(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        ShowWeaponSprite(setActiveWeaponEventArgs);
    }

    private void ShowWeaponSprite(SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (setActiveWeaponEventArgs.weapon != null)
        {
            weaponImage.gameObject.SetActive(true);
          
            if (setActiveWeaponEventArgs.isWeaponRanged)
            {
                weaponImage.sprite = (setActiveWeaponEventArgs.weapon as RangedWeapon).weaponDetails.weaponSprite;
                countItems.text = "" + (setActiveWeaponEventArgs.weapon as RangedWeapon).weaponClipRemainingAmmo;
            }
            else
            {
                weaponImage.sprite = (setActiveWeaponEventArgs.weapon as MeleeWeapon).weaponDetails.weaponSprite;
                countItems.text = "";
            }
        }
        else
        {
            weaponImage.gameObject.SetActive(false);
        }
    }

}
