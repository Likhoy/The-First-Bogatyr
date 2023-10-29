using PixelCrushers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(PlayerResources))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(MeleeAttackEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(ItemUsedEvent))]
[RequireComponent(typeof(ProductBoughtEvent))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ReceiveContactDamage))]
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public PlayerResources playerResources;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public PlayerHealth health;
    [HideInInspector] public DestroyedEvent destroyedEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public MeleeAttackEvent meleeAttackEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public ItemUsedEvent itemUsedEvent;
    [HideInInspector] public ProductBoughtEvent productBoughtEvent;
    [HideInInspector] public PlayerController playerControl;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        // Load components
        playerResources = GetComponent<PlayerResources>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<PlayerHealth>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        meleeAttackEvent = GetComponent<MeleeAttackEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        itemUsedEvent = GetComponent<ItemUsedEvent>();
        productBoughtEvent = GetComponent<ProductBoughtEvent>();
        playerControl = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Initialize the player
    /// </summary>
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        // Create player starting weapons
        CreatePlayerStartingWeapon();

        // Set player starting health
        SetPlayerHealth();
    }

    private void OnEnable()
    {
        // Subscribe to player health event
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged2; // for button helper
    }

    private void OnDisable()
    {
        // Unsubscribe from player health event
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }


    /// <summary>
    /// Handle health changed event
    /// </summary>
    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        // If player has died
        if (healthEventArgs.healthAmount <= 0f)
        {
            GameManager.Instance.HandlePlayerDeath();
        }
    }

    private void HealthEvent_OnHealthChanged2(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthPercent <= 0.75f)
        {
            FadingOutText fadingOutText = FindObjectOfType<FadingOutText>();
            if (fadingOutText != null && !GameProgressData.healthHintShown) 
            {
                fadingOutText.TextToShow = "Святая вода восстанавливает здоровье...";
                fadingOutText.ShowHint(0);
                GameProgressData.healthHintShown = true;
            }
            healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged2;
        }
    }

    /// <summary>
    /// Set the player starting weapon
    /// </summary>
    private void CreatePlayerStartingWeapon()
    {
        // Clear list
        weaponList.Clear();

        // Add weapon to player
        AddWeaponToPlayer(playerDetails.startingWeapon);
    }

    /// <summary>
    /// Add a weapon to the player weapon list 
    /// </summary>
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails, int weaponAmmoAmount = 0)
    {
        Weapon weapon;
        bool isWeaponRanged = false;
        if (weaponDetails is MeleeWeaponDetailsSO meleeWeaponDetails)
            weapon = new MeleeWeapon() { weaponDetails = meleeWeaponDetails };
        else
        {
            RangedWeaponDetailsSO rangedWeaponDetails = weaponDetails as RangedWeaponDetailsSO;

            int weaponRemainingAmmo = Mathf.Clamp(weaponAmmoAmount, 0, rangedWeaponDetails.weaponAmmoCapacity);
            weapon = new RangedWeapon() { weaponDetails = rangedWeaponDetails, 
                weaponRemainingAmmo = weaponRemainingAmmo,
                weaponClipRemainingAmmo = weaponRemainingAmmo < rangedWeaponDetails.weaponClipAmmoCapacity || rangedWeaponDetails.hasInfiniteClipCapacity ? 
                weaponRemainingAmmo : rangedWeaponDetails.weaponClipAmmoCapacity };
            
            isWeaponRanged = true;
        }

        // Add the weapon to the list
        weaponList.Add(weapon);

        // Set weapon position in list
        weapon.weaponListPosition = weaponList.Count;

        // Set the added weapon as active
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon, isWeaponRanged);

        return weapon;
    }

    /// <summary>
    /// Delete player weapon when out of ammo or in other cases
    /// </summary>
    public void DeletePlayerWeapon(int weaponListPosition)
    {
        if (weaponList.Count == 0)
        {
            // here should be the message of deleting last weapon

            return;
        }

        Weapon previousWeapon = GetPreviousWeapon(weaponListPosition);

        // Set previous weapon as active if there is one
        setActiveWeaponEvent.CallSetActiveWeaponEvent(previousWeapon, previousWeapon is RangedWeapon);

        // Remove weapon from the list
        weaponList.RemoveAt(weaponListPosition - 1);

        // Correct weapon list positions
        foreach (Weapon weapon in weaponList.Skip(weaponListPosition - 1))
            weapon.weaponListPosition--;
    }

    /// <summary>
    /// Get next weapon from the weapon list - without validating zero number
    /// </summary>
    public Weapon GetNextWeaponAfterCurrent()
    {
        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        return currentWeapon.weaponListPosition == weaponList.Count ? weaponList[0] : weaponList[currentWeapon.weaponListPosition];
    }

    /// <summary>
    /// Get previous weapon from the weapon list - without validating zero number
    /// </summary>
    public Weapon GetPreviousWeapon(int weaponListPosition)
    {
        return weaponListPosition == 0 ? weaponList.Last() : weaponList[weaponListPosition - 2];
    }

    /// <summary>
    /// Returns the player position
    /// </summary>
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Set player health from playerDetails SO
    /// </summary>
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }
}
