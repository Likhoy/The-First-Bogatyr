using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HolyWater : Item
{
    private int healthBoostPerSecond;
    private float healingDuration;

    private Player player;
    private float healingTimer;

    override protected void Start()
    {
        base.Start();
        player = GameManager.Instance.GetPlayer();
        healthBoostPerSecond = 4;
        healingDuration = 5;
    }

    private void Update()
    {
        healingTimer -= Time.deltaTime;
    }

    private IEnumerator HealerEffectRoutine()
    {
        while (healingTimer >= 0f)
        {
            player.health.AddHealth(healthBoostPerSecond);
            yield return new WaitForSeconds(1f);
        }
    }

    override public void UseItem()
    {
        // AnimateDrinking();
        healingTimer = healingDuration;
        StopAllCoroutines();
        StartCoroutine(HealerEffectRoutine());
    }
}
