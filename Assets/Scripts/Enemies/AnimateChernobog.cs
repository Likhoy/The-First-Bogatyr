using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimateChernobog : MonoBehaviour
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
    }

    private void OnDisable()
    {
        // Unsubscribe from movement event
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        // Unsubscribe from idle event
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
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
        //enemy.animator.SetBool(Settings.lookLeft, false);
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
                if (enemy.transform.localScale.x < 0f)
                    enemy.transform.localScale = new Vector3(-enemy.transform.localScale.x, enemy.transform.localScale.y, enemy.transform.localScale.z);
                break;

            case LookDirection.Left:
                enemy.animator.SetBool(Settings.lookRight, true); // new way of direction changing using scale
                if (enemy.transform.localScale.x > 0f)
                    enemy.transform.localScale = new Vector3(-enemy.transform.localScale.x, enemy.transform.localScale.y, enemy.transform.localScale.z);
                break;
        }
    }
}
