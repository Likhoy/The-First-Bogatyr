
using UnityEngine;

public class VirtualArmour : Protection
{
    public VirtualArmour()
    {
        effectPriority = 1;
    }

    public override void ApplyEffect(ref int damage)
    {
        if (totalEffectPercent > 0)
        {
            damage -= Mathf.RoundToInt((float)totalEffectPercent / 100 * damage);
        }
    }
}
