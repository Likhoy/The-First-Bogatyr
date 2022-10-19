using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemType = 0;
    /*
        0 - is for generic item
        1 - armor
        2 - weapon/tool
        3 - consumables
    */
    public int itemID = 0;
    public bool isStackable = false;
    public bool isDisposable = false;
    public int itemCount = 0;
    public int itemMaxCount = 1;
}
