using System.Collections;
using UnityEngine;

public class TemporaryObject : MonoBehaviour
{
    [SerializeField] private bool fadeOutOnAwake = true;
    [SerializeField] private float lifeDuration;
    private SpriteRenderer spriteRenderer;

    private float alphaDecrease = 0.05f;
    private WaitForSeconds alphaDecreaseCooldown = new WaitForSeconds(0.1f);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (fadeOutOnAwake)
            StartCoroutine(FadingOutRoutine());
    }

    private IEnumerator FadingOutRoutine()
    {
        yield return new WaitForSeconds(lifeDuration);

        while (spriteRenderer.color.a > 0f)
        {
            Color color = spriteRenderer.color;
            color.a = color.a - alphaDecrease;
            spriteRenderer.color = color;
            yield return alphaDecreaseCooldown;
        }

        Destroy(gameObject);
    }
}
