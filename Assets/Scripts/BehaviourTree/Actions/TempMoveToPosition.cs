using TheKiwiCoder;
using UnityEngine;

[System.Serializable]
public class TempMoveToPosition : MoveToPosition
{
    protected MovementToPositionEvent movementToPositionEvent;

    protected IdleEvent idleEvent;

    public override void OnInit()
    {
        base.OnInit();

        movementToPositionEvent = context.gameObject.GetComponent<MovementToPositionEvent>();

        idleEvent = context.gameObject.GetComponent<IdleEvent>();
    }

    protected override void OnStart()
    {
        context.agent.stoppingDistance = stoppingDistance.Value;
        context.agent.speed = speed.Value;
        context.agent.destination = targetPosition.Value;
        context.agent.acceleration = acceleration.Value;
        context.agent.isStopped = false;

        Vector2 moveDirection = (targetPosition.Value - context.transform.position).normalized;

        movementToPositionEvent.CallMovementToPositionEvent(
                    targetPosition.Value, context.transform.position, speed.Value, moveDirection);
    }

    protected override void OnStop()
    {
        base.OnStop();

        idleEvent.CallIdleEvent();
    }
}
