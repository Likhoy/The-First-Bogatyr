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
}
