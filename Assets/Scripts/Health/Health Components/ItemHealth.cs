public class ItemHealth : Health
{

    protected override void Start()
    {
        base.Start();

        hasHitEffect = true;
    }
}
