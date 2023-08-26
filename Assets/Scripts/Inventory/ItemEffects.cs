using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ItemEffects
{
    private static int healerEffectCount = 0;
    private static int damageEffectCount = 0;

    public static IEnumerator HealerEffectRoutine(float healingTimer, int healthBoostPerSecond, Image effectImage, Sprite effectSprite)
    {
        healerEffectCount++;
        effectImage.sprite = effectSprite;
        effectImage.color = Color.white;

        Player player = GameManager.Instance.GetPlayer();
        if (player == null)
            yield break;

        while (healingTimer > 0f)
        {
            player.health.AddHealth(healthBoostPerSecond);
            healingTimer--;
            yield return new WaitForSeconds(1f);
        }

        if (healerEffectCount == 1)
            effectImage.color = new Color(0, 0, 0, 0);
        healerEffectCount--;
    }

    public static IEnumerator DamageEffectRoutine(float strengthTimer, Image effectImage, Sprite effectSprite)
    {
        damageEffectCount++;

        effectImage.sprite = effectSprite;
        effectImage.color = Color.white;

        Player player = GameManager.Instance.GetPlayer();
        if (player == null)
            yield break;

        if (damageEffectCount == 1)
        {
            (player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMinDamage *= 2;
            (player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMaxDamage *= 2;
        }

        while (strengthTimer > 0f)
        {
            strengthTimer--;
            yield return new WaitForSeconds(1f);
        }
        if (damageEffectCount == 1)
        {
            (player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMinDamage /= 2;
            (player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMaxDamage /= 2;
        }
        if (damageEffectCount == 1)
            effectImage.color = new Color(0, 0, 0, 0);
        damageEffectCount--;
    }
}
