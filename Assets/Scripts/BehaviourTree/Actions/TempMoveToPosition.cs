using TheKiwiCoder;
using Unity.VisualScripting;

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

        movementToPositionEvent.CallMovementToPositionEvent(
                    targetPosition.Value, context.transform.position, speed.Value, (targetPosition.Value - context.transform.position).normalized);
    }

    protected override void OnStop()
    {
        base.OnStop();

        idleEvent.CallIdleEvent();
    }
}
