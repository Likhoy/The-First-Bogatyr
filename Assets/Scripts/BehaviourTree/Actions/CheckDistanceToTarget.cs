using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckDistanceToTarget : ActionNode
{
    [SerializeField] private NodeProperty<float> distance;

    [SerializeField] private NodeProperty<Vector3> targetPosition;

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {

        if (Vector3.Distance(targetPosition.Value, context.transform.position) < distance.Value)
            return State.Success;

        return State.Failure;
    }
}
