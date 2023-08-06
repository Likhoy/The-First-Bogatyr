using UnityEngine;

public interface IFireable
{
    void InitialiseAmmo(AmmoDetailsSO ammoDetails, GameObject shooter, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector,
        Vector2 targetPosition, bool overrideAmmoMovement = false);

    GameObject GetGameObject();
}
