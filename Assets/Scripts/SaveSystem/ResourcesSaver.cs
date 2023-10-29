using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers;
using System;

[AddComponentMenu("")]
public class ResourcesSaver : Saver
{
    [Serializable]
    public class Data
    {
        public int moneyAmount;
        public int experienceAmount;
    }

    private Data m_data = new Data();

    public override string RecordData()
    {
        m_data.moneyAmount = GetComponent<PlayerResources>().PlayerMoney;
        m_data.moneyAmount = GetComponent<PlayerResources>().PlayerExperience;
        return SaveSystem.Serialize(m_data);
    }

    public override void ApplyData(string s)
    {
        var data = SaveSystem.Deserialize<Data>(s, m_data);
        if (data == null) return;
        m_data = data;

        PlayerResources playerResources = GetComponent<PlayerResources>();
        playerResources.PlayerMoney = 0;
        playerResources.PlayerExperience = 0;
        playerResources.AddMoney(m_data.moneyAmount);
        playerResources.AddExperience(m_data.experienceAmount);
    }
}
