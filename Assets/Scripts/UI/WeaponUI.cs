using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;

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
            if (setActiveWeaponEventArgs.isWeaponRanged)
            {
                weaponImage.sprite = (setActiveWeaponEventArgs.weapon as RangedWeapon).weaponDetails.weaponSprite;
            }
            else
            {
                weaponImage.sprite = (setActiveWeaponEventArgs.weapon as MeleeWeapon).weaponDetails.weaponSprite;
            }
        }
    }

}
