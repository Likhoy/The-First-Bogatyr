using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckDistanceToPlayer : ActionNode
{
    [SerializeField] private NodeProperty<float> distance;

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate() 
    {

        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        if (Vector3.Distance(playerPosition, context.transform.position) < distance.Value)
            return State.Success;
        
        return State.Failure;
    }
}
