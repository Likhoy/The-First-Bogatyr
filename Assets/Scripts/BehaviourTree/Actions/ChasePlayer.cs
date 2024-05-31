using TheKiwiCoder;
using UnityEngine;

[System.Serializable]
public class ChasePlayer : TempMoveToPosition
{
    public NodeProperty<float> distance;

    private float pathRebuildCooldown = Settings.enemyPathRebuildCooldown;

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

    protected override void OnStop()
    {
        if (context.agent.pathPending)
        {
            context.agent.ResetPath();
        }

        if (context.agent.remainingDistance > tolerance.Value)
        {
            context.agent.isStopped = true;
        }
    }
}
