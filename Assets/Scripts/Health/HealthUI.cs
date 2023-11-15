using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        
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
        healthBar.fillAmount = healthEventArgs.healthPercent;
        healthText.text = healthEventArgs.healthAmount.ToString() + "/" + healthEventArgs.maxHealthAmount.ToString();
    }
}
