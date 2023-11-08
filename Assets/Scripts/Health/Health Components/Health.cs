using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    protected int initialHealth;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public HealthEvent healthEvent;
    private Coroutine effectCoroutine;
    protected bool hasHitEffect = false;
    protected float effectTime = 0f;
    protected SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.33f;
    private WaitForSeconds waitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public List<Protection> currentProtections = new List<Protection>();

    private void Awake()
    {
        // Load components
        healthEvent = GetComponent<HealthEvent>();
    }

    protected virtual void Start()
    {
        // Trigger a health event for UI update
        CallHealthEvent(0);
    }

    /// <summary>
    /// Public method called when damage is taken
    /// </summary>
    public virtual void TakeDamage(int damageAmount)
    {

        if (isDamageable)
        {
            int processedDamage = currentProtections.Count > 0 ? 
                ProcessRawDamage(damageAmount) : damageAmount;
            
            if (processedDamage == 0)
                return;

            currentHealth -= processedDamage;
            CallHealthEvent(processedDamage);

            if (hasHitEffect)
                PostHitEffect();
        }

    }

    private int ProcessRawDamage(int rawDamage)
    {
        foreach (Protection protection in currentProtections)
        {
            protection.ApplyEffect(ref rawDamage);
            if (rawDamage <= 0)
                return 0;
        }  

        return rawDamage;
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

    protected void CallHealthEvent(int damageAmount)
    {
        // Trigger health event
        healthEvent.CallHealthChangedEvent((float)currentHealth / (float)GetMaxHealth(), currentHealth, GetMaxHealth(), damageAmount);
    }


    /// <summary>
    /// Set starting health 
    /// </summary>
    public virtual void SetStartingHealth(int startingHealth)
    {
        initialHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    /// Get the starting health
    /// </summary>
    public virtual int GetMaxHealth()
    {
        return initialHealth;
    }

    public int GetInitialHealth()
    {
        return initialHealth;
    }

    /// <summary>
    /// Increase health by specified percent
    /// </summary>
    public void AddHealth(float healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((GetMaxHealth() * healthPercent) / 100f);

        int totalHealth = currentHealth + healthIncrease;

        if (totalHealth > GetMaxHealth())
        {
            currentHealth = GetMaxHealth();
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

        if (totalHealth > GetMaxHealth())
        {
            currentHealth = GetMaxHealth();
        }
        else
        {
            currentHealth = totalHealth;
        }

        CallHealthEvent(0);
    }

}
