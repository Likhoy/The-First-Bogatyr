using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour, IUseable
{
    public int itemType = 0;

    /*
        0 - is for generic item
        1 - armor
        2 - weapon/tool
        3 - consumables
    */
    public string itemName;
    public int itemID;
    public bool isStackable = false;
    public bool isDisposable = false;
    public bool isTaken;
    //public int itemCount = 0;
    //public int itemMaxCount = 1;
    public Sprite sprite;

    protected Image effectImage; 
    [SerializeField] protected Sprite effectSprite;
    protected AudioSource audioEffects;
    [SerializeField] protected AudioClip CDrink;

    virtual protected void Start()
    {
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
        effectImage = GameObject.Find("EffectsImage").GetComponent<Image>();
        sprite = GetComponent<SpriteRenderer>().sprite;
        isTaken = false;
    }

    // private void OnMouseDown() => TakeItem();

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
            Inventory inventory = GameObject.FindObjectOfType<Inventory>();
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            inventory.AddItem(this);
            isTaken = true;
            if (inventory.ContainsItem(itemID) > 1)
                GameObject.Destroy(this.gameObject);
        }
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
