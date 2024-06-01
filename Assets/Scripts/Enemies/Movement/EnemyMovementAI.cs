using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]

public class EnemyMovementAI : BaseEnemyMovementAI // maybe should use strategy pattern instead of inheritance !
{
    private Vector3 randomPosition; // for choosing patroling path
    private bool isSetTargetPoint = false; // patroling path has been chosen

    private Vector3 cellMidPoint; // needed to adjust enemy target point when patroling

    private bool costil = true;

    protected override void Start()
    {
        base.Start();

        // Create waitforfixed update for use in coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();

        cellMidPoint = new Vector3(MainLocationInfo.Grid.cellSize.x * 0.5f, MainLocationInfo.Grid.cellSize.y * 0.5f, 0f);

        SetRandomTargetPoint();
    }

    /// <summary>
    /// Handle enemy movement, while enemy is alive
    /// </summary>
    protected override void MoveEnemy()
    {
        Player player = GameManager.Instance.GetPlayer();
        if (player != null)
        {
            Vector3 playerPosition = player.GetPlayerPosition();

            // Check distance to player to see if enemy should start attacking for the first time
            if (!chasePlayer && Vector3.Distance(transform.position, playerPosition) < enemy.enemyDetails.aggressionDistance)
            {
                // Check if player is in sight area 
                // if (EnemyVisionAI.PlayerIsInSightArea())
                ChasePlayer();
                chasePlayer = true;

                if (costil && gameObject.tag == "Chernobog")
                {
                    GameObject.Find("AudioManager").GetComponent<BossFightMusic>().SetBossFightMusic();
                    costil = false;
                }
            }
            // Check distance to player to see if enemy should carry on chasing
            else if (chasePlayer && Vector3.Distance(transform.position, playerPosition) < enemy.enemyDetails.chaseDistance)
            {
                ChasePlayer();
            }
            // otherwise patrol the area
            else
            {
                if (chasePlayer)
                {
                    SetRandomTargetPoint();
                    if (moveEnemyRoutine != null)
                        StopCoroutine(moveEnemyRoutine);
                    chasePlayer = false;
                }
                PatrolTheArea();
            }
        } 
    }

    /// <summary>
    /// Patrol specific area to find their player - if enemy is outside this area and it isn't chasing the player return to area
    /// </summary>
    private void PatrolTheArea()
    {
        if (isSetTargetPoint && pathRebuildNeeded)
        {
            CreatePath(randomPosition);

            // If a path has been found move the enemy
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    // Trigger idle event
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                // Move enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));

                pathRebuildNeeded = false;
            }
        }   
        else if (!isSetTargetPoint)
            enemy.idleEvent.CallIdleEvent();
        if (isSetTargetPoint && Vector2.Distance(transform.position, randomPosition) < 0.2f)
        {
            isSetTargetPoint = false;
            pathRebuildNeeded = true;
            Invoke(nameof(SetRandomTargetPoint), 3);
        }
    }


    private void SetRandomTargetPoint()
    {
        do {
            randomPosition = new Vector3(Random.Range(movementDetails.patrolingAreaLeftBottom.x, movementDetails.patrolingAreaRightTop.x), 
                Random.Range(movementDetails.patrolingAreaLeftBottom.y, movementDetails.patrolingAreaRightTop.y), 0); // рандомный выбор позиции
            randomPosition = MainLocationInfo.Grid.CellToWorld(GetNearestNonObstaclePosition(randomPosition)) + cellMidPoint;
        } 
        while (Vector2.Distance(transform.position, randomPosition) < 3f || randomPosition == Vector3Int.zero);
        
        isSetTargetPoint = true;
    }

}
