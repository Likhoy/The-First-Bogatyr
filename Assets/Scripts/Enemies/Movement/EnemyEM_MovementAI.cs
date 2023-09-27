using UnityEngine;

public class EnemyEM_MovementAI : BaseEnemyMovementAI
{
    private bool pathRebuildNeeded = true;
    private Vector3 gatewayPosition;

    private void Start()
    {
        // Reset player reference position
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        gatewayPosition = Settings.gatewayPosition;
    }

    protected override void MoveEnemy()
    {
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        float distanceToPlayer = Vector3.Distance(transform.position, playerReferencePosition);
        if (distanceToPlayer < Vector3.Distance(transform.position, gatewayPosition) && distanceToPlayer < enemy.enemyDetails.aggressionDistance)
        {
            chasePlayer = true;
            ChasePlayer();
            pathRebuildNeeded = true;
        }
        else
        {
            chasePlayer = false;
            MoveTowardsGateway();
        }
    }

    private void MoveTowardsGateway()
    {
        if (pathRebuildNeeded)
        {
            CreatePath(gatewayPosition);

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
    }
}
