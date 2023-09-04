using System;
using UnityEngine;

[Serializable]
public class Product
{
    public GameObject itemPrefab;
    public Sprite sprite;
    public string itemName;
    public int price;
}

[Serializable]
public class WeaponProduct
{
    public WeaponDetailsSO weaponDetails;
    public int weaponAmmoAmount = 0;
    public int price;
}

