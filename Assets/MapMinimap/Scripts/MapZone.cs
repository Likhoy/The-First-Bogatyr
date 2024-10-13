using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapMinimap
{
    /// <summary>
    /// Map zone should always be defined in the XZ plane
    /// </summary>

    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider))]
    public class MapZone : MonoBehaviour
    {
        public int zone_id;
        public Texture map;

        private BoxCollider collide;

        private static List<MapZone> zone_list = new List<MapZone>();

        private void Awake()
        {
            zone_list.Add(this);

            Renderer render = GetComponent<Renderer>();
            if (render != null)
                render.enabled = false;

            collide = GetComponent<BoxCollider>();
            collide.enabled = !Application.isPlaying;
            collide.center = Vector3.zero;
            collide.size = Vector3.one;

            if (map == null && Application.isPlaying)
                Debug.LogError("MapZone has no map assigned!");
        }

        private void OnDestroy()
        {
            zone_list.Remove(this);
        }

        public void SetBounds(Bounds bound)
        {
            transform.position = bound.center;
            transform.localScale = bound.size;
        }

        public Vector3 GetCenter()
        {
            return transform.position;
        }

        public float GetRotation()
        {
            return transform.rotation.eulerAngles.y;
        }

        public Vector2 GetSize()
        {
            return new Vector2(transform.localScale.x, transform.localScale.z);
        }

        public Vector2 GetExtents()
        {
            return new Vector2(transform.localScale.x / 2f, transform.localScale.z / 2f);
        }

        //Check if point is inside the map zone
        public bool IsInside(Vector3 pos)
        {
            Vector3 lpos = transform.InverseTransformPoint(pos);
            return lpos.x < 0.5f && lpos.z < 0.5f
                && lpos.x > -0.5f && lpos.z > -0.5f;
        }

        //Return position of point in -1/1 range
        public Vector2 GetNormalizedPos(Vector3 pos)
        {
            Vector3 lpos = transform.InverseTransformPoint(pos);
            Vector2 map_pos = new Vector2(lpos.x * 2f, lpos.z * 2f);
            return map_pos;
        }

        //Return world position from the normalized map position
        public Vector3 GetWorldPosition(Vector2 map_pos)
        {
            Vector3 lpos = new Vector3(map_pos.x * 0.5f, 0f, map_pos.y * 0.5f);
            return transform.TransformPoint(lpos);
        }

        //Convert World pos to World pos on zone plane
        public Vector3 GetProjectedPosition(Vector3 pos)
        {
            pos.y = 0f;
            return pos;
        }

        public static MapZone Get(int id)
        {
            foreach (MapZone zone in zone_list)
            {
                if (zone.zone_id == id)
                    return zone;
            }
            return null;
        }

        public static MapZone Get(Vector3 pos)
        {
            foreach (MapZone zone in zone_list)
            {
                if (zone.IsInside(pos))
                    return zone;
            }
            return null;
        }

        public static List<MapZone> GetAll()
        {
            return zone_list;
        }
    }

}
