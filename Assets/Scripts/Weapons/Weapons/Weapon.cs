public abstract class Weapon
{
    public int weaponListPosition;
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

