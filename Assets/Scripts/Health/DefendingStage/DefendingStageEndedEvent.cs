using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DefendingStageEndedEvent : MonoBehaviour
{

    public event Action<DefendingStageEndedEvent, DefendingStageEndedEventArgs> OnDefendingStageEnded;

    public void CallDefendingStageEndedEvent()
    {
        OnDefendingStageEnded?.Invoke(this, new DefendingStageEndedEventArgs());
    }

}

public class DefendingStageEndedEventArgs : EventArgs { }
