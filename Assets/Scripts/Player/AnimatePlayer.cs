using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        // Subscribe to movement by velocity event
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;

        // Subscribe to movement to position event
        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        // Subscribe to idle event
        player.idleEvent.OnIdle += IdleEvent_OnIdle;

        // Subscribe to set active weapon event
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        // TODO: add setting weapon inactive reaction

        // Subscribe to fire weapon event
        player.fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;

        // Subscribe to weapon fired event
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
    }

    private void OnDisable()
    {
        // Unsubscribe from movement by velocity event
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;

        // Unsubscribe from movement to position event
        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // Unsubscribe from idle event
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;

        // Unsubscribe from set active weapon event
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        // Unsubscribe from fire weapon event
        player.fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;

        // Unsubscribe from weapon fired event
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
    }

    /// <summary>
    /// On movement by velocity event handler
    /// </summary>
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        // InitializeDashAnimationParameters();
        InitializeLookAnimationParameters();
        SetMovementAnimationParameters();

        float moveAngle = HelperUtilities.GetAngleFromVector(movementByVelocityArgs.moveDirection);
        LookDirection lookDirection = HelperUtilities.GetLookDirection(moveAngle);
        SetLookAnimationParameters(lookDirection);
    }

    /// <summary>
    /// On movement to position event handler
    /// </summary>
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        InitializeLookAnimationParameters();
        SetMovementAnimationParameters();

        float moveAngle = HelperUtilities.GetAngleFromVector(movementToPositionArgs.moveDirection);
        LookDirection lookDirection = HelperUtilities.GetLookDirection(moveAngle);
        SetLookAnimationParameters(lookDirection);
        //SetMovementToPositionAnimationParameters(movementToPositionArgs);
    }

    /// <summary>
    /// On idle event handler
    /// </summary>
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }

    /// <summary>
    /// On set active weapon event handler
    /// </summary>
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetHoldingWeaponAnimationParameters();
    }

    /// <summary>
    /// On fire weapon event handler
    /// </summary>
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        PlayWeaponFireAnimation();
    }

    /// <summary>
    /// On weapon fired event handler
    /// </summary>
    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        InitializeAttackAnimationParameters();
    }

    /// <summary>
    /// Set holding weapon animation parameters (player now is holding a weapon)
    /// </summary>
    private void SetHoldingWeaponAnimationParameters()
    {
        player.animator.SetBool(Settings.holdsWeapon, true);
    }

    private void InitializeAttackAnimationParameters()
    {
        player.animator.SetBool(Settings.attackUp, false);
        player.animator.SetBool(Settings.attackDown, false);
        player.animator.SetBool(Settings.attackRight, false);
        player.animator.SetBool(Settings.attackLeft, false);
    }

    /// <summary>
    /// Plays weapon fire animation
    /// </summary>
    private void PlayWeaponFireAnimation()
    {
        if (player.animator.GetBool(Settings.lookUp))
            player.animator.SetBool(Settings.attackUp, true);
        else if (player.animator.GetBool(Settings.lookDown))
            player.animator.SetBool(Settings.attackDown, true);
        else if (player.animator.GetBool(Settings.lookRight))
            player.animator.SetBool(Settings.attackRight, true);
        else if (player.animator.GetBool(Settings.lookLeft))
            player.animator.SetBool(Settings.attackLeft, true);
    }

    /// <summary>
    /// Initialise look animation parameters
    /// </summary>
    private void InitializeLookAnimationParameters()
    {
        player.animator.SetBool(Settings.lookUp, false);
        player.animator.SetBool(Settings.lookRight, false);
        player.animator.SetBool(Settings.lookLeft, false);
        player.animator.SetBool(Settings.lookDown, false);
    }

    /*private void InitializeDashAnimationParameters()
    {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollLeft, false);
        player.animator.SetBool(Settings.rollUp, false);
    }*/


    /// <summary>
    /// Set movement animation parameters
    /// </summary>
    private void SetMovementAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    /// <summary>
    /// Set movement to position animation parameters
    /// </summary>
    /*private void SetMovementToPositionAnimationParameters(MovementToPositionArgs movementToPositionArgs)
    {
        // Animate roll
        if (movementToPositionArgs.isRolling)
        {
            if (movementToPositionArgs.moveDirection.x > 0f)
            {
                player.animator.SetBool(Settings.rollRight, true);
            }
            else if (movementToPositionArgs.moveDirection.x < 0f)
            {
                player.animator.SetBool(Settings.rollLeft, true);
            }
            else if (movementToPositionArgs.moveDirection.y > 0f)
            {
                player.animator.SetBool(Settings.rollUp, true);
            }
            else if (movementToPositionArgs.moveDirection.y < 0f)
            {
                player.animator.SetBool(Settings.rollDown, true);
            }
        }
    }*/

    /// <summary>
    /// Set movement animation parameters
    /// </summary>
    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
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
                player.animator.SetBool(Settings.lookUp, true);
                break;

            case LookDirection.Right:
                player.animator.SetBool(Settings.lookRight, true);
                break;

            case LookDirection.Left:
                player.animator.SetBool(Settings.lookLeft, true);
                break;

            case LookDirection.Down:
                player.animator.SetBool(Settings.lookDown, true);
                break;

        }

    }

}

