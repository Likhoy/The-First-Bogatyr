using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapMinimap
{

    public class MapTool
    {
        //Return the closest fog waypoint that can be saved as 'revealed' in the data, in world coord
        public static FogId GetClosestFogPoint(Vector3 world_pos)
        {
            MapZone zone = MapZone.Get(world_pos);
            return GetClosestFogPoint(zone, world_pos);
        }

        //Return the closest fog waypoint that can be saved as 'revealed' in the data, in world coord
        public static FogId GetClosestFogPoint(MapZone zone, Vector3 world_pos)
        {
            if (zone != null)
            {
                Vector3 map_pos = zone.GetNormalizedPos(world_pos);
                Vector2Int fog_pos = GetClosestFogPointMap(map_pos);
                FogId fog = new FogId(fog_pos.x, fog_pos.y, zone.zone_id);
                return fog;
            }
            return FogId.zero;
        }

        //Return the closest fog waypoint that can be saved as 'revealed' in the data, in map coord (-1 to 1)
        public static Vector2Int GetClosestFogPointMap(Vector2 map_pos)
        {
            MapSettingsData settings = MapSettingsData.Get();
            if (settings != null)
            {
                Vector3 pos = (map_pos + Vector2.one) * 0.5f;  //Convert from 0 to 1
                int x = Mathf.RoundToInt(pos.x * (settings.fog_precision));
                int y = Mathf.RoundToInt(pos.y * (settings.fog_precision));
                return new Vector2Int(x, y);
            }
            return Vector2Int.zero;
        }

        //Return map coordinates (-1 to 1) of fog points in data
        public static Vector2 GetFogMapPos(FogId fog_point)
        {
            MapSettingsData settings = MapSettingsData.Get();
            if (settings != null)
            {
                float x = fog_point.x / (float)(settings.fog_precision);
                float y = fog_point.y / (float)(settings.fog_precision);
                return new Vector2(x * 2f - 1f, y * 2f - 1f);
            }
            return Vector2.zero;
        }

        //Return world coordinates (scene position) of fog points in data
        public static Vector3 GetFogWorldPos(FogId fog_point)
        {
            MapZone zone = MapZone.Get(fog_point.zone);
            Vector2 map_pos = GetFogMapPos(fog_point);
            return MapToWorldPos(zone, map_pos);
        }

        //Return normalized pos in -1, 1
        public static Vector2 WorldToMapPos(Vector3 world_pos)
        {
            MapZone zone = MapZone.Get(world_pos);
            if (zone != null)
            {
                Vector2 norm_pos = zone.GetNormalizedPos(world_pos);
                return norm_pos;
            }
            return Vector2.zero;
        }

        //Return scene 3D position (from a map pos from -1 to 1)
        public static Vector3 MapToWorldPos(MapZone zone, Vector2 map_pos)
        {
            if (zone != null)
            {
                Vector3 world_pos = zone.GetWorldPosition(map_pos);
                return world_pos;
            }
            return Vector3.zero;
        }

        public static Vector3 WorldToProjectedWorldPos(Vector3 world_pos)
        {
            MapZone zone = MapZone.Get(world_pos);
            if (zone != null)
            {
                Vector3 wpos = zone.GetProjectedPosition(world_pos);
                return wpos;
            }
            return world_pos;
        }

        public static string GetCurrentScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
    }

}
