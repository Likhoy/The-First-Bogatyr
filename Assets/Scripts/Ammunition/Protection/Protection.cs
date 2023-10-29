
using PixelCrushers;
using System;

public abstract class Protection : IRollBackable, IComparable<Protection>
{
    protected Health parentHealthReference;
    protected int effectPriority = 0; // lower number - higher priority

    protected int totalEffectPercent;

    protected BonusLevel protectionLevel;

    public abstract void ApplyEffect(ref int damage);

    public static void AddProtection<T>(Health characterHealth, PowerBonusDetailsSO bonusDetails) where T : Protection, new()
    {
        Protection protection = characterHealth.currentProtections.Find(protection => protection.GetType() == typeof(T));
        if (protection != null)
        {
            if (bonusDetails.bonusLevel > protection.protectionLevel)
            {
                int effectRise = (protection.protectionLevel - bonusDetails.bonusLevel) * bonusDetails.levelRaisePercent;

                protection.totalEffectPercent += effectRise;
                protection.protectionLevel = bonusDetails.bonusLevel;
            }
        }
        else
        {
            T newProtection = new T();
            newProtection.totalEffectPercent = bonusDetails.bonusPercent + ((int)bonusDetails.bonusLevel - 1) * bonusDetails.levelRaisePercent;
            newProtection.protectionLevel = bonusDetails.bonusLevel;
            characterHealth.currentProtections.AddSorted(newProtection);
            newProtection.parentHealthReference = characterHealth;
        }
    }

    /*public static void DecreaseEffect<T>(Health characterHealth, int percentDecreased) where T : Protection
    {
        T protectionComponent = characterHealth.GetComponent<T>();
        if (protectionComponent != null)
        {
            protectionComponent.totalEffectPercent -= percentDecreased;
            if (protectionComponent.totalEffectPercent <= 0)
            {
                protectionComponent.totalEffectPercent = 0;
                characterHealth.currentProtections.Remove(protectionComponent);
            }
        }
    }*/

    public void Rollback()
    {
        parentHealthReference.currentProtections.Remove(this);
    }

    public int CompareTo(Protection other)
    {
        return effectPriority.CompareTo(other.effectPriority);
    }
}
