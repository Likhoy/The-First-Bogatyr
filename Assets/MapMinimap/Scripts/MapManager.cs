using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MapMinimap
{
    /// <summary>
    /// Class to manage the map UI in game
    /// </summary>

    public class MapManager : MonoBehaviour
    {
        public MapSettingsData default_settings;    //This will be used if no MapSettingsData is added to the scene
        public GameObject map_ui;
        public GameObject event_system;

        public UnityAction onOpenMap;
        public UnityAction onCloseMap;

        [HideInInspector]
        public int reveal_id = 0; //For assets with multiple players, otherwise keep to 0

        [HideInInspector]
        public bool show_player_warning = true;

        private float update_timer = 0f;
        private int icon_reveal_index = 0;

        private static MapManager _instance;

        private void Awake()
        {
            _instance = this;

            MapUI ui = FindObjectOfType<MapUI>();
            if (ui == null)
                Instantiate(map_ui);

            EventSystem evt_sys = FindObjectOfType<EventSystem>();
            if (evt_sys == null)
                Instantiate(event_system);

            MapData.LoadLast();
        }

        private void Start()
        {
            MapUI.Get().onShow += () => { if (onOpenMap != null) onOpenMap.Invoke(); };
            MapUI.Get().onHide += () => { if (onCloseMap != null) onCloseMap.Invoke(); };

            MapReveal player = MapReveal.GetPlayer();
            if (player == null && show_player_warning)
            {
                Debug.Log("There is no player MapReveal in the scene, map wont move and fog won't be revealed!");
            }

            MapZone zone = FindObjectOfType<MapZone>();
            if (zone == null)
                Debug.LogError("There are no MapZone in the scene!");
        }

        private void Update()
        {
            MapControls controls = MapControls.Get();
            if (controls && controls.IsPressMap())
            {
                MapUI.Get().Toggle();
            }

            MapSettingsData settings = MapSettingsData.Get();
            update_timer += Time.deltaTime;
            if (settings != null && update_timer > settings.fog_update_rate)
            {
                update_timer = 0f;
                SlowUpdate();
            }
        }

        private void SlowUpdate()
        {
            //Check if reveal fog
            MapSettingsData settings = MapSettingsData.Get();
            MapSceneData scene_data = GetMapSceneData();
            if (settings && settings.fog && scene_data != null)
            {
                foreach (MapReveal reveal in MapReveal.GetAll())
                {
                    if (reveal.reveal_id == reveal_id)
                    {
                        Vector3 wpos = reveal.GetWorldPos();
                        MapZone zone = MapZone.Get(wpos);
                        if (zone != null)
                        {
                            FogId fog_point = MapTool.GetClosestFogPoint(zone, wpos);
                            scene_data.Reveal(fog_point, settings.fog_reveal_radius);
                        }
                    }
                }
            }

            //Refresh icons in fog
            MapReveal player = MapReveal.GetPlayer(reveal_id);
            if (settings != null && player != null)
            {
                int nb_icons = MapIcon.GetAll().Count;
                int reveal_min = icon_reveal_index;
                int reveal_max = icon_reveal_index + settings.icon_refresh_max; //Only refresh 100 icons per SlowUpdate, for performance
                icon_reveal_index = reveal_max < nb_icons ? reveal_max : 0;

                int index = 0;
                foreach (MapIcon icon in MapIcon.GetAll())
                {
                    if (!icon.IsRevealed())
                    {
                        if(index >= reveal_min && index < reveal_max)
                            icon.RefreshRevealed(); //Refresh reveal with slow function
                        if ((player.GetWorldPos() - icon.GetWorldPos()).magnitude < settings.fog_reveal_radius)
                            icon.Reveal(); //Force reveal if near player
                    }
                    index++;
                }
            }
        }

        public void OpenMap()
        {
            MapUI.Get().Show();
        }

        public void CloseMap()
        {
            MapUI.Get().Hide();
        }

        //World position
        public void RevealAt(Vector3 pos, float radius)
        {
            MapSceneData scene_data = GetMapSceneData();
            FogId fog_point = MapTool.GetClosestFogPoint(pos);
            scene_data?.Reveal(fog_point, radius);
        }

        public void RevealAllMap()
        {
            MapSceneData scene_data = GetMapSceneData();
            scene_data?.Reveal(FogId.zero, 100000f);
        }

        //Check if position (in the scene) is revealed (very slow, bad performance, avoid using in loop)
        public bool IsRevealed(Vector3 world_pos)
        {
            MapSceneData scene_data = GetMapSceneData();
            world_pos = MapTool.WorldToProjectedWorldPos(world_pos);
            if (scene_data != null)
            {
                foreach (KeyValuePair<FogId, float> point in scene_data.fog_reveal)
                {
                    Vector3 fpos = MapTool.GetFogWorldPos(point.Key);
                    float dist = (fpos - world_pos).magnitude;
                    if (dist < point.Value)
                        return true;
                }
            }
            return false;
        }

        //Check if map position is revealed (from -1 to 1 pos) (very slow, bad performance, avoid using in loop)
        public bool IsRevealedMap(MapZone zone, Vector2 map_pos) {
            Vector3 world_pos = MapTool.MapToWorldPos(zone, map_pos);
            return IsRevealed(world_pos);
        }

        public MapIcon AddIcon(string id, Sprite icon, Vector3 world_pos, MapIconType type = MapIconType.Default)
        {
            return MapIcon.Create(id, icon, world_pos, type);
        }

        public MapIcon AddIcon(string id, Sprite icon, Transform parent, MapIconType type = MapIconType.Default)
        {
            return MapIcon.Create(id, icon, parent, type);
        }

        public void DeleteIcon(string id)
        {
            MapIcon.Delete(id);
        }
		
        public MapSceneData GetMapSceneData()
        {
            MapData mdata = MapData.Get(reveal_id);
            if (mdata != null)
                return mdata.GetSceneData(MapTool.GetCurrentScene());
            return null;
        }

        public MapSceneData GetMapSceneData(int player_id)
        {
            MapData mdata = MapData.Get(player_id);
            if(mdata != null)
                return mdata.GetSceneData(MapTool.GetCurrentScene());
            return null;
        }

        public static MapManager Get()
        {
            return _instance;
        }
    }

}
