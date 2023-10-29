using UnityEngine;

[RequireComponent(typeof(Health))]

[DisallowMultipleComponent]
public class DamageReflector : Protection
{
    public override void ApplyEffect(ref int damage)
    {
        if (totalEffectPercent > 0)
        {
            int value = Random.Range(1, 101);
            if (value <= totalEffectPercent)
                damage = 0;
        }
    }
}
