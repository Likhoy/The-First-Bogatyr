using System;
using UnityEngine;

[DisallowMultipleComponent]
public class LookAtEvent : MonoBehaviour
{
    public event Action<LookAtEvent, StaticAttackingStartedEventArgs> OnLookAt;

    public void CallLookAtEvent(Vector3 playerPosition)
    {
        OnLookAt?.Invoke(this, new StaticAttackingStartedEventArgs { playerPosition = playerPosition });
    }
}

public class StaticAttackingStartedEventArgs : EventArgs
{
    public Vector3 playerPosition;
}
