using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveInfoUI : MonoBehaviour
{
    public Image waveInfoPanelImage;
    public TextMeshProUGUI waveInfoText;
    public float fadeDuration = 2.0f;

    private void OnEnable()
    {
        StaticEventHandler.OnWaveLaunched += ShowInfoPanel;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnWaveLaunched -= ShowInfoPanel;
    }

    private void ShowInfoPanel(WaveLaunchedEventArgs args)
    {      
        waveInfoText.text = "Волна номер " + args.waveNumber.ToString();

        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        waveInfoPanelImage.gameObject.SetActive(true);

        SetAlpha(0f);

        float timer = 0f;
        yield return new WaitForSeconds(3f);

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
           
        }

        waveInfoPanelImage.gameObject.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        Color imageColor = waveInfoPanelImage.color;
        imageColor.a = alpha;
        waveInfoPanelImage.color = imageColor;

        Color textColor = waveInfoText.color;
        textColor.a = alpha;
        waveInfoText.color = textColor;
    }

}
