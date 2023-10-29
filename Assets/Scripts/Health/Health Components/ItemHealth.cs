using UnityEngine;

public class ItemHealth : Health
{
    private DestroyableItem destroyableItem;

    protected override void Start()
    {
        base.Start();
        destroyableItem = GetComponent<DestroyableItem>();

        hasHitEffect = true;
        effectTime = destroyableItem.effectTime;
        spriteRenderer = destroyableItem.GetComponent<SpriteRenderer>();
    }
}
