using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    //private SetActiveWeaponEvent weaponEvent; // он в классе Player

    void Update()
    {
        ShowWeaponSprite();
    }

    private void ShowWeaponSprite()
    {
        if (GameManager.Instance.GetPlayer().activeWeapon.GetCurrentWeapon() != null)
        {
            Weapon weapon = GameManager.Instance.GetPlayer().activeWeapon.GetCurrentWeapon();
            Debug.Log(weapon.weaponDetails == null);
            Debug.Log(weapon.weaponListPosition);
            weaponImage.sprite = GameManager.Instance.GetPlayer().activeWeapon.GetCurrentWeapon().weaponDetails.weaponSprite;
        }
    }

}
