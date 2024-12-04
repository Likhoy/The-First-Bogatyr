using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text hpText;

    public GameObject player;
    private void Awake()
    {
        
    }

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

        int maxHp = stat.totalStat.health;
        float hp = stat.health;
        float curHp = hp / maxHp;

        if (curHp > hpBar.fillAmount)
        {
            hpBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
            if (hpBar.fillAmount > curHp)
            {
                hpBar.fillAmount = curHp;
            }
        }
        if (curHp < hpBar.fillAmount)
        {
            hpBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
            if (hpBar.fillAmount < curHp)
            {
                hpBar.fillAmount = curHp;
            }
        }

        if (hpText)
        {
            hpText.text = hp.ToString() + "/" + maxHp.ToString();
        }
    }
    //    private void OnEnable()
    //{
    //    // GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    //}

    //private void OnDisable()
    //{
    //    // GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    //}

    //private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    //{
    //    SetHealthUI(healthEventArgs);
    //}

    //private void SetHealthUI(HealthEventArgs healthEventArgs)
    //{
    //    healthBar.fillAmount = healthEventArgs.healthPercent;
    //    healthText.text = healthEventArgs.healthAmount.ToString() + "/" + healthEventArgs.maxHealthAmount.ToString();
    //}
}
