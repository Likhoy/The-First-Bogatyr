using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class Fire : ActionNode
{
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private FireWeaponEvent fireWeaponEvent;

    [SerializeField] private EnemyDetailsSO enemyDetails;
    [SerializeField] private RangedWeaponDetailsSO weaponDetails;
    private Transform weaponShootPosition;
    #region Tooltip
    [Tooltip("Select the layers that the enemy bullets will hit")]
    #endregion Tooltip
    [SerializeField] private LayerMask layerMask;

    private RangedWeapon weapon;

    // кулдаун между выстрелами
    private float firingIntervalTime = 0;
    
    // максимальное время выстрелов подряд
    private float firingDurationTime = float.MaxValue;
    
    private float previousFireTime = 0;

    public override void OnInit()
    {
        base.OnInit();

        weaponShootPosition = context.transform.Find("WeaponAnchorPosition/WeaponShootPosition");

        setActiveWeaponEvent = context.gameObject.GetComponent<SetActiveWeaponEvent>();
        fireWeaponEvent = context.gameObject.GetComponent<FireWeaponEvent>();

        weapon = new RangedWeapon()
        {
            weaponDetails = this.weaponDetails,
            weaponCurrentMinDamage = weaponDetails.GetWeaponMinDamage(),
            weaponCurrentMaxDamage = weaponDetails.GetWeaponMaxDamage(),
            weaponReloadTimer = 0f,
            weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity,
            weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity,
            isWeaponReloading = false
        };

    }

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate() 
    {

        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon, true);

        // Interval Timer
        if (Time.time - previousFireTime > firingIntervalTime)
        {
            if (Time.time - previousFireTime <= firingDurationTime)
            {
                FireWeapon();
                previousFireTime = Time.time;
            }
            else
            {
                // Reset timers
                firingIntervalTime = WeaponShootInterval();
                firingDurationTime = WeaponShootDuration();
            }
        }

        return State.Success;
    }

    private float WeaponShootInterval()
    {
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }

    private float WeaponShootDuration()
    {
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }

    private void FireWeapon()
    {
        // Player distance
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - context.transform.position;

        // Calculate direction vector of player from weapon shoot position
        Vector3 weaponDirection = (GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position);

        // Get weapon to player angle
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // Get enemy to player angle
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        // Does this enemy require line of sight to the player before firing?
        if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection)) return;

        // Trigger fire weapon event
        fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirection, enemyDetails.chaseDistance, layerMask);

        if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;
    }
}
