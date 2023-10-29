using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ExperienceUI : MonoBehaviour
{
    [SerializeField] private Image experienceBar;

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().playerResources.experienceIncreasedEvent.OnExperienceIncreased += ExperienceEvent_OnExperienceChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().playerResources.experienceIncreasedEvent.OnExperienceIncreased -= ExperienceEvent_OnExperienceChanged;
    }

    private void ExperienceEvent_OnExperienceChanged(ExperienceIncreasedEvent experienceEvent, ExperienceIncreasedEventArgs experienceEventArgs)
    {
        SetExperienceUI(experienceEventArgs);
    }

    private void SetExperienceUI(ExperienceIncreasedEventArgs experienceEventArgs)
    {
           experienceBar.fillAmount = (experienceEventArgs.playerExperience % 1000)/ 1000f;
    }
}
