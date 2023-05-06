using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StaticAttackingEndedEvent : MonoBehaviour
{
    public event Action<StaticAttackingEndedEvent> OnStaticAttackingEnded;

    public void CallStaticAttackingEndedEvent()
    {
        OnStaticAttackingEnded?.Invoke(this);
    }
}
