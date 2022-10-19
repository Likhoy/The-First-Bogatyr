using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class DialogProceededEvent : MonoBehaviour
{
    public event Action<DialogProceededEvent, DialogProceededEventArgs> OnProceedDialog;

    public void CallDialogProceedEvent()
    {
        OnProceedDialog?.Invoke(this, new DialogProceededEventArgs());
    }
}


public class DialogProceededEventArgs : EventArgs
{
    // TODO
}

