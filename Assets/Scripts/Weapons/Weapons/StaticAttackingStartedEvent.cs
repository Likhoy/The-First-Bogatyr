using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StaticAttackingStartedEvent : MonoBehaviour
{
    public event Action<StaticAttackingStartedEvent> OnStaticAttackingStarted;

    public void CallStaticAttackingStartedEvent()
    {
        OnStaticAttackingStarted?.Invoke(this);
    }
}
