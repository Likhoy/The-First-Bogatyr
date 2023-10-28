
using Unity.VisualScripting;
using UnityEngine;

public abstract class Protection : MonoBehaviour
{
    protected int totalEffectPercent;

    public abstract void ApplyEffect(ref int damage);

    public static void IncreaseEffect<T>(Health characterHealth, int percentAdded) where T : Protection
    {
        T protectionComponent = characterHealth.GetComponent<T>();
        if (protectionComponent != null)
        {
            if (!characterHealth.currentProtections.Contains(protectionComponent))
                characterHealth.currentProtections.Add(protectionComponent);
            HelperUtilities.AddValueToPercentage(ref protectionComponent.totalEffectPercent, percentAdded);
        }
        else
        {
            characterHealth.AddComponent<T>();
            characterHealth.currentProtections.Add(protectionComponent);
        }
    }

    public static void DecreaseEffect<T>(Health characterHealth, int percentDecreased) where T : Protection
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
    }

}
