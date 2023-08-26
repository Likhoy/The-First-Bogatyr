using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public Image weaponImage;
    //private SetActiveWeaponEvent weaponEvent;

    void Update()
    {
        ShowWeaponSprite();
    }

    private void ShowWeaponSprite()
    {
        if (GameManager.Instance.GetPlayer().activeWeapon.GetCurrentWeapon() != null)
        {
            weaponImage.overrideSprite = GameManager.Instance.GetPlayer().activeWeapon.GetCurrentWeapon().weaponDetails.weaponSprite;
        }
    }

}
