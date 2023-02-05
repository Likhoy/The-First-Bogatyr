using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Select the layers that the enemy bullets will hit")]
    #endregion Tooltip
    [SerializeField] private LayerMask layerMask;
    #region Tooltip
    [Tooltip("Populate this with the WeaponShootPosition child gameobject transform")]
    #endregion Tooltip
    [SerializeField] private Transform weaponShootPosition;
    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;


    

    private void Awake()
    {
        // Load Components
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetails = enemy.enemyDetails;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }


    private void Update()
    {
        // if chasing player
        if (enemy.enemyMovementAI.chasePlayer)
        {
            // if close enough use melee attack
            if (Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) <= enemy.enemyDetails.handDistance)
            {
                MeleeAttack();
            }
            // else fire if possible
            else if (enemy.enemyDetails.enemyRangedWeapon != null)
            {
                // Update timers
                firingIntervalTimer -= Time.deltaTime;

                // Interval Timer
                if (firingIntervalTimer < 0f)
                {
                    if (firingDurationTimer >= 0)
                    {
                        firingDurationTimer -= Time.deltaTime;

                        FireWeapon();
                    }
                    else
                    {
                        // Reset timers
                        firingIntervalTimer = WeaponShootInterval();
                        firingDurationTimer = WeaponShootDuration();
                    }
                }
            }
        }
    }

    private void MeleeAttack()
    {
        
    }

    /// <summary>
    /// Calculate a random weapon shoot duration between the min and max values
    /// </summary>
    private float WeaponShootDuration()
    {
        // Calculate a random weapon shoot duration
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }

    /// <summary>
    /// Calculate a random weapon shoot interval between the min and max values
    /// </summary>
    private float WeaponShootInterval()
    {
        // Calculate a random weapon shoot interval
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }

    /// <summary>
    /// Fire the weapon
    /// </summary>
    private void FireWeapon()
    {
        // Player distance
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        // Calculate direction vector of player from weapon shoot position
        Vector3 weaponDirection = (GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position);

        // Get weapon to player angle
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // Get enemy to player angle
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        /*// Set enemy aim direction
        AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

        // Trigger weapon aim event
        enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);*/

        // Get ammo range
        //float enemyAmmoRange = enemyDetails.enemyRangedWeapon.weaponCurrentAmmo.ammoRange;

        /*// Is the player in range
        if (playerDirectionVector.magnitude <= enemyAmmoRange)
        {
            
        }*/

        // Does this enemy require line of sight to the player before firing?
        if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection)) return;

        // Trigger fire weapon event
        enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirection, enemy.enemyDetails.chaseDistance, layerMask);

        if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
    }

#endif
    #endregion Validation
}