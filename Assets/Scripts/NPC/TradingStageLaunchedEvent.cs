using UnityEngine;
using System;

[DisallowMultipleComponent]
public class TradingStageLaunchedEvent : MonoBehaviour
{
    public event Action<TradingStageLaunchedEvent, TradingStageLaunchedEventArgs> OnLaunchTradingStage;

    public void CallTradingStageLaunchedEvent()
    {
        OnLaunchTradingStage?.Invoke(this, new TradingStageLaunchedEventArgs()); 
    }
}


public class TradingStageLaunchedEventArgs : EventArgs
{
    // TODO
}
