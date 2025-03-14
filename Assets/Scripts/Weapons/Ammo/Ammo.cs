using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate with child TrailRenderer component")]
    #endregion Tooltip
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f; // the range of each ammo
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private RangedWeapon currentWeaponRef;
    private float ammoChargeTimer;
    private float ammoGettingVisibleTimer;
    private bool isAmmoVisible = true;
    private bool isAmmoMaterialSet = true;
    private bool overrideAmmoMovement;
    private bool isColliding = false;
    private GameObject shooter;

    private void Awake()
    {
        // cache sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Handle ammo invisibility
        if (ammoGettingVisibleTimer > 0f)
        {
            ammoGettingVisibleTimer -= Time.deltaTime;
        }
        // Enable sprite renderer when outside of char body
        else if (!isAmmoVisible)
        {
            ToggleVisibility(true);
        }

        // Ammo charge effect
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // Don't move ammo if movement has been overriden - e.g. this ammo is part of an ammo pattern
        if (!overrideAmmoMovement)
        {
            // Calculate distance vector to move ammo
            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

            transform.position += distanceVector;

            // Disable after max range reached
            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0f)
            {
                if (ammoDetails.isPlayerAmmo)
                {
                    // no multiplier
                    //StaticEventHandler.CallMultiplierEvent(false);
                }

                DisableAmmo();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If already colliding with something return
        if (isColliding) return;

        // handle shooter hitting themselves
        if (collision.gameObject == shooter) return;
        
        // Deal Damage To Collision Object
        DealDamage(collision);

        // Show ammo hit effect
        // AmmoHitEffect();

        DisableAmmo();
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        if (health != null)
        {
            // Set isColliding to prevent ammo dealing damage multiple times
            isColliding = true;

            health.TakeDamage(currentWeaponRef.GetWeaponDamage()); // TODO: think about this method
        }

    }


    /// <summary>
    /// Initialise the ammo being fired - using the ammodetails, the aimangle, weaponAngle, and
    /// weaponAimDirectionVector. If this ammo is part of a pattern the ammo movement can be
    /// overriden by setting overrideAmmoMovement to true
    /// </summary>
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, GameObject shooter, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, Vector2 targetPosition, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;

        currentWeaponRef = shooter.GetComponent<ActiveWeapon>().GetCurrentWeapon() as RangedWeapon;

        this.shooter = shooter;

        if (ammoDetails.ammoInvisibleTime > 0f)
        {
            ammoGettingVisibleTimer = ammoDetails.ammoInvisibleTime;
            // Set ammo invisible while it's inside of charater body
            ToggleVisibility(false);
        }

        // Set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // Set ammo sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        // set initial ammo material depending on whether there is an ammo charge period
        if (ammoDetails.ammoChargeTime > 0f)
        {
            // Set ammo charge timer
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // Set ammo range
        ammoRange = ammoDetails.ammoRange;

        // Set ammo speed
        this.ammoSpeed = ammoSpeed;

        // Override ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        // Activate ammo gameobject
        gameObject.SetActive(true);

        #endregion Ammo


        /*#region Trail

        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion Trail*/

    }

    // toggle visibility of ammo object
    private void ToggleVisibility(bool isVisible)
    {
        spriteRenderer.enabled = isVisible;
        isAmmoVisible = isVisible;
    }

    /// <summary>
    /// Set ammo fire direction and angle based on the input angle and direction adjusted by the
    /// random spread
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {

        // calculate random spread angle between min and max
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // Get a random spread toggle of 1 or -1
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        // Adjust ammo fire angle angle by random spread
        fireDirectionAngle += spreadToggle * randomSpread;

        // Set ammo rotation
        // transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        // Set ammo fire direction
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);

    }

    /// <summary>
    /// Destroy the ammo
    /// </summary>
    private void DisableAmmo()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Display the ammo hit effect
    /// </summary>
    /*private void AmmoHitEffect()
    {
        // Process if a hit effect has been specified
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            // Get ammo hit effect gameobject from the pool (with particle system component)
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

            // Set Hit Effect
            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);

            // Set gameobject active (the particle system is set to automatically disable the
            // gameobject once finished)
            ammoHitEffect.gameObject.SetActive(true);
        }
    }*/


    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        // HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }

#endif
    #endregion Validation

}