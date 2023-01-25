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
    private Coroutine playerDashCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private float playerDashCooldownTimer = 0f;
    private bool isPlayerMovementDisabled = false;

    [HideInInspector] public bool isPlayerDashing = false;

    private float timeBetweenAttack;
    public float startTimeBetweenAttack;
    public Transform attackPose;
    public float attackRange;
    private LayerMask enemy;
    public int damageAmount;


    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        // create waitForFixedUpdate for use in corountine
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    void Update()
    {

        // Process the player dialog input
        ProcessDialogInput();

        // if player movement disabled then return
        if (isPlayerMovementDisabled)
            return;

        //if Player is dashing then return
        if (isPlayerDashing)
            return;

        // Process the player movement input
        ProcessMovementInput();

        //player dash cooldown timer
        PlayerDashCooldownTimer();
    }

    /// <summary>
    /// Player movement input
    /// </summary>
    private void ProcessMovementInput()
    {
        // Get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

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
            if (!rightMouseButtonDown)
            {
                // trigger movement event
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            // else player dash if not cooling down
            else if (playerDashCooldownTimer <= 0f)
            {
                PlayerDash((Vector3)direction);
            }
        }
        // else trigger idle event
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    /// <summary>
    /// Player dash
    /// </summary>
    private void PlayerDash(Vector3 direction)
    {
        playerDashCoroutine = StartCoroutine(PlayerDashRoutine(direction));
    }

    /// <summary>
    /// Player dash corountine
    /// </summary>
    private IEnumerator PlayerDashRoutine(Vector3 directon)
    {
        // minDistance used to decide when to exit coroutine loop
        float minDistance = 0.2f;
        isPlayerDashing = true;
        Vector3 targetPosition = player.transform.position + directon * movementDetails.dashDistance;

        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.dashSpeed, directon, isPlayerDashing);

            // yield and wait for fixed update
            yield return waitForFixedUpdate;
        }
        isPlayerDashing = false;
        playerDashCooldownTimer = movementDetails.dashCooldownTime;
        player.transform.position = targetPosition;
    }

    private void PlayerDashCooldownTimer()
    {
        if (playerDashCooldownTimer >= 0f)
        {
            playerDashCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collided with something stop player dash coroutine
        StopPlayerDashRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //if in collided with something stop player dash coroutine
        StopPlayerDashRoutine();
    }

    private void StopPlayerDashRoutine()
    {
        if (playerDashCoroutine != null)
        {
            StopCoroutine(playerDashCoroutine);
            isPlayerDashing = false;
        }
    }

    private void ProcessDialogInput()
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

    private void WeaponInput()
    {
        if(timeBetweenAttack <= 0)
        {
            if(Input.GetKey(KeyCode.Space))
            {
               Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPose.position, attackRange, enemy);
                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemiesToDamage[i].GetComponent<Health>().TakeDamage(damageAmount);
                } 
            }
            
            
            timeBetweenAttack = startTimeBetweenAttack;
        }

        else
        {
            timeBetweenAttack -= Time.deltaTime;
        }
    }
}
