using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;

    public InventorySlot[] Slots { get => slots; }

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().GetComponent<DialogueSystemEvents>().conversationEvents.onConversationStart.AddListener(delegate { HideInventory(); });
        GameManager.Instance.GetPlayer().GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.AddListener(delegate { ShowInventory(); });
    }

    private void OnDisable()
    {
        Player player = GameManager.Instance.GetPlayer();
        if (player != null)
        {
            player.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationStart.RemoveListener(delegate { HideInventory(); });
            player.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationEnd.RemoveListener(delegate { ShowInventory(); });
        }
    }

    public void AddItems(Item item, int count)
    {
        for (int i = 0; i < count; i++)
            AddItem(item);
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

    public void ShowInventory()
    {
        foreach(InventorySlot slot in slots)
            slot.ShowInventorySlot();
    }

    public void HideInventory()
    {
        foreach (InventorySlot slot in slots)
            slot.HideInventorySlot();
    }

    public int ContainsItem(int ID)
    {
        int res = 0;

        foreach (InventorySlot slot in slots)
            if (slot.ID == ID)
            {
                res = slot.Count;
                break;
            }

        return res;
    }
}
