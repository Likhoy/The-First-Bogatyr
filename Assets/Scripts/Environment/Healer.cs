using System.Collections;
using UnityEngine;

public class Healer : MonoBehaviour, IUseable
{
    #region Header GAME EFFECT SETTINGS
    [Header("GAME EFFECT SETTINGS")]
    #endregion Header GAME EFFECT SETTINGS
    #region Tooltip
    [Tooltip("What amount of health will be healed per second")]
    #endregion Tooltip
    [SerializeField] private int healthBoostPerSecond;
    #region Tooltip
    [Tooltip("Healing effect duration")]
    #endregion Tooltip
    [SerializeField] private float healingDuration;

    private Player player;
    private float healingTimer;

    private void Awake()
    {
        // Load components
        player = GameManager.Instance.GetPlayer();
    }

    private void Update()
    {
        healingTimer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        // Subscribe to item used event
        // player.itemUsedEvent.OnItemUsed += ItemUsedEvent_OnHealerUsed;
    }

    private void OnDisable()
    {
        // Unsubscribe from item used event
        // player.itemUsedEvent.OnItemUsed -= ItemUsedEvent_OnHealerUsed;
    }

    private void ItemUsedEvent_OnHealerUsed(ItemUsedEvent itemUsedEvent, ItemUsedEventArgs itemUsedEventArgs)
    {
        UseItem();
    }

    private IEnumerator HealerEffectRoutine()
    {
        while (healingTimer >= 0f)
        {
            // player.health.AddHealth(healthBoostPerSecond);
            yield return new WaitForSeconds(1f);
        }
    }

    public void UseItem()
    {
        // AnimateDrinking();
        healingTimer = healingDuration;
        StopAllCoroutines();
        StartCoroutine(HealerEffectRoutine());
    }
}
