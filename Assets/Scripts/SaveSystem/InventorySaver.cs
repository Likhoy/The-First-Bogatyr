using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;
using System;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Saves the slots data of the inventory
/// </summary>
[AddComponentMenu("")]
public class InventorySaver : Saver
{
    private Inventory inventory;

    private int realSlotsCount;

    public override void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    [Serializable]
    public class Data
    {
        public int realSlotsCount;
        public int[] item_ids;
        public int[] counts;
    }

    private Data m_data = new Data();

    public override string RecordData()
    {
        realSlotsCount = inventory.Slots.Count(slot => !slot.IsEmpty);
        m_data.realSlotsCount = realSlotsCount;
        m_data.item_ids = new int[realSlotsCount]; 
        m_data.counts = new int[realSlotsCount];
        for (int i = 0; i < realSlotsCount; i++)
        {
            
            m_data.item_ids[i] = inventory.Slots[i].Item.itemID;
            m_data.counts[i] = inventory.Slots[i].Count;
        }
        return SaveSystem.Serialize(m_data);
    }

    public override void ApplyData(string s)
    {
        if (string.IsNullOrEmpty(s)) return;
        var data = SaveSystem.Deserialize<Data>(s, m_data);
        if (data == null) return;
        m_data = data;
        for (int i = 0; i < m_data.realSlotsCount; i++)
        {
            GameObject itemPrefab = GameResources.Instance.items.First(item => item.GetComponent<Item>().itemID == m_data.item_ids[i]);
            for (int j = 0; j < m_data.counts[i]; j++)
                GameManager.Instance.GiveItem(itemPrefab);
        }
    }
}
