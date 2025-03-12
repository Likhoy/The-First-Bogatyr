using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ExperienceUI : MonoBehaviour
{
    [SerializeField] private Image expBar;
    [SerializeField] private TMP_Text expText;

    public GameObject player;

    void Start()
    {
        if (!player)
        {
            player = GameObject.FindWithTag("Player");
        }
    }


    void Update()
    {
        if (!player)
        {
            Destroy(gameObject);
            return;
        }
        Status stat = player.GetComponent<Status>();

        float maxExp = stat.maxExp;
        int exp = stat.exp;
        float curExp = exp / maxExp;

        if (curExp > expBar.fillAmount)
        {
            expBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
            if (expBar.fillAmount > curExp)
            {
                expBar.fillAmount = curExp;
            }
        }
        if (curExp < expBar.fillAmount)
        {
            expBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
            if (expBar.fillAmount < curExp)
            {
                expBar.fillAmount = curExp;
            }
        }

        if (expText)
        {
            expText.text = exp.ToString() + "/" + maxExp.ToString();
        }
    }
    //private void OnEnable()
    //{
    //    // GameManager.Instance.GetPlayer().playerResources.experienceIncreasedEvent.OnExperienceIncreased += ExperienceEvent_OnExperienceChanged;
    //}

    //private void OnDisable()
    //{
    //    // GameManager.Instance.GetPlayer().playerResources.experienceIncreasedEvent.OnExperienceIncreased -= ExperienceEvent_OnExperienceChanged;
    //}

    //private void ExperienceEvent_OnExperienceChanged(ExperienceIncreasedEvent experienceEvent, ExperienceIncreasedEventArgs experienceEventArgs)
    //{
    //    SetExperienceUI(experienceEventArgs);
    //}

    //private void SetExperienceUI(ExperienceIncreasedEventArgs experienceEventArgs)
    //{
    //    experienceBar.fillAmount = (experienceEventArgs.playerExperience % 1000)/ 1000f;
    //    experienceText.text = (experienceEventArgs.playerExperience).ToString() + "/" + ((experienceEventArgs.playerExperience / 1000 + 1) * 1000).ToString();
    //}
}
