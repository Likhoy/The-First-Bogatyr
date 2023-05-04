using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TemporaryObject : MonoBehaviour
{
    [SerializeField] private float lifeDuration;
    private SpriteRenderer spriteRenderer;

    private float alphaDecrease = 0.05f;
    private WaitForSeconds alphaDecreaseCooldown = new WaitForSeconds(0.1f);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadingOutRoutine());
    }

    private IEnumerator FadingOutRoutine()
    {
        yield return new WaitForSeconds(lifeDuration);

        while (spriteRenderer.color.a > 0f)
        {
            spriteRenderer.color = spriteRenderer.color.WithAlpha(spriteRenderer.color.a - alphaDecrease);
            yield return alphaDecreaseCooldown;
        }

        Destroy(gameObject);
    }
}
