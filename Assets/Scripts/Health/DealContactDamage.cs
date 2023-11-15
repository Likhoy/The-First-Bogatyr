using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    #region Header DEAL DAMAGE
    [Space(10)]
    [Header("DEAL DAMAGE")]
    #endregion
    #region Tooltip
    [Tooltip("The contact damage to deal (is overridden by the receiver)")]
    #endregion
    [SerializeField] private int contactDamageAmount = 0;
    #region Tooltip
    [Tooltip("Specify what layers objects should be on to receive contact damage")]
    #endregion
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;
    private MeleeWeapon weapon;

    private void OnEnable()
    {
        if (GetComponent<Enemy>() == null)
        {
            GetComponentInParent<MeleeAttackEvent>().OnMeleeAttack += MeleeAttackEvent_OnMeleeAttack;
            GetComponentInParent<SetActiveWeaponEvent>().OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        }
    }

    private void OnDisable()
    {
        if (GetComponent<Enemy>() == null)
        {
            SetActiveWeaponEvent setActiveWeaponEvent = GetComponentInParent<SetActiveWeaponEvent>();
            if (setActiveWeaponEvent != null)
            {
                GetComponentInParent<MeleeAttackEvent>().OnMeleeAttack -= MeleeAttackEvent_OnMeleeAttack;
                setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
            }
        }   
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        weapon = setActiveWeaponEventArgs.weapon as MeleeWeapon;
    }

    private void MeleeAttackEvent_OnMeleeAttack(MeleeAttackEvent meleeAttackEvent, MeleeAttackEventArgs meleeAttackEventArgs)
    {
        SetContactDamageAmount();
    }

    private void SetContactDamageAmount()
    {
        // Set contact damage amount matching weaponDamage if character is holding a melee weapon
        contactDamageAmount = weapon?.GetWeaponDamage() ?? contactDamageAmount;
    }


    // Trigger contact damage when enter a collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If already colliding with something return
        if (isColliding) return;

        ContactDamage(collision);
    }

    // Trigger contact damage when staying withing a collider
    private void OnTriggerStay2D(Collider2D collision)
    {
        // If already colliding with something return
        if (isColliding) return;

        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        // if the collision object isn't in the specified layer then return (use bitwise comparison)
        int collisionObjectLayerMask = (1 << collision.gameObject.layer);

        if ((layerMask.value & collisionObjectLayerMask) == 0)
            return;

        // Check to see if the colliding object should take contact damage
        ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

        if (receiveContactDamage != null)
        {
            isColliding = true;

            // Reset the contact collision after set time
            Invoke("ResetContactCollision", Settings.contactDamageCollisionResetDelay); // temporary implementation

            receiveContactDamage.TakeContactDamage(contactDamageAmount);

        }

    }

    /// <summary>
    /// Reset the isColliding bool
    /// </summary>
    private void ResetContactCollision()
    {
        isColliding = false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif
    #endregion
}
