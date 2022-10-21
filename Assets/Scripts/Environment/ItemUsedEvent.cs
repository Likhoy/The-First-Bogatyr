using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ItemUsedEvent : MonoBehaviour
{
    public event Action<ItemUsedEvent, ItemUsedEventArgs> OnItemUsed;

    public void CallItemUsedEvent()
    {
        OnItemUsed?.Invoke(this, new ItemUsedEventArgs());
    }
}

public class ItemUsedEventArgs
{
    // TODO
}
