using UnityEngine;
using System;

[DisallowMultipleComponent]
public class MeleeAttackEvent : MonoBehaviour
{
    public event Action<MeleeAttackEvent, MeleeAttackEventArgs> OnMeleeAttack;

    public void CallMeleeAttackEvent(Vector3 targetPosition)
    {
        OnMeleeAttack?.Invoke(this, new MeleeAttackEventArgs() { targetPosition = targetPosition });
    }
}

public class MeleeAttackEventArgs : EventArgs
{
    public Vector3 targetPosition;
}

