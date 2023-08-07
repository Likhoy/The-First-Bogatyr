using UnityEngine;

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

    private void Awake()
    {
        // cache sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        transform.position = new Vector3(newX, newY);

        float angleOffset = Time.deltaTime * ammoSpeed / radius * circleDirectionToggle;

        circleAngle += angleOffset; // calculating angular velocity

        transform.rotation *= Quaternion.Euler(0, 0, angleOffset * Mathf.Rad2Deg);

        if (Vector3.Distance(transform.position, landingPosition) < 0.2f)
        {
            targetReached = true;
            DisableAmmo();
        }

    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, GameObject shooter, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, Vector2 targetPosition, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;

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

        Vector2 motionDirection = HelperUtilities.GetDirectionVectorFromAngle(realThrowingAngle);

        transform.rotation *= Quaternion.Euler(0, 0, realThrowingAngle);

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
    /// Destroy the ammo
    /// </summary>
    private void DisableAmmo()
    {
        Destroy(gameObject); // for testing
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

}
