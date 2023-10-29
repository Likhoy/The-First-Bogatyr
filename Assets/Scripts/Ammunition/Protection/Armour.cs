using UnityEngine;

public class Armour : Protection
{
    private int durability;

    public Armour()
    {
        effectPriority = 1;
        durability = parentHealthReference.GetInitialHealth() * (int)protectionLevel; // not very good
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
            Rollback(); // delete armour
        }
    }
}
