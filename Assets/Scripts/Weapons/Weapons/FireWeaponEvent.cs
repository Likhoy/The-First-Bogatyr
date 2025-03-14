using UnityEngine;
using System;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    public void CallFireWeaponEvent(bool fire, bool firePreviousFrame, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, float targetPositionX = 0f, float targetPositionY = 0f)
    {
        OnFireWeapon?.Invoke(this, new FireWeaponEventArgs() { fire = fire, firePreviousFrame = firePreviousFrame, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector, targetPosition = new Vector2(targetPositionX, targetPositionY) });
    }
}

public class FireWeaponEventArgs : EventArgs
{
    public bool fire;
    public bool firePreviousFrame;
    //public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
    public Vector2 targetPosition; // for throwing weapons
}