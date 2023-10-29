using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ExperienceIncreasedEvent : MonoBehaviour
{
    public event Action<ExperienceIncreasedEvent, ExperienceIncreasedEventArgs> OnExperienceIncreased;

    public void CallExperienceIncreasedEvent(int playerExp)
    {
        OnExperienceIncreased?.Invoke(this, new ExperienceIncreasedEventArgs() { playerExperience = playerExp });
    }
}


public class ExperienceIncreasedEventArgs : EventArgs
{
    public int playerExperience;
}
