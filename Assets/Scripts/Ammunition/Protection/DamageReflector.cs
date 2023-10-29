using UnityEngine;

public class DamageReflector : Protection
{

    public DamageReflector()
    {
        effectPriority = 0;
    }

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
