using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    InventorySlot[] slots;

    public void AddItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty)
                if (slots[i].ID == item.itemID)
                {
                    slots[i].IncCount();
                    return;
                }

            if (slots[i].IsEmpty)
            {
                slots[i].AddNewItem(item);
                return;
            }
        }
    }
}
