using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;
using System;
using UnityEngine.UI;

/// <summary>
/// Saves the slots data of the inventory
/// </summary>
[AddComponentMenu("")]
public class InventorySaver : Saver
{
    private Inventory inventory;

    public override void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    [Serializable]
    public class Data
    {
        public Item[] items;
        public int[] counts;
    }

    private Data m_data = new Data();

    public override string RecordData()
    {
        m_data.items = new Item[inventory.Slots.Length]; 
        m_data.counts = new int[inventory.Slots.Length];
        for (int i = 0; i < inventory.Slots.Length; i++)
        {
            m_data.items[i] = inventory.Slots[i].Item;
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
        for (int i = 0; i < inventory.Slots.Length; i++)
        {
            inventory.Slots[i].AddNewItem(m_data.items[i]);
            for (int j = 0; j < m_data.counts[i] - 1; j++)
                inventory.Slots[i].IncCount();
        }
    }
}
