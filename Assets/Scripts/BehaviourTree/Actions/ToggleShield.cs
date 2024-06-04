using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class ToggleShield : ActionNode
{
    private PolygonCollider2D polygonCollider;

    [SerializeField] private bool enable;

    public override void OnInit()
    {
        base.OnInit();

        polygonCollider = context.gameObject.GetComponent<PolygonCollider2D>();
    }

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate() 
    {
        polygonCollider.enabled = enable;

        return State.Success;
    }
}
