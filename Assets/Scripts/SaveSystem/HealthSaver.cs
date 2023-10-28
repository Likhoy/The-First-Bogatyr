using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;
using System;
using Language.Lua;

[AddComponentMenu("")]
public class HealthSaver : Saver
{
    [Serializable]
    public class Data
    {
        public int health;
    }


    private Data m_data = new Data();

    public override string RecordData()
    {
        m_data.health = GetComponent<Health>().currentHealth;
        return SaveSystem.Serialize(m_data);
    }

    public override void ApplyData(string s)
    {
        var data = SaveSystem.Deserialize<Data>(s, m_data);
        if (data == null) return;
        m_data = data;
        Health health = GetComponent<Health>();
        health.currentHealth = m_data.health;
        health.healthEvent.CallHealthChangedEvent((float)m_data.health / (float)health.GetMaxHealth(), m_data.health, health.GetMaxHealth() - m_data.health);
    }
}
