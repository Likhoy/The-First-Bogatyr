using UnityEngine;

[RequireComponent(typeof(TemporaryObject))]
public class ThrowingWeapon : MonoBehaviour, IFireable
{

    private AmmoDetailsSO ammoDetails;
    private SpriteRenderer spriteRenderer;
    private float ammoSpeed;

    private Vector2 weaponShootPosition;
    private Vector3 directionVector;
    private Vector2 landingPosition;
    private float relativeThrowingAngle;
    private bool targetReached = false;
    private float circleAngle; // in radians
    private float shooterToTargetDirectionAngle;
    private Vector2 circleCenter;
    private float radius;
    private float circleDirectionToggle;
    private float xRotationAngle;
    private float xRotationToggle; // upwards we need negative angle offset
    private const float xRotationAngleCoefficient = 1.5f;

    private bool isColliding = false;
    private GameObject shooter;

    private PolygonCollider2D polygonCollider;
    private TemporaryObject temporaryObjectComponent; // for fading out when not reaching any collider

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        temporaryObjectComponent = GetComponent<TemporaryObject>();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private void Update()
    {
        if (targetReached) return;

        float newX = circleCenter.x + radius * Mathf.Cos(circleAngle);
        float newY = circleCenter.y + radius * Mathf.Sin(circleAngle);

        Vector3 newPosition = new Vector3(newX, newY);

        // already travelled path to the landing position
        float traveledPartOftheChord = Vector3.Project(newPosition - transform.position, directionVector).magnitude;

        transform.position = newPosition;

        float angleOffset = Time.deltaTime * ammoSpeed / radius * circleDirectionToggle;

        circleAngle += angleOffset; // calculating angular velocity

        // calculating correct speed of x-axis rotation using travelled path to whole distance ratio
        float xRotationAngleOffset = (traveledPartOftheChord / directionVector.magnitude) * 2 * xRotationAngle * -xRotationToggle;

        transform.Rotate(xRotationAngleOffset, 0, angleOffset * Mathf.Rad2Deg, Space.World); // World axes orientation is required here

        if (Vector3.Distance(transform.position, landingPosition) < 0.1f)
        {
            targetReached = true;
            DisableWeapon();
        }

    }

    /// <summary>
    /// Disable weapon when not reaching anything
    /// </summary>
    private void DisableWeapon()
    {
        temporaryObjectComponent.StartCoroutine("FadingOutRoutine");

        Vector3 scale = transform.lossyScale;

        // losing parent to be independent
        transform.SetParent(null);

        // return global scale 
        transform.localScale = scale;

        Destroy(polygonCollider);
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

        DestroyWeapon();
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        if (health != null)
        {
            // Set isColliding to prevent ammo dealing damage multiple times
            isColliding = true;

            health.TakeDamage(ammoDetails.GetAmmoDamage());
        }
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, GameObject shooter, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, Vector2 targetPosition, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;

        this.shooter = shooter;

        this.ammoSpeed = ammoSpeed;

        transform.rotation = ammoDetails.baseRotation;

        // Set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector, targetPosition);

        SetCircleCenter();

        circleAngle = Mathf.Deg2Rad * HelperUtilities.GetAngleFromVector(weaponShootPosition - circleCenter);

        // Set ammo sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        // set initial ammo material depending on whether there is an ammo charge period
        if (ammoDetails.ammoChargeTime > 0f)
        {
            // Set ammo charge timer
            // ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            // isAmmoMaterialSet = false;
        }
        else
        {
            // ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            // isAmmoMaterialSet = true;
        }

        // Activate ammo gameobject
        gameObject.SetActive(true);
    }

    private void SetCircleCenter()
    {
        float halfChordLength = (landingPosition - weaponShootPosition).magnitude / 2f;

        xRotationAngle = (ammoDetails.ammoBaseThrowingAngle - relativeThrowingAngle) * xRotationAngleCoefficient;

        radius = halfChordLength / Mathf.Sin(relativeThrowingAngle * Mathf.Deg2Rad);

        // if we are in the right area of the coordinate system then we need bigger angle and clockwise movement, otherwise - vice versa
        float realThrowingAngle;
        if (directionVector.x > 0)
        {
            realThrowingAngle = shooterToTargetDirectionAngle + relativeThrowingAngle;
            circleDirectionToggle = -1;
        }
        else
        {
            realThrowingAngle = shooterToTargetDirectionAngle - relativeThrowingAngle;
            circleDirectionToggle = 1;
        }

        xRotationToggle = directionVector.y > 0 ? -1 : 1;

        Vector2 motionDirection = HelperUtilities.GetDirectionVectorFromAngle(realThrowingAngle);

        transform.Rotate(xRotationAngle * xRotationToggle, 0, realThrowingAngle, Space.World);

        Vector2 perpendicularVector = new Vector2(-motionDirection.y * circleDirectionToggle, motionDirection.x * circleDirectionToggle).normalized * radius;

        this.circleCenter = weaponShootPosition + perpendicularVector;
    }

    /// <summary>
    /// Set ammo fire direction based on the input angle and direction adjusted by the
    /// spread calculated for needed distance
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, Vector2 targetPosition)
    {
        // Get weapon shoot position from which the ammo will start it's flight
        weaponShootPosition = targetPosition - (Vector2)weaponAimDirectionVector;

        float distance = (targetPosition - weaponShootPosition).magnitude - ammoDetails.ammoDistanceMin;

        float maxDistance = ammoDetails.ammoRange - ammoDetails.ammoDistanceMin;

        // Calculate spread for needed distance
        float spread = ammoDetails.ammoSpreadMin + (distance / maxDistance) * (ammoDetails.ammoSpreadMax - ammoDetails.ammoSpreadMin);

        // Get a random spread direction angle between 0 and 360 degrees
        float spreadDirectionAngle = Random.Range(0f, 360f);

        landingPosition = targetPosition + spread * (Vector2)HelperUtilities.GetDirectionVectorFromAngle(spreadDirectionAngle);

        directionVector = landingPosition - weaponShootPosition;

        shooterToTargetDirectionAngle = HelperUtilities.GetAngleFromVector(directionVector); // from -180 to 180 degrees

        relativeThrowingAngle = ammoDetails.ammoBaseThrowingAngle * (Mathf.Abs(90f - Mathf.Abs(shooterToTargetDirectionAngle)) / 90f);
    }

    /// <summary>
    /// Destroy the ammo completely when reaching anything
    /// </summary>
    private void DestroyWeapon()
    {
        Destroy(gameObject);
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

}
