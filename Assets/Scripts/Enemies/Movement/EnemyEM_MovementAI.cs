using UnityEngine;

public class EnemyEM_MovementAI : BaseEnemyMovementAI
{
    private bool pathRebuildNeeded = true;
    private Vector3 gatewayPosition;

    protected override void Start()
    {
        base.Start();

        gatewayPosition = Settings.gatewayPosition;
    }

    protected override void MoveEnemy()
    {
        Vector2 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        
        if (distanceToPlayer < Vector3.Distance(transform.position, gatewayPosition) && distanceToPlayer < enemy.enemyDetails.chaseDistance)
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
