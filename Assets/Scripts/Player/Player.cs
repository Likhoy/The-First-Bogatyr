using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public PlayerController playerControl;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private Inventory inventory;

    private void Awake()
    {
        // Load components
        playerControl = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
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
        // healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        // healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged2; // for button helper
    }

    

    private void OnDisable()
    {
        // Unsubscribe from player health event
        // healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }


    /// <summary>
    /// Handle health changed event
    /// </summary>
    /*private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        // If player has died
        if (healthEventArgs.healthAmount <= 0f)
        {
            if (health.UseExtraLive())
            {
                // do something
            }
            else
            {
                GameManager.Instance.HandlePlayerDeath();
            }
        }
    }*/

    private void HealthEvent_OnHealthChanged2(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthPercent <= 0.75f && healthEventArgs.damageAmount > 0)
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
        // AddWeaponToPlayer(playerDetails.startingWeapon);

        /*string weapon = "Bronze Sword";

        inventory.AddEquipment(weapon);
        inventory.EquipItemFromID(weapon);*/

        /*string armor = "Bronze Armor";

        inventory.AddEquipment(armor);
        inventory.EquipItemFromID(armor);*/

        string weapon2 = "Spear";

        inventory.AddEquipment(weapon2);
        inventory.EquipItemFromID(weapon2);
    }

    /// <summary>
    /// Add a weapon to the player weapon list 
    /// </summary>
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails, int weaponAmmoAmount = 0)
    {
        Weapon weapon;
        bool isWeaponRanged = false;
        if (weaponDetails is MeleeWeaponDetailsSO meleeWeaponDetails)
            weapon = new MeleeWeapon() { weaponDetails = meleeWeaponDetails, weaponCurrentMinDamage = weaponDetails.GetWeaponMinDamage(),
                                            weaponCurrentMaxDamage = weaponDetails.GetWeaponMaxDamage()};
        else
        {
            RangedWeaponDetailsSO rangedWeaponDetails = weaponDetails as RangedWeaponDetailsSO;

            int weaponRemainingAmmo = Mathf.Clamp(weaponAmmoAmount, 0, rangedWeaponDetails.weaponAmmoCapacity);
            weapon = new RangedWeapon() { weaponDetails = rangedWeaponDetails,
                weaponCurrentMinDamage = weaponDetails.GetWeaponMinDamage(),
                weaponCurrentMaxDamage = weaponDetails.GetWeaponMaxDamage(),
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
        // setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon, isWeaponRanged);

        return weapon;
    }

    /// <summary>
    /// Delete player weapon when out of ammo or in other cases
    /// </summary>
    public void DeletePlayerWeapon(int weaponListPosition)
    {
        if (weaponList.Count == 1)
        {
            weaponList.Clear();

            // No more weapons
            // setActiveWeaponEvent.CallSetActiveWeaponEvent(null, false);

            return;
        }

        Weapon previousWeapon = GetPreviousWeapon(weaponListPosition);

        // Set previous weapon as active if there is one
        // setActiveWeaponEvent.CallSetActiveWeaponEvent(previousWeapon, previousWeapon is RangedWeapon);

        // Remove weapon from the list
        weaponList.RemoveAt(weaponListPosition - 1);

        // Correct weapon list positions
        foreach (Weapon weapon in weaponList.Skip(weaponListPosition - 1))
            weapon.weaponListPosition--;
    }

    /// <summary>
    /// Get next weapon from the weapon list - without validating zero number
    /// </summary>
    /*public Weapon GetNextWeaponAfterCurrent()
    {
        Weapon currentWeapon = activeWeapon.GetCurrentWeapon();
        return currentWeapon.weaponListPosition == weaponList.Count ? weaponList[0] : weaponList[currentWeapon.weaponListPosition];
    }*/

    /// <summary>
    /// Get previous weapon from the weapon list - without validating zero number
    /// </summary>
    public Weapon GetPreviousWeapon(int weaponListPosition)
    {
        return weaponListPosition == 0 ? weaponList.Last() : weaponList[weaponListPosition - 2];
    }

    public Weapon FindWeaponByName(string weaponName)
    {
        return weaponList.Find(weapon => (weapon is MeleeWeapon meleeWeapon) ? 
        meleeWeapon.weaponDetails.weaponName == weaponName : 
        (weapon as RangedWeapon).weaponDetails.weaponName == weaponName);
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
        // health.SetStartingHealth(playerDetails.playerHealthAmount);
    }
}
