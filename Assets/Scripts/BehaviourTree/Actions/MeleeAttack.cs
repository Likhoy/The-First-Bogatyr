using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class MeleeAttack : ActionNode
{
    [SerializeField] private NodeProperty<Vector3> targetPosition;
    
    private MeleeAttackEvent meleeAttackEvent;

    [SerializeField] private MeleeWeaponDetailsSO weaponDetails;

    private MeleeWeapon weapon;

    private float meleeWeaponCooldownTime = 0f;

    private float previousAttackTime = 0f;

    public override void OnInit()
    {
        base.OnInit();

        meleeAttackEvent = context.gameObject.GetComponent<MeleeAttackEvent>();

        weapon = new MeleeWeapon()
        {
            weaponDetails = this.weaponDetails,
            weaponCurrentMinDamage = weaponDetails.GetWeaponMinDamage(),
            weaponCurrentMaxDamage = weaponDetails.GetWeaponMaxDamage(),
            weaponListPosition = 1
        };
    }

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate() 
    {
        if (Time.time - previousAttackTime >= meleeWeaponCooldownTime)
        {
            Attack();
            previousAttackTime = Time.time;
        }

        return State.Success;
    }

    private void Attack()
    {
        //audioEffects.PlayOneShot(CMeleeAttack);
        meleeAttackEvent.CallMeleeAttackEvent(targetPosition.Value);
        meleeWeaponCooldownTime = weapon.weaponDetails.weaponCooldownTime;
    }
}
