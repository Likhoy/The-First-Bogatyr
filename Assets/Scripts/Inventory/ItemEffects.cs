using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemEffects
{
    public static IEnumerator HealerEffectRoutine(float healingTimer, int healthBoostPerSecond)
    {
        Player player = GameManager.Instance.GetPlayer();

        while (healingTimer > 0f)
        {
            player.health.AddHealth(healthBoostPerSecond);
            Debug.Log("Heal " + healingTimer);
            healingTimer--;
            yield return new WaitForSeconds(1f);
        }
    }

    public static IEnumerator DamageEffectRoutine(float strengthTimer)
    {
        Player _player = GameManager.Instance.GetPlayer();
        while (strengthTimer > 0f)
        {
            Debug.Log("Damage " + strengthTimer);
            strengthTimer--;
            yield return new WaitForSeconds(1f);
        }
        (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMinDamage /= 2;
        (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMaxDamage /= 2;
    }
}
