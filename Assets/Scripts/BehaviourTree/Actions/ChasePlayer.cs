using TheKiwiCoder;
using UnityEngine;

[System.Serializable]
public class ChasePlayer : MoveToPosition
{
    private MovementToPositionEvent movementToPositionEvent;

    private float pathRebuildCooldown = 0f;

    public override void OnInit()
    {
        base.OnInit();

        movementToPositionEvent = context.gameObject.GetComponent<MovementToPositionEvent>();

        context.agent.stoppingDistance = stoppingDistance.Value;
        context.agent.speed = speed.Value;
        context.agent.acceleration = acceleration.Value;
        
    }

    protected override void OnStart() { }

    protected override State OnUpdate()
    {
        pathRebuildCooldown -= Time.deltaTime;

        context.agent.isStopped = false;

        if (pathRebuildCooldown < 0)
        {
            pathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
            context.agent.SetDestination(playerPosition);
        }

        Vector2 moveDirection = (context.agent.destination - context.transform.position).normalized;

        movementToPositionEvent.CallMovementToPositionEvent(
                    context.agent.destination, context.transform.position, speed.Value, moveDirection);

        return base.OnUpdate();
    }
}
