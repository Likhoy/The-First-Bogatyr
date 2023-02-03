using UnityEngine;
using System;

[DisallowMultipleComponent]
public class MeleeAttackEvent : MonoBehaviour
{
    public event Action<MeleeAttackEvent, MeleeAttackEventArgs> OnMeleeAttack;

    public void CallMeleeAttackEvent()
    {
        OnMeleeAttack?.Invoke(this, new MeleeAttackEventArgs());
    }
}

public class MeleeAttackEventArgs : EventArgs
{
    // TODO
}

