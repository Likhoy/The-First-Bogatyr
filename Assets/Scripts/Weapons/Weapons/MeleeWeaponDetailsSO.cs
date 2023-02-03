using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Melee Weapon Details")]
public class MeleeWeaponDetailsSO : WeaponDetailsSO
{
    #region Tooltip
    [Tooltip("Populate with weapon damage per hit")]
    #endregion Tooltip
    public int weaponDamage;
    #region Tooltip
    [Tooltip("Populate with weapon strike animation time")]
    #endregion Tooltip
    public float weaponStrikeTime;
    #region Tooltip
    [Tooltip("Select weapon cooldown time")]
    #endregion Tooltip
    public float weaponCooldownTime;

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);

    }

#endif
    #endregion Validation
}

