using UnityEngine;

public class ThrowingWeapon : MonoBehaviour, IFireable
{

    private AmmoDetailsSO ammoDetails;
    private SpriteRenderer spriteRenderer;
    private float ammoSpeed;

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

        circleAngle += Time.deltaTime * ammoSpeed / radius * circleDirectionToggle; // calculating angular velocity

        // Rotate ammo
        /*if (ammoDetails.ammoBaseRotationSpeed > 0f)
        {
            transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoBaseRotationSpeed * Time.deltaTime));
        }*/

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

        // Set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector, targetPosition);

        SetCircleCenter();

        circleAngle = Mathf.Deg2Rad * HelperUtilities.GetAngleFromVector(transform.position - (Vector3)circleCenter);

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
        float halfChordLength = (landingPosition - (Vector2)transform.position).magnitude / 2f;

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

        Vector2 perpendicularVector = new Vector2(-motionDirection.y * circleDirectionToggle, motionDirection.x * circleDirectionToggle).normalized * radius;

        this.circleCenter = (Vector2)transform.position + perpendicularVector;
    }

    /// <summary>
    /// Set ammo fire direction based on the input angle and direction adjusted by the
    /// spread calculated for needed distance
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, Vector2 targetPosition)
    {
        float distance = (targetPosition - (Vector2)transform.position).magnitude - ammoDetails.ammoDistanceMin;

        float maxDistance = ammoDetails.ammoRange - ammoDetails.ammoDistanceMin;

        // calculate spread for needed distance
        float spread = ammoDetails.ammoSpreadMin + (distance / maxDistance) * (ammoDetails.ammoSpreadMax - ammoDetails.ammoSpreadMin);

        // Get a random spread direction angle between 0 and 360 degrees
        float spreadDirectionAngle = Random.Range(0f, 360f);

        landingPosition = targetPosition + spread * (Vector2)HelperUtilities.GetDirectionVectorFromAngle(spreadDirectionAngle);

        directionVector = (Vector3)landingPosition - transform.position;

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
