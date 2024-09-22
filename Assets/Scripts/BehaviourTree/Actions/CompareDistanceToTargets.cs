using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CompareDistanceToTargets : ActionNode
{
    [SerializeField] private NodeProperty<Vector3> target1Position;

    [SerializeField] private NodeProperty<Vector3> target2Position;

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (Vector3.Distance(target1Position.Value, context.transform.position) < Vector3.Distance(target2Position.Value, context.transform.position))
            return State.Success;

        return State.Failure;
    }
}
