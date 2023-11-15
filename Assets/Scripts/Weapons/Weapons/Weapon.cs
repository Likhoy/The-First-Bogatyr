using UnityEngine;

public abstract class Weapon
{
    public int weaponListPosition;
    public int weaponCurrentMinDamage;
    public int weaponCurrentMaxDamage;

    public int GetWeaponDamage() => Random.Range(weaponCurrentMinDamage, weaponCurrentMinDamage);
}

public class MeleeWeapon : Weapon
{
    public MeleeWeaponDetailsSO weaponDetails;
}

public class RangedWeapon : Weapon 
{
    public RangedWeaponDetailsSO weaponDetails;
    public float weaponReloadTimer;
    public int weaponClipRemainingAmmo;
    public int weaponRemainingAmmo;
    public bool isWeaponReloading;
}

