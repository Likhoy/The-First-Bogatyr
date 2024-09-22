using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class LookAtPlayer : ActionNode
{
    private LookAtEvent lookAtEvent;

    public override void OnInit()
    {
        base.OnInit();

        lookAtEvent = context.gameObject.GetComponent<LookAtEvent>();
    }

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate() 
    {

        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        lookAtEvent.CallLookAtEvent(playerPosition);

        return State.Success;
    }
}
