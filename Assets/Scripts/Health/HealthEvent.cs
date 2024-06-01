using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthEvent : MonoBehaviour
{
    public event Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthChangedEvent(float healthPercent, int healthAmount,int maxHealthAmount, int damageAmount)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs() { healthPercent = healthPercent, healthAmount = healthAmount, maxHealthAmount = maxHealthAmount, damageAmount = damageAmount });
    }
}

public class HealthEventArgs : EventArgs
{
    public float healthPercent;
    public int healthAmount;
    public int maxHealthAmount;
    public int damageAmount;
}
