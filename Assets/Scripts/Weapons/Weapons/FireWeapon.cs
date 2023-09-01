using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private AudioSource audioEffects;
    [SerializeField] private AudioClip CFire;

    private void Awake()
    {
        // Load components.
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // Subscribe to fire weapon event.
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        // Unsubscribe from fire weapon event.
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        // Decrease cooldown timer.
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Handle fire weapon event.
    /// </summary>
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    /// <summary>
    /// Fire weapon.
    /// </summary>
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // Handle weapon precharge timer.
        //WeaponPreCharge(fireWeaponEventArgs);

        // Weapon fire.
        if (fireWeaponEventArgs.fire)
        {
            // Test if weapon is ready to fire.
            if (IsWeaponReadyToFire(fireWeaponEventArgs.targetPosition))
            {
                audioEffects.PlayOneShot(CFire);

                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector, fireWeaponEventArgs.targetPosition);

                ResetCoolDownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    /// <summary>
    /// Handle weapon precharge.
    /// </summary>
    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // Weapon precharge.
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            // Decrease precharge timer if fire button held previous frame.
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            // else reset the precharge timer.
            ResetPrechargeTimer();
        }
    }

    /// <summary>
    /// Returns true if the weapon is ready to fire, else returns false.
    /// </summary>
    private bool IsWeaponReadyToFire(Vector2 targetPosition)
    {
        RangedWeapon currentWeapon = activeWeapon.GetCurrentWeapon() as RangedWeapon;

        // if there is no appropriate weapon then return false
        if (currentWeapon == null)
            return false;

        // if there is no ammo and weapon doesn't have infinite ammo then return false.
        if (currentWeapon.weaponRemainingAmmo <= 0 && !currentWeapon.weaponDetails.hasInfiniteAmmo)
        {
            return false;
        } 

        // if the weapon is reloading then return false.
        if (currentWeapon.isWeaponReloading)
            return false;

        // If the weapon isn't precharged or is cooling down then return false.
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f)
            return false;

        // if no ammo in the clip and the weapon doesn't have infinite clip capacity then return false.
        if (!currentWeapon.weaponDetails.hasInfiniteClipCapacity && currentWeapon.weaponClipRemainingAmmo <= 0)
        {
            // trigger a reload weapon event.
            reloadWeaponEvent.CallReloadWeaponEvent(currentWeapon, 0);

            return false;
        }

        AmmoDetailsSO ammoDetails = currentWeapon.weaponDetails.weaponCurrentAmmo;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // if this is a throwing weapon and target is not reachable then return false
        if (ammoDetails.isThrowingWeaponAmmo && (distanceToTarget < ammoDetails.ammoDistanceMin || distanceToTarget > ammoDetails.ammoRange))
            return false;

        // weapon is ready to fire - return true
        return true;
    }

    /// <summary>
    /// Set up ammo using an ammo gameobject and component from the object pool.
    /// </summary>
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, Vector2 targetPosition)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            // Fire ammo routine.
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector, targetPosition));
        }
    }

    /// <summary>
    /// Coroutine to spawn multiple ammo per shot if specified in the ammo details
    /// </summary>
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, Vector2 targetPosition)
    {

        RangedWeapon currentWeapon = activeWeapon.GetCurrentWeapon() as RangedWeapon;

        int ammoCounter = 0;

        // Get random ammo per shot
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        // Get random interval between ammo
        float ammoSpawnInterval;

        if (ammoPerShot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        // Loop for number of ammo per shot
        while (ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            // Get ammo prefab from array
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            // Get random speed value
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            GameObject ammoGameObject = Instantiate(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity, gameObject.transform);

            IFireable ammo = ammoGameObject.GetComponent<IFireable>();

            ammo.InitialiseAmmo(currentAmmo, gameObject, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, targetPosition);

           
            // Get Gameobject with IFireable component
            // IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            // Wait for ammo per shot timegap
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        // Reduce ammo clip count if not infinite clip capacity
        if (!currentWeapon.weaponDetails.hasInfiniteClipCapacity)
        {
            currentWeapon.weaponClipRemainingAmmo--;
        }
        currentWeapon.weaponRemainingAmmo--;

        // if player weapon has run out of ammo than delete player weapon (maybe we'll need an event here)
        if (currentWeapon.weaponRemainingAmmo <= 0 && currentWeapon.weaponDetails.weaponCurrentAmmo.isPlayerAmmo)
            GameManager.Instance.GetPlayer().DeletePlayerWeapon(currentWeapon.weaponListPosition);

        // Call weapon fired event
        weaponFiredEvent.CallWeaponFiredEvent(currentWeapon);

        // Display weapon shoot effect
        // WeaponShootEffect(aimAngle);

        // Weapon fired sound effect
        // WeaponSoundEffect();
    }

    /// <summary>
    /// Reset cooldown timer
    /// </summary>
    private void ResetCoolDownTimer()
    {
        // Reset cooldown timer
        fireRateCoolDownTimer = ((RangedWeapon)activeWeapon.GetCurrentWeapon()).weaponDetails.weaponFireRate;
    }

    /// <summary>
    /// Reset precharge timers
    /// </summary>
    private void ResetPrechargeTimer()
    {
        // Reset precharge timer
        firePreChargeTimer = ((RangedWeapon)activeWeapon.GetCurrentWeapon()).weaponDetails.weaponPrechargeTime;
    }


    /// <summary>
    /// Display the weapon shoot effect
    /// </summary>
    /*private void WeaponShootEffect(float aimAngle)
    {
        // Process if there is a shoot effect & prefab
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect != null && activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab != null)
        {
            // Get weapon shoot effect gameobject from the pool with particle system component
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab, activeWeapon.GetShootEffectPosition(), Quaternion.identity);

            // Set shoot effect
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect, aimAngle);

            // Set gameobject active (the particle system is set to automatically disable the
            // gameobject once finished)
            weaponShootEffect.gameObject.SetActive(true);
        }
    }*/

    /// <summary>
    /// Play weapon shooting sound effect
    /// </summary>
    /*private void WeaponSoundEffect()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }*/
}