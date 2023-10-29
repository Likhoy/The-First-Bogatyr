using UnityEngine;

[RequireComponent(typeof(Health))]

[DisallowMultipleComponent]
public class Armour : Protection
{
    public override void ApplyEffect(ref int damage)
    {
        if (totalEffectPercent > 0)
        {
            damage -= Mathf.RoundToInt((float)totalEffectPercent / 100 * damage);
        }
    }

}
