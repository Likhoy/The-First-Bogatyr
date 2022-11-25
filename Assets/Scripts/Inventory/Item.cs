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

    public int itemID;
    public bool isStackable = false;
    public bool isDisposable = false;
    //public int itemCount = 0;
    //public int itemMaxCount = 1;
    Sprite sprite;
    public Sprite Sprite { get { return sprite; } }

    /*public void DropItem()
    {
        
    }*/

    protected void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
    }

    public void OnMouseDown() => TakeItem();

    public void TakeItem()
    {
        GameObject.FindObjectOfType<Inventory>().AddItem(this);
        GameObject.Destroy(this.gameObject);
    }

    virtual public void UseItem()
    {
        throw new System.NotImplementedException();
    }
}
