using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(DialogStartedEvent))]
[RequireComponent(typeof(DialogProceededEvent))]
[RequireComponent(typeof(PlayerController))]
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public DialogStartedEvent dialogStartedEvent;
    [HideInInspector] public DialogProceededEvent dialogProceededEvent;
    [HideInInspector] public PlayerController playerControl;

    private void Awake()
    {
        // Load components
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        idleEvent = GetComponent<IdleEvent>();
        dialogStartedEvent = GetComponent<DialogStartedEvent>();
        dialogProceededEvent = GetComponent<DialogProceededEvent>();
        playerControl = GetComponent<PlayerController>();
    }

    /// <summary>
    /// Initialize the player
    /// </summary>
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        //Create player starting weapons
        // CreatePlayerStartingWeapons();

        // Set player starting health
        // SetPlayerHealth();
    }

    /// <summary>
    /// Returns the player position
    /// </summary>
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
}

