using UnityEngine;
using System;

[DisallowMultipleComponent]
public class MoneyIncreasedEvent : MonoBehaviour
{
    public event Action<MoneyIncreasedEvent, MoneyIncreasedEventArgs> OnMoneyIncreased;

    public void CallMoneyIncreasedEvent(int playerMoney)
    {
        OnMoneyIncreased?.Invoke(this, new MoneyIncreasedEventArgs() { playerMoney = playerMoney });
    }
}


public class MoneyIncreasedEventArgs : EventArgs
{
    public int playerMoney;
}
