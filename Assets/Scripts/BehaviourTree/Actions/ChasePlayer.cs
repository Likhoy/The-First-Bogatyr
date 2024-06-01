using TheKiwiCoder;
using UnityEngine;

[System.Serializable]
public class ChasePlayer : MoveToPosition
{
    private MovementToPositionEvent movementToPositionEvent;

    private float pathRebuildCooldown = Settings.enemyPathRebuildCooldown;

    public override void OnInit()
    {
        base.OnInit();

        movementToPositionEvent = context.gameObject.GetComponent<MovementToPositionEvent>();
    }

    protected override void OnStart()
    {
        context.agent.stoppingDistance = stoppingDistance.Value;
        context.agent.speed = speed.Value;
        context.agent.destination = GameManager.Instance.GetPlayer().GetPlayerPosition();
        context.agent.acceleration = acceleration.Value;
        context.agent.isStopped = false;

        movementToPositionEvent.CallMovementToPositionEvent(
                    context.agent.destination, context.transform.position, speed.Value, (context.agent.destination - context.transform.position).normalized);
    }

    protected override State OnUpdate()
    {
        pathRebuildCooldown -= Time.deltaTime;

        if (pathRebuildCooldown < 0)
        {
            pathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
            context.agent.SetDestination(playerPosition);
        }

        movementToPositionEvent.CallMovementToPositionEvent(
                    context.agent.destination, context.transform.position, speed.Value, (context.agent.destination - context.transform.position).normalized);

        return base.OnUpdate();
    }
}
