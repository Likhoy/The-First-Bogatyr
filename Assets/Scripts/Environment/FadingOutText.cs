using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FadingOutText : MonoBehaviour
{
    public string TextToShow { get; set; } = "";
    private TextMeshProUGUI TMPro;
    private float alphaDecrease = 0.05f;
    private WaitForSeconds alphaDecreaseCooldown = new WaitForSeconds(0.1f);

    private void Awake()
    {
        TMPro = GetComponent<TextMeshProUGUI>();
    }

    public void ShowHint(float delayDuration)
    {
        StartCoroutine(FadingOutRoutine(delayDuration));
    }


    private IEnumerator FadingOutRoutine(float delayDuration)
    {
        string textToShow = TextToShow;
        if (delayDuration > 0)
            yield return new WaitForSeconds(delayDuration);
        TMPro.text = textToShow;
        yield return new WaitForSeconds(3f);

        while (TMPro.alpha > 0f)
        {
            TMPro.alpha -= alphaDecrease;
            yield return alphaDecreaseCooldown;
        }
        TMPro.alpha = 1f;
        TMPro.text = "";
    }
}
