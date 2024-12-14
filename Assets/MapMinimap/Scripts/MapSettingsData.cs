using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapMinimap
{

    [CreateAssetMenu(fileName = "MapSettings", menuName = "Data/MapSettings", order = 10)]
    public class MapSettingsData : ScriptableObject
    {
        [Header("Zoom")]
        public float zoom_max = 4f;

        [Header("Icons")]
        public float icon_scale = 1f;
        public int icon_refresh_max = 100;

        [Header("Fog")]
        public bool fog = true;
        public float fog_reveal_radius = 10f;
        public int fog_precision = 100;
        public float fog_update_rate = 0.5f;

        public static MapSettingsData Get()
        {
            MapLevelSettings lsettings = MapLevelSettings.Get();
            if (lsettings != null)
                return lsettings.data;
            MapManager manager = MapManager.Get();
            if (manager != null)
                return manager.default_settings;
            return null;
        }
    }

}
