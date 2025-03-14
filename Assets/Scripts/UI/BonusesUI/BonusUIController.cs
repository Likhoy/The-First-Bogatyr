using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusUIController : MonoBehaviour
{
    public GameObject bonusPanel;
    public Button[] bonusButtons;
    public GameObject bonusDescription;

    private List<BonusDetailsSO> randomBonuses;

    private void OnEnable()
    {
        StaticEventHandler.OnWaveFinished += ShowBonusPanel;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnWaveFinished -= ShowBonusPanel;
    }

    private void ShowBonusPanel(WaveFinishedEventArgs args)
    {   
        if(args.waveNumber != Settings.wavesAmount)
        {
            randomBonuses = BonusHandler.GetRandomBonusesForWave(args.waveDetails);

            if (randomBonuses == null)
                return;

            for (int i = 0; i < bonusButtons.Length; i++)
            {
                int index = i;
                bonusButtons[i].onClick.AddListener(() => ActivateBonus(index));
                bonusButtons[i].image.overrideSprite = randomBonuses[i].bonusSprite;
            }

            StartCoroutine(ShowBonusPanelWithDelay());
        }      
    }

    private IEnumerator ShowBonusPanelWithDelay()
    {     
        yield return new WaitForSecondsRealtime(1.5f);

        bonusPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ActivateBonus(int index)
    {
        BonusDetailsSO bonusDetails = randomBonuses[index];
        BonusHandler.ApplyBonus(bonusDetails);
        //Debug.Log("Activated bonus: " + bonusDetails.bonusName);

        bonusPanel.SetActive(false);
        bonusDescription.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OnButtonHover(int index)
    {
        bonusDescription.SetActive(true);

        BonusDetailsSO bonusDetails = randomBonuses[index];
        bonusDescription.GetComponentInChildren<TMP_Text>().text = bonusDetails.bonusName + "\n" + bonusDetails.bonusDescription;
    }

    public void OnButtonHoverExit()
    {
        bonusDescription.SetActive(false);
    }
}