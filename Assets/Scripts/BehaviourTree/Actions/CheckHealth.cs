using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckHealth : ActionNode
{
    [SerializeField] private int[] healthPercentsToCheck;

    private int currentCheck = 0;

    private Health health;

    public override void OnInit()
    {
        base.OnInit();

        health = context.gameObject.GetComponent<Health>();
    }

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        float currentHealthPercent = (float)health.currentHealth / health.GetInitialHealth() * 100;

        if (currentCheck < healthPercentsToCheck.Length && currentHealthPercent < healthPercentsToCheck[currentCheck])
        {
            currentCheck++;
            return State.Success;
        }

        return State.Failure;
    }
}
