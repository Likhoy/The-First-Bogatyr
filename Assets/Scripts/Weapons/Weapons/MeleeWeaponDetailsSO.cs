using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Melee Weapon Details")]
public class MeleeWeaponDetailsSO : WeaponDetailsSO
{
    #region Tooltip
    [Tooltip("Populate with weapon min damage per hit")]
    #endregion Tooltip
    public int weaponMinDamage;
    #region Tooltip
    [Tooltip("Populate with weapon max damage per hit")]
    #endregion Tooltip
    public int weaponMaxDamage;
    #region Tooltip
    [Tooltip("Select weapon cooldown time")]
    #endregion Tooltip
    public float weaponCooldownTime;

    public override int GetWeaponMinDamage()
    {
        return weaponMinDamage;
    }

    public override int GetWeaponMaxDamage()
    {
        return weaponMaxDamage;
    }
    

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);

    }

#endif
    #endregion Validation
}

