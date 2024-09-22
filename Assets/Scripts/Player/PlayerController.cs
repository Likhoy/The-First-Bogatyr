using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Tooltip

    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]

    #endregion Tooltip
    [SerializeField] private MovementDetailsSO movementDetails;
    #region Tooltip 

    [Tooltip("The player WeaponShootPosition gameobject in the hieracrchy")]

    #endregion Tooltip
    [SerializeField] private Transform weaponShootPosition;

    [HideInInspector] public List<Item> takeItemList;
    [HideInInspector] public bool isTaking;
    private Player player;
    private float moveSpeed;
    private Coroutine playerDashCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private float playerDashCooldownTimer = 0f;
    private bool isPlayerMovementDisabled = false;

    [HideInInspector] public bool isPlayerDashing = false;

    private float timeBetweenAttack = 0f;

    private AudioSource audioSource;
    private AudioSource audioEffects;
    [SerializeField]
    AudioClip CAttack;

    private Vector3 before;
    private Vector3 after;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
        audioSource.volume = 0.5f;

        // create waitForFixedUpdate for use in corountine
        waitForFixedUpdate = new WaitForFixedUpdate();

        takeItemList = new List<Item>();
        isTaking = false;
    }

    void Update()
    {
        after = transform.position;
        
        // if player movement disabled then return
        if (isPlayerMovementDisabled)
            return;

        // if player is dashing then return
        if (isPlayerDashing)
            return;

        // Process the player movement input
        ProcessMovementInput();

        // Process the player weapon input
        ProcessWeaponInput();

        // player dash cooldown timer
        PlayerDashCooldownTimer();

        // player weapon cooldown timer
        PlayerWeaponCooldownTimer();

        // collecting items by the player controller
        TakeItem();

        before = after;
    }

    private void TakeItem()
    {
        if (Input.GetKeyDown(Settings.commandButtons[Command.TakeItem]))
            if (takeItemList.Count > 0)
            {
                System.Random r = new System.Random();
                Item item = takeItemList[r.Next(takeItemList.Count)];
                takeItemList.Remove(item);
                item.TakeItem();
            }
    }

    /// <summary>
    /// Player movement input
    /// </summary>
    private void ProcessMovementInput()
    {
        // Get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool dashButtonPressed = Input.GetKey(Settings.commandButtons[Command.Dash]);

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
            if (!dashButtonPressed)
            {
                // trigger movement event
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
                if (!audioSource.isPlaying && before != after)
                    audioSource.Play();
            }
            // else player dash if not cooling down
            else if (playerDashCooldownTimer <= 0f)
            {
                audioSource.Stop();
                PlayerDash((Vector3)direction);
            }

            if (before == after)
                audioSource.Stop();
        }
        // else trigger idle event
        else
        {
            audioSource.Stop();
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
        // if collided with something stop player dash coroutine
        StopPlayerDashRoutine(true);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // if in collided with something stop player dash coroutine
        StopPlayerDashRoutine(true);
    }

    private void StopPlayerDashRoutine(bool timerResetNeeded)
    {
        if (playerDashCoroutine != null)
        {
            StopCoroutine(playerDashCoroutine);
            isPlayerDashing = false;
            if (timerResetNeeded)
                playerDashCooldownTimer = movementDetails.dashCooldownTime;
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
        StopPlayerDashRoutine(true);
        player.idleEvent.CallIdleEvent();
    }

    private void ProcessWeaponInput()
    {
        // Hitting someone or something :)
        if (Input.GetKey(Settings.commandButtons[Command.Hit]))
        {
            if (player.activeWeapon.GetCurrentWeapon() is MeleeWeapon meleeWeapon)
            {
                if (timeBetweenAttack <= 0)
                {
                    audioEffects.PlayOneShot(CAttack, 1f);
                    player.meleeAttackEvent.CallMeleeAttackEvent(Vector3.zero);
                    timeBetweenAttack = meleeWeapon.weaponDetails.weaponCooldownTime;
                }
            }
            else
            {
                RangedWeapon rangedWeapon = player.activeWeapon.GetCurrentWeapon() as RangedWeapon;
                Vector3 targetPosition = HelperUtilities.GetMouseWorldPosition();

                // Target distance
                Vector3 targetDirectionVector = targetPosition - transform.position;

                // Calculate direction vector of target from weapon shoot position
                Vector3 weaponDirection = (targetPosition - weaponShootPosition.position);

                // Get weapon to target angle
                float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

                // Get player to target angle
                float targetAngleDegrees = HelperUtilities.GetAngleFromVector(targetDirectionVector);

                player.fireWeaponEvent.CallFireWeaponEvent(true, true, targetAngleDegrees, weaponAngleDegrees, weaponDirection, targetPosition.x, targetPosition.y);
            }
        }
        // Switching weapon
        else if (Input.GetKeyDown(Settings.commandButtons[Command.SwitchWeapon]))
        {
            if (player.weaponList.Count == 0 || player.weaponList.Count == 1)
                return;
            Weapon nextWeapon = player.GetNextWeaponAfterCurrent();
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(nextWeapon, nextWeapon is RangedWeapon);
        }
    }

    private void PlayerWeaponCooldownTimer()
    {
        if (timeBetweenAttack >= 0f)
        {
            timeBetweenAttack -= Time.deltaTime;
        }
    }

    private void DealWithMeleeWeaponStrikedEvent()
    {
        //EnablePlayer();
        player.weaponFiredEvent.CallWeaponFiredEvent(player.activeWeapon.GetCurrentWeapon());
    }
}
