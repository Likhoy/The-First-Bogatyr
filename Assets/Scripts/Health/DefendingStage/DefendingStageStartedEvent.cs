using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DefendingStageStartedEvent : MonoBehaviour
{
    
    public event Action<DefendingStageStartedEvent, DefendingStageStartedEventArgs> OnDefendingStageStarted;

    public void CallDefendingStageStartedEvent()
    {
        OnDefendingStageStarted?.Invoke(this, new DefendingStageStartedEventArgs());
    }

}

public class DefendingStageStartedEventArgs : EventArgs { }
