using UnityEngine;
using System;

[DisallowMultipleComponent]
public class DialogStartedEvent : MonoBehaviour
{
    public event Action<DialogStartedEvent, DialogStartedEventArgs> OnStartDialog;

    public void CallDialogStartedEvent()
    {
        OnStartDialog?.Invoke(this, new DialogStartedEventArgs());
    }
}


public class DialogStartedEventArgs : EventArgs
{
    // TODO
}
