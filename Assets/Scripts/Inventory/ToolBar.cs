using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ToolBar : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;

    public InventorySlot[] Slots { get => slots; }

    private void OnEnable()
    {
        Player player = GameManager.Instance.GetPlayer();
        if (player == null)
            return;

        var events = player.GetComponent<DialogueSystemEvents>();
        if (events != null)
        {
            events.conversationEvents.onConversationStart.AddListener(delegate { HideInventory(); });
            events.conversationEvents.onConversationEnd.AddListener(delegate { ShowInventory(); });
        }
    }

    private void OnDisable()
    {
        Player player = GameManager.Instance.GetPlayer();
        if (player == null)
            return;

        var events = player.GetComponent<DialogueSystemEvents>();
        if (events != null)
        {
            events.conversationEvents.onConversationStart.RemoveListener(delegate { HideInventory(); });
            events.conversationEvents.onConversationEnd.RemoveListener(delegate { ShowInventory(); });
        }
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
