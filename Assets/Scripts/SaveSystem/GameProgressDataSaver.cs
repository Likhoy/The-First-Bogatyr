using UnityEngine;
using PixelCrushers;
using System;

[AddComponentMenu("")]
public class GameProgressDataSaver : Saver
{
    [Serializable]
    public class Data
    {
        public bool healthHintShown;

        public bool[] checkpointReached;
    }

    private Data m_data = new Data();

    public override string RecordData()
    {
        m_data.healthHintShown = GameProgressData.healthHintShown;
        m_data.checkpointReached = GameProgressData.checkpointReached;
        return SaveSystem.Serialize(m_data);
    }

    public override void ApplyData(string s)
    {
        var data = SaveSystem.Deserialize<Data>(s, m_data);
        if (data == null) return;
        m_data = data;
        GameProgressData.healthHintShown = m_data.healthHintShown;
        GameProgressData.checkpointReached = m_data.checkpointReached;
    }
}
