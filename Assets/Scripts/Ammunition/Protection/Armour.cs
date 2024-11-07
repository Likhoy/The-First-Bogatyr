using UnityEngine;

public class Armour : Protection
{

    public Armour()
    {
        effectPriority = 1;
    }

    public override void ApplyEffect(ref int damage)
    {
        if (totalEffectPercent > 0)
        {
            durability -= damage;
            damage -= Mathf.RoundToInt((float)totalEffectPercent / 100 * damage);
        }
        if (durability <= 0)
        {
            parentHealthReference.ProtectionsToDelete.Add(this);
        }
    }
}
