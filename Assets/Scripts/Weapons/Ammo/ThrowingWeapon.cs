using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
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

        // Calculate tangent vector to move ammo
        /*Vector2 tangentVector = ammoSpeed * Time.deltaTime * motionVector + (Vector2)transform.position;

        float distanceToCenter = (circleCenter - tangentVector).magnitude;

        // Adjust position to be on the circle
        Vector2 newPosition = (circleCenter - tangentVector).normalized * Mathf.Sqrt(distanceToCenter - radius) + tangentVector;*/

        float newX = circleCenter.x + radius * Mathf.Cos(circleAngle);
        float newY = circleCenter.y + radius * Mathf.Sin(circleAngle);

        transform.position = new Vector3(newX, newY);

        circleAngle += Time.deltaTime * ammoSpeed / radius * circleDirectionToggle; // calculating angular velocity

        // Rotate ammo
        /*if (ammoDetails.ammoBaseRotationSpeed > 0f)
        {
            transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoBaseRotationSpeed * Time.deltaTime));
        }*/

        /*Vector2 centerVector = circleCenter - newPosition;

        // Adjust motion vector to organize movement of the ammo in the circle
        motionVector = new Vector2(-centerVector.y, centerVector.x).normalized;*/

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

        SetCircleCenter(landingPosition, relativeThrowingAngle);

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

    /*private void SetCircleCenter(Vector2 targetPosition, float ammoThrowingAngle)
    {
        float chordAngle = 90f - ammoThrowingAngle;

        float chordLength = (targetPosition - (Vector2)transform.position).magnitude;

        radius = chordLength / (2f * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * ammoThrowingAngle)));

        Vector2 perpendicularVector = new Vector2(motionVector.y, -motionVector.x).normalized * Mathf.Sqrt(radius);

        this.circleCenter = perpendicularVector + (Vector2)transform.position; 
    }*/

    private void SetCircleCenter(Vector2 targetPosition, float ammoThrowingAngle)
    {
        float halfChordLength = (targetPosition - (Vector2)transform.position).magnitude / 2f;

        radius = halfChordLength / Mathf.Sin(ammoThrowingAngle * Mathf.Deg2Rad / 2f);

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
        float distance = (targetPosition - (Vector2)transform.position).magnitude;

        float maxDistance = ammoDetails.ammoRange - ammoDetails.ammoDistanceMin;

        // calculate spread for needed distance
        float spread = ammoDetails.ammoSpreadMin + (distance / maxDistance) * (ammoDetails.ammoSpreadMax - ammoDetails.ammoSpreadMin);

        // Get a random spread direction angle between 0 and 360 degrees
        float spreadDirectionAngle = Random.Range(0f, 360f);

        landingPosition = targetPosition + spread * (Vector2)HelperUtilities.GetDirectionVectorFromAngle(spreadDirectionAngle);

        directionVector = (Vector3)landingPosition - transform.position;

        shooterToTargetDirectionAngle = HelperUtilities.GetAngleFromVector(directionVector); // from -180 to 180 degrees

        relativeThrowingAngle = ammoDetails.ammoBaseThrowingAngle * (Mathf.Abs(90f - Mathf.Abs(shooterToTargetDirectionAngle)) / 90f);

        // Set ammo flight initial direction
        // motionVector = ((Vector2)HelperUtilities.GetDirectionVectorFromAngle(throwingAngle) + realPosition.normalized).normalized;
    }

    /// <summary>
    /// Destroy the ammo
    /// </summary>
    private void DisableAmmo()
    {
        Destroy(gameObject);
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

}
