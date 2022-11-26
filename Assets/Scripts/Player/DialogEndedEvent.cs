using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class DialogEndedEvent : MonoBehaviour
{
    public event Action<DialogEndedEvent, DialogEndedEventArgs> OnEndDialog;

    public void CallDialogStartedEvent()
    {
        OnEndDialog?.Invoke(this, new DialogEndedEventArgs());
    }
}


public class DialogEndedEventArgs : EventArgs
{
    // TODO
}
