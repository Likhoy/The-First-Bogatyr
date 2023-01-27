public class Weapon
{
    public WeaponDetailsSO weaponDetails;
    public int weaponListPosition;
}

public class MeleeWeapon : Weapon
{
    public new MeleeWeaponDetailsSO weaponDetails;
}

public class RangedWeapon : Weapon
{
    public new RangedWeaponDetailsSO weaponDetails;
    public float weaponReloadTimer;
    public int weaponClipRemainingAmmo;
    public int weaponRemainingAmmo;
    public bool isWeaponReloading;
}

