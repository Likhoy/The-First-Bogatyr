using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ItemEffects
{
    public static IEnumerator HealerEffectRoutine(float healingTimer, int healthBoostPerSecond, Image effectImage, Sprite effectSprite)
    {
        effectImage.sprite = effectSprite;
        effectImage.color = Color.white;
        Player player = GameManager.Instance.GetPlayer();

        while (healingTimer > 0f)
        {
            player.health.AddHealth(healthBoostPerSecond);
            healingTimer--;
            yield return new WaitForSeconds(1f);
        }
        effectImage.color = new Color(0, 0, 0, 0);
    }

    public static IEnumerator DamageEffectRoutine(float strengthTimer, Image effectImage, Sprite effectSprite)
    {
        Player _player = GameManager.Instance.GetPlayer();

        effectImage.sprite = effectSprite;
        effectImage.color = Color.white;

        (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMinDamage *= 2;
        (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMaxDamage *= 2;

        while (strengthTimer > 0f)
        {
            strengthTimer--;
            yield return new WaitForSeconds(1f);
        }
        (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMinDamage /= 2;
        (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMaxDamage /= 2;

        effectImage.color = new Color(0, 0, 0, 0);
    }
}
