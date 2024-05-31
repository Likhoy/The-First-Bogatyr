using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SetPlayerPosition : ActionNode
{

    public NodeProperty<Vector3> playerPosition;

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate() 
    {
        playerPosition.Value = GameManager.Instance.GetPlayer().GetPlayerPosition();

        return State.Success;
    }
}
