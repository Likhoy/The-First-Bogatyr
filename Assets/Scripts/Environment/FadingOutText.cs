using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadingOutText : MonoBehaviour
{
    TextMeshProUGUI text;
    float alphaDecrease = 0.05f;
    WaitForSeconds alphaDecreaseCooldown = new WaitForSeconds(0.1f);

    private void OnEnable()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadingOutRoutine());
    }

    private IEnumerator FadingOutRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (text.alpha > 0f)
        {
            text.alpha -= alphaDecrease;
            yield return alphaDecreaseCooldown;
        }
        gameObject.SetActive(false);
        text.alpha = 1f;
    }
}
