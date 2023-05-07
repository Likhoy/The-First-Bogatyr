using System;
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
        healingTimer = 0;
    }

    private void Update()
    {
        //healingTimer -= healingTimer >= 0 ? Time.deltaTime : 0;
    }

    override public void UseItem()
    {
        // AnimateDrinking();
        healingTimer += healingDuration;
        //StopAllCoroutines();
        StartCoroutine(ItemEffects.HealerEffectRoutine(healingDuration, healthBoostPerSecond));
    }
}
