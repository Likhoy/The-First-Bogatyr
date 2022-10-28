using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Tooltip

    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]

    #endregion Tooltip
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private float moveSpeed;

    private bool isPlayerMovementDisabled = false;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    void Update()
    {
        
        DialogInput();

        /*// Movement input
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");*/
    }
    public void FixedUpdate()
    {
        /*if (isDialogPlaying) // optional
        {
            
            return; 
        }*/

        // if player movement disabled then return
        if (isPlayerMovementDisabled) 
            return;

        // Process the player movement input
        MovementInput();


        /*//Changing animation
        if (horizontal < 0)//Left
            this.facing = 2;
        if (horizontal > 0)//Right
            this.facing = 3;
        if (vertical > 0)//Up
            this.facing = 0;
        if (vertical < 0)//Down
            this.facing = 1;
        this.AN.SetInteger("state", this.facing);
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        this.RB.velocity = new Vector2(horizontal * this.moveSpeed * Time.timeScale, vertical * this.moveSpeed * Time.timeScale);*/
    }

    /// <summary>
    /// Player movement input
    /// </summary>
    private void MovementInput()
    {
        // Get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Create a direction vector based on the input
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // Adjust distance for diagonal movement (pythagoras approximation)
        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        // If there is movement then move
        if (direction != Vector2.zero)
        {
            // trigger movement event
            player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
        }
        // else trigger idle event
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NPC" && DialogManager.Instance.isDialogReady)
            player.dialogStartedEvent.CallDialogStartedEvent(); // maybe better in NPC class
    }

    private void DialogInput()
    {
        // check for mouse down event - switch dialog text
        if (DialogManager.Instance.isDialogPlaying && Input.GetMouseButtonDown(0) && !DialogManager.Instance.optionButtonsAreBeingDisplayed)
        {
            player.dialogProceededEvent.CallDialogProceedEvent();
        }
    }

    /// <summary>
    /// Enable the player movement
    /// </summary>
    public void EnablePlayer()
    {
        isPlayerMovementDisabled = false;
    }

    /// <summary>
    /// Disable the player movement
    /// </summary>
    public void DisablePlayer()
    {
        isPlayerMovementDisabled = true;
        player.idleEvent.CallIdleEvent();
    }
}
