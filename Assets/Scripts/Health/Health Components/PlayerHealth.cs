using UnityEngine;

public class PlayerHealth : Health
{
    private int extraLives;
    private int maxHealth;
    private Player player;

    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();

        if (player.playerDetails.hasHitEffect)
        {
            hasHitEffect = true;
            EffectTime = player.playerDetails.hitEffectTime;
            SpriteRenderer = player.spriteRenderer;
        }
    }

    public void IncreaseMaxHealth(int healthPercentToAdd)
    {
        maxHealth = initialHealth;
        maxHealth += Mathf.RoundToInt((float)healthPercentToAdd / 100 * initialHealth);
        CallHealthEvent(0);
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

    public override void SetStartingHealth(int startingHealth)
    {
        base.SetStartingHealth(startingHealth);
        maxHealth = startingHealth;
    }

    public override int GetMaxHealth()
    {
        return maxHealth;
    }
}
