using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the SpriteRenderer on the child Weapon gameobject")]
    #endregion
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    #region Tooltip
    [Tooltip("Populate with the PolygonCollider2D on the child Weapon gameobject")]
    #endregion
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
    #region Tooltip
    [Tooltip("Populate with the Transform on the WeaponShootPosition gameobject")]
    #endregion
    [SerializeField] private Transform weaponShootPositionTransform;
    /*#region Tooltip
    [Tooltip("Populate with the Transform on the WeaponEffectPosition gameobject")]
    #endregion
    [SerializeField] private Transform weaponEffectPositionTransform;*/

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;


    private void Awake()
    {
        // Load components
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (setActiveWeaponEventArgs.weapon != null)
            SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        if (currentWeapon is RangedWeapon rangedWeapon)
        {
            // Set current weapon sprite
            if (weaponSpriteRenderer != null)
                weaponSpriteRenderer.sprite = rangedWeapon.weaponDetails.weaponSprite;

            // Set weapon shoot position
            weaponShootPositionTransform.localPosition = rangedWeapon.weaponDetails.weaponShootPosition;
        }
        else if (currentWeapon is MeleeWeapon meleeWeapon)
        {
            // Set current weapon sprite
            if (weaponSpriteRenderer != null)
                weaponSpriteRenderer.sprite = meleeWeapon.weaponDetails.weaponSprite;
            // ...

        }


        // If the weapon has a polygon collider and a sprite then set it to the weapon sprite physics shape
        if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            // Get sprite physics shape - this returns the sprite physics shape points as a list of Vector2s
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            // Set polygon collider on weapon to pick up physics shap for sprite - set collider points to sprite physics shape points
            weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();
        }
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        if (currentWeapon is RangedWeapon rangedWeapon)
            return rangedWeapon.weaponDetails.weaponCurrentAmmo;
        else
            return null;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    /*public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }*/

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        // HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        // HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        // HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
    #endregion

}
