using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;
    private DialogStartedEvent dialogStartedEvent;
    private DialogEndedEvent dialogEndedEvent;

    private void Awake()
    {
        dialogStartedEvent = GameObject.FindGameObjectWithTag("Player").GetComponent<DialogStartedEvent>();
        dialogEndedEvent = GameObject.FindGameObjectWithTag("Player").GetComponent<DialogEndedEvent>();
    }

    private void OnEnable()
    {
        // Subscribe to movement to position event
        dialogStartedEvent.OnStartDialog += HideInventory;
        dialogEndedEvent.OnEndDialog += ShowInventory;
    }

    private void OnDisable()
    {
        // Unsubscribe from movement to position event
        dialogStartedEvent.OnStartDialog -= HideInventory;
        dialogEndedEvent.OnEndDialog -= ShowInventory;
    }

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

    private void ShowInventory(DialogEndedEvent movementToPositionEvent, DialogEndedEventArgs movementToPositionArgs)
    {
        foreach(InventorySlot slot in slots)
            slot.ShowInventorySlot();
    }

    private void HideInventory(DialogStartedEvent movementToPositionEvent, DialogStartedEventArgs movementToPositionArgs)
    {
        foreach (InventorySlot slot in slots)
            slot.HideInventorySlot();
    }
}
