using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(NPC))]
[DisallowMultipleComponent]
public class AnimateNPC : MonoBehaviour
{
    private NPC npc;

    private void Awake()
    {
        // Load components
        npc = GetComponent<NPC>();
    }

    private void OnEnable()
    {
        

    }

    private void OnDisable()
    {
    }

    /// <summary>
    /// On trading stage launched event handler
    /// </summary>
    private void TradingStageLaunchedEvent_OnLaunchTradingStage(TradingStageLaunchedEvent tradingStageLaunchedEvent, TradingStageLaunchedEventArgs tradingStageLaunchedEventArgs)
    {
       
    }

}
