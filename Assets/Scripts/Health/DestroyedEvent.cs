using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDied, int points)
    {
        OnDestroyed?.Invoke(this, new DestroyedEventArgs() { playerDied = playerDied });
    }
}

public class DestroyedEventArgs : EventArgs
{
    public bool playerDied;
    // public int points; here goes something given for killing enemy
}

