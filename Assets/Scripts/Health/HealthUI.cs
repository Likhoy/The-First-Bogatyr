using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    // for testing
    //private TextMeshProUGUI textMeshPro;
    public Image healthBar;

    private void Awake()
    {
        // Load components
    //    textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthUI(healthEventArgs);
    }



    private void SetHealthUI(HealthEventArgs healthEventArgs)
    {
     //   textMeshPro.text = "Health: " + healthEventArgs.healthAmount.ToString();
        healthBar.fillAmount = healthEventArgs.healthAmount / 100f;
     
    }
}
