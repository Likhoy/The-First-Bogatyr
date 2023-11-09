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

    private void ShowBonusPanel(WaveFinishedEventArgs obj)
    {
        randomBonuses = BonusHandler.GetRandomBonusesForWave(obj.waveNumber);

        for (int i = 0; i < bonusButtons.Length; i++)
        {
            int index = i;
            bonusButtons[i].onClick.AddListener(() => ActivateBonus(index));
            bonusButtons[i].image.overrideSprite = randomBonuses[i].bonusSprite;
        }

        bonusPanel.SetActive(true);
    }


    private void ActivateBonus(int index)
    {
        BonusDetailsSO bonusDetails = randomBonuses[index];
        BonusHandler.ApplyBonus(bonusDetails);
        Debug.Log("Activated bonus: " + bonusDetails.bonusName);

        bonusPanel.SetActive(false);
    }

    public void OnButtonHover(int index)
    {
        bonusDescription.SetActive(true);

        BonusDetailsSO bonusDetails = randomBonuses[index];
        bonusDescription.GetComponentInChildren<TMP_Text>().text = bonusDetails.bonusName;
    }

    public void OnButtonHoverExit()
    {
        bonusDescription.SetActive(false);
    }
}