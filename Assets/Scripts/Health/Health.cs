using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour // player and other health types need to be divided
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the HealthBar component on the HealthBar gameobject")]
    #endregion
    [SerializeField] private HealthBar healthBar;
    private int extraLives;
    private int maxHealth;
    private int initialHealth;
    public int currentHealth;
    internal HealthEvent healthEvent;
    private Player player;
    private Coroutine effectCoroutine;
    private bool hasHitEffect = false;
    private float effectTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.33f;
    private WaitForSeconds waitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;

    public List<Protection> currentProtections = new List<Protection>();

    private void Awake()
    {
        //Load compnents
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        // Trigger a health event for UI update
        CallHealthEvent(0);

        // Attempt to load enemy / player components
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        DestroyableItem destroyableItem = GetComponent<DestroyableItem>();

        // Get player / enemy hit effect details
        if (player != null)
        {
            if (player.playerDetails.hasHitEffect)
            {
                hasHitEffect = true;
                effectTime = player.playerDetails.hitEffectTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetails.hasHitEffect)
            {
                hasHitEffect = true;
                effectTime = enemy.enemyDetails.hitEffectTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }
        else if (destroyableItem != null)
        {
            hasHitEffect = true;
            effectTime = destroyableItem.effectTime;
            spriteRenderer = destroyableItem.GetComponent<SpriteRenderer>();
        }

        // Enable the health bar if required
        if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if (healthBar != null)
        {
            healthBar.DisableHealthBar();
        }
    }

    /// <summary>
    /// Public method called when damage is taken
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        bool isDashing = false;

        /*if (player != null)
            isDashing = player.playerControl.isPlayerDashing;*/

        if (isDamageable && !isDashing)
        {
            int processedDamage = ProcessRawDamage(damageAmount);
            
            if (processedDamage == 0)
                return;

            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            if (hasHitEffect)
                PostHitEffect();

            // Set health bar as the percentage of health remaining
            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)maxHealth);
            }
        }

    }

    private int ProcessRawDamage(int rawDamage)
    {
        foreach (Protection protection in currentProtections)
            protection.ApplyEffect(ref rawDamage);

        return rawDamage;
    }

    public void IncreaseMaxHealth(int healthPercentToAdd)
    {
        maxHealth += Mathf.RoundToInt((float)healthPercentToAdd / 100 * initialHealth);
    }

    public void AddExtraLives(int extraLivesToAdd)
    {
        extraLives += extraLivesToAdd;
    }

    public bool UseExtraLive()
    {
        if (extraLives <= 0)
            return false;
        
        extraLives--;
        // TODO: play resurrection animation and only then return player max health value
        currentHealth = maxHealth;
        return true;
    }

    /// <summary>
    /// Indicate a hit and give some post hit effect
    /// </summary>
    private void PostHitEffect()
    {
        // Check if gameobject is active - if not return
        if (gameObject.activeSelf == false)
            return;

        if (effectCoroutine != null)
            StopCoroutine(effectCoroutine);

        // flash red and give period of immunity
        effectCoroutine = StartCoroutine(PostHitEffectRoutine(effectTime, spriteRenderer));

        /*// If there is post hit immunity then
        if (isImmuneAfterHit)
        {
            
        }*/

    }

    /// <summary>
    /// Coroutine to indicate a hit and give some post hit effect
    /// </summary>
    private IEnumerator PostHitEffectRoutine(float effectTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(effectTime / spriteFlashInterval / 2f);

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return waitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return waitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;

        }

    }

    private void CallHealthEvent(int damageAmount)
    {
        // Trigger health event
        healthEvent.CallHealthChangedEvent((float)currentHealth / (float)maxHealth, currentHealth, damageAmount);
    }


    /// <summary>
    /// Set starting health 
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        maxHealth = startingHealth;
        initialHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    /// Get the starting health
    /// </summary>
    public int GetStartingHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Increase health by specified percent
    /// </summary>
    public void AddHealth(float healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((maxHealth * healthPercent) / 100f);

        int totalHealth = currentHealth + healthIncrease;

        if (totalHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }

        CallHealthEvent(0);
    }

    /// <summary>
    /// Increase health by specified value
    /// </summary>
    public void AddHealth(int healthBoost)
    {
        int totalHealth = currentHealth + healthBoost;

        if (totalHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }

        CallHealthEvent(0);
    }

}
