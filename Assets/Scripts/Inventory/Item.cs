using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IUseable
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
    [SerializeField] public Sprite sprite;

    virtual protected void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        isTaken = false;
    }

    private void OnMouseDown() => TakeItem();

    public void TakeItem()
    {
        //������� (���������)
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
}
