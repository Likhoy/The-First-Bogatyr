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
        GameManager.Instance.GetPlayer().weaponFiredEvent.OnWeaponFired += FiredIvent_OnWeaponFired;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().setActiveWeaponEvent.OnSetActiveWeapon -= WeaponEvent_OnWeaponChanged;
        GameManager.Instance.GetPlayer().weaponFiredEvent.OnWeaponFired -= FiredIvent_OnWeaponFired;
    }

    private void WeaponEvent_OnWeaponChanged(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        ShowWeaponSprite(setActiveWeaponEventArgs);
    }

    private void FiredIvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        ShowWeaponText(weaponFiredEventArgs);
    }

    private void ShowWeaponText(WeaponFiredEventArgs weaponFiredEventArgs)
    {
        if(weaponFiredEventArgs.weapon != null)
        {
            if(weaponFiredEventArgs.weapon is RangedWeapon)
            {
                countItems.text = "" + (weaponFiredEventArgs.weapon as RangedWeapon).weaponRemainingAmmo;
            }
            else
            {
                countItems.text = "";
            }
        }
        else
        {
            countItems.text = "";
        }
        
    }

    private void ShowWeaponSprite(SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (setActiveWeaponEventArgs.weapon != null)
        {
            weaponImage.gameObject.SetActive(true);
          
            if (setActiveWeaponEventArgs.isWeaponRanged)
            {
                weaponImage.sprite = (setActiveWeaponEventArgs.weapon as RangedWeapon).weaponDetails.weaponSprite;
                countItems.text = "" + (setActiveWeaponEventArgs.weapon as RangedWeapon).weaponRemainingAmmo;
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
