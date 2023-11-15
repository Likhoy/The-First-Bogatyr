using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDied, int exp)
    {
        OnDestroyed?.Invoke(
            this, 
            new DestroyedEventArgs()
            {
                playerDied = playerDied,
                experience = exp
            });
    }
}

public class DestroyedEventArgs : EventArgs
{
    public bool playerDied;
    public int experience; // here goes something given for killing enemy
}

