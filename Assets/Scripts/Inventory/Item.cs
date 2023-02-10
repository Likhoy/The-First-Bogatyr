using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Item : MonoBehaviour, IUseable
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
    public bool isTaken;
    //public int itemCount = 0;
    //public int itemMaxCount = 1;
    public Sprite sprite;

    virtual protected void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        isTaken = false;
    }

    //private void OnMouseDown() => TakeItem();

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();
            if (!player.takeItemList.Contains(this))
                player.takeItemList.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();
            if (player.takeItemList.Contains(this))
                player.takeItemList.Remove(this);
        }   
    }

    public void TakeItem()
    {
        //Костыль (исправить)
        if (!isTaken)
        {
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            GameObject.FindObjectOfType<Inventory>().AddItem(this);
            isTaken = true;
            return;
        }
        GameObject.Destroy(this.gameObject);
    }

    virtual public void UseItem()
    {
        throw new System.NotImplementedException();
    }

   public void SellItem()
    {
        throw new System.NotImplementedException();
    }
}
