using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        // Load components
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        // Subscribe to movement event
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        // Subscribe to idle event
        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;

        // Subscribe to weapon aim event
        // enemy.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;

        // Subscribe to melee attack event
        enemy.meleeAttackEvent.OnMeleeAttack += MeleeAttackEvent_OnMeleeAttack;

    }

    private void OnDisable()
    {
        // Unsubscribe from movement event
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // Unsubscribe from idle event
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;

        // Unsubscribe from weapon aim event event
        // enemy.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;

        // Unsubscribe from melee attack event
        enemy.meleeAttackEvent.OnMeleeAttack -= MeleeAttackEvent_OnMeleeAttack;
    }

    /// <summary>
    /// On weapon aim event handler
    /// </summary>
    /*private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitialiseAimAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
    }*/

    /// <summary>
    /// On movement event handler
    /// </summary>
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        InitializeLookAnimationParameters();
        SetMovementAnimationParameters();

        float moveAngle = HelperUtilities.GetAngleFromVector(movementToPositionArgs.moveDirection);
        LookDirection lookDirection = HelperUtilities.GetLookDirection(moveAngle);
        SetLookAnimationParameters(lookDirection);
        //SetMovementToPositionAnimationParameters();
    }

    /// <summary>
    /// On idle event handler
    /// </summary>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }

    /// <summary>
    /// Initialise look animation parameters
    /// </summary>
    private void InitializeLookAnimationParameters()
    {
        enemy.animator.SetBool(Settings.lookUp, false);
        enemy.animator.SetBool(Settings.lookRight, false);
        enemy.animator.SetBool(Settings.lookLeft, false);
        enemy.animator.SetBool(Settings.lookDown, false);
    }

    /// <summary>
    /// Set movement animation parameters
    /// </summary>
    private void SetMovementAnimationParameters()
    {
        // Set Moving
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isMoving, true);
    }


    /// <summary>
    /// Set idle animation parameters
    /// </summary>
    private void SetIdleAnimationParameters()
    {
        // Set idle
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, true);
    }

    /// <summary>
    /// On melee attack event handler
    /// </summary>
    private void MeleeAttackEvent_OnMeleeAttack(MeleeAttackEvent meleeAttackEvent, MeleeAttackEventArgs meleeAttackEventArgs)
    {
        EnemyMeleeAttackAnimation();
    }




    /// <summary>
    /// Plays weapon fire animation
    /// </summary>
    private void EnemyMeleeAttackAnimation()
    {
        if (enemy.animator.GetBool(Settings.lookUp))
            enemy.animator.SetBool(Settings.attackUp, true);
        else if (enemy.animator.GetBool(Settings.lookDown))
            enemy.animator.SetBool(Settings.attackDown, true);
        else if (enemy.animator.GetBool(Settings.lookRight))
            enemy.animator.SetBool(Settings.attackRight, true);
        else if (enemy.animator.GetBool(Settings.lookLeft))
            enemy.animator.SetBool(Settings.attackLeft, true);
    }

    /// <summary>
    /// Set look animation parameters
    /// </summary>
    private void SetLookAnimationParameters(LookDirection lookDirection)
    {
        // Set aim direction
        switch (lookDirection)
        {
            case LookDirection.Up:
                enemy.animator.SetBool(Settings.lookUp, true);
                break;

            case LookDirection.Right:
                enemy.animator.SetBool(Settings.lookRight, true);
                break;

            case LookDirection.Left:
                enemy.animator.SetBool(Settings.lookLeft, true);
                break;

            case LookDirection.Down:
                enemy.animator.SetBool(Settings.lookDown, true);
                break;

        }
    }
}