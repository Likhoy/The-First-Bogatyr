using UnityEngine;

public class AnimateChernobog : MonoBehaviour
{
    private Enemy enemy;

    // private WaitForFixedUpdate waitForFixedUpdate;

    private void Awake()
    {
        // Load components
        enemy = GetComponent<Enemy>();

        //waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        // Subscribe to movement event
        // enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        // Subscribe to fire weapon event
        // enemy.fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;

        // Subscribe to weapon fired event
        // enemy.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        // Subscribe to defending events
        enemy.defendingStageStartedEvent.OnDefendingStageStarted += DefendingStageStartedEvent_OnDefendingStageStarted;
        enemy.defendingStageEndedEvent.OnDefendingStageEnded += DefendingStageEndedEvent_OnDefendingStageEnded;

        enemy.lookAtEvent.OnLookAt += LookAtEvent_OnLookAt;

        // Subscribe to idle event
        // enemy.idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        // Unsubscribe from movement event
        // enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // Unsubscribe from fire weapon event
        // enemy.fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;

        // Unsubscribe from weapon fired event
        // enemy.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        // Unsubscribe from defending events
        enemy.defendingStageStartedEvent.OnDefendingStageStarted -= DefendingStageStartedEvent_OnDefendingStageStarted;
        enemy.defendingStageEndedEvent.OnDefendingStageEnded -= DefendingStageEndedEvent_OnDefendingStageEnded;

        enemy.lookAtEvent.OnLookAt -= LookAtEvent_OnLookAt;

        // Unsubscribe from idle event
        // enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    /// <summary>
    /// On movement event handler
    /// </summary>
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        InitializeLookAnimationParameters();
        SetMovementAnimationParameters();

        float moveAngle = HelperUtilities.GetAngleFromVector(movementToPositionArgs.moveDirection);
        LookDirection lookDirection = HelperUtilities.GetLookDirectionLR(moveAngle);
        SetLookAnimationParameters(lookDirection);
    }

    private void DefendingStageStartedEvent_OnDefendingStageStarted(DefendingStageStartedEvent defendingStageStartedEvent, DefendingStageStartedEventArgs defendingStageStartedEventArgs)
    {
        TriggerDefendingAnimation();
    }

    private void TriggerDefendingAnimation()
    {
        enemy.animator.SetBool("isDefending", true);
        enemy.animator.ResetTrigger("attackTrigger");
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isMoving, false);
    }

    private void StopDefendingAnimation()
    {
        enemy.animator.SetBool("isDefending", false);
    }

    /*private IEnumerator LookDirectionWatchRoutine()
    {
        Player player = GameManager.Instance.GetPlayer();
        while (true)
        {
            enemy.animator.SetBool(Settings.isMoving, false);
            InitializeLookAnimationParameters();
            if (player != null)
            {
                float angleToPlayer = HelperUtilities.GetAngleFromVector((player.transform.position - transform.position).normalized);
                LookDirection lookDirection = HelperUtilities.GetLookDirectionLR(angleToPlayer);
                SetLookAnimationParameters(lookDirection);
            }
            yield return waitForFixedUpdate;
        }
    }*/

    private void DefendingStageEndedEvent_OnDefendingStageEnded(DefendingStageEndedEvent defendingStageEndedEvent, DefendingStageEndedEventArgs defendingStageEndedEventArgs)
    {
        StopDefendingAnimation();
    }

    private void LookAtEvent_OnLookAt(LookAtEvent staticAttackingStartedEvent, StaticAttackingStartedEventArgs args)
    {
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, false);
        
        InitializeLookAnimationParameters();
        float angleToPlayer = HelperUtilities.GetAngleFromVector((args.playerPosition - transform.position).normalized);
        LookDirection lookDirection = HelperUtilities.GetLookDirectionLR(angleToPlayer);
        SetLookAnimationParameters(lookDirection);
    }

    

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        TriggerAttackAnimation();
    }

    private void TriggerAttackAnimation()
    {
        enemy.animator.SetTrigger("attackTrigger");
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        enemy.animator.ResetTrigger("attackTrigger");
    }

    /// <summary>
    /// On idle event handler
    /// </summary>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
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
    /// Initialise look animation parameters
    /// </summary>
    private void InitializeLookAnimationParameters()
    {
        enemy.animator.SetBool(Settings.lookRight, false);
        enemy.animator.SetBool(Settings.lookLeft, false);
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
    /// Set look animation parameters
    /// </summary>
    private void SetLookAnimationParameters(LookDirection lookDirection)
    {
        // Set aim direction
        switch (lookDirection)
        {
            case LookDirection.Right:
                enemy.animator.SetBool(Settings.lookRight, true);
                break;

            case LookDirection.Left:
                enemy.animator.SetBool(Settings.lookLeft, true); 
                break;
        }
    }
}
