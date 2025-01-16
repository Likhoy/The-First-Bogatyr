/*using UnityEngine;

public class EnemyHealth : Health
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the HealthBar component on the HealthBar gameobject")]
    #endregion
    [SerializeField] private HealthBar healthBar;

    [HideInInspector] public Enemy enemy;

    protected override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();

        if (enemy.enemyDetails.hasHitEffect)
        {
            hasHitEffect = true;
            EffectTime = enemy.enemyDetails.hitEffectTime;
            SpriteRenderer = enemy.spriteRendererArray[0];
        }

        // Enable the health bar if required
        if (enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if (healthBar != null)
        {
            healthBar.DisableHealthBar();
        }
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);

        if (isDamageable)
        {
            // Set health bar as the percentage of health remaining
            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)initialHealth);
            }
        }

    }


}
*/