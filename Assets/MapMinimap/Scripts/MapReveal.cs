using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapMinimap
{

    /// <summary>
    /// Attach to any object to make the object reveal fog
    /// </summary>
    
    public class MapReveal : MonoBehaviour
    {
        public bool player = true;  //If true, map position with pan to this reveal

        [HideInInspector]
        public int reveal_id = 0; //For assets with multiple players, otherwise keep to 0

        private Transform trans;

        private static List<MapReveal> list = new List<MapReveal>();

        void Awake()
        {
            list.Add(this);
            trans = transform;
        }

        void OnDestroy()
        {
            list.Remove(this);
        }

        public Vector3 GetWorldPos()
        {
            return trans.position;
        }

        public static MapReveal GetPlayer()
        {
            foreach (MapReveal reveal in list)
            {
                if (reveal.player)
                    return reveal;
            }
            return null;
        }

        public static MapReveal GetPlayer(int id)
        {
            foreach (MapReveal reveal in list)
            {
                if (reveal.player && reveal.reveal_id == id)
                    return reveal;
            }
            return null;
        }

        public static MapReveal Get(int id)
        {
            foreach (MapReveal reveal in list)
            {
                if (reveal.reveal_id == id)
                    return reveal;
            }
            return null;
        }

        public static List<MapReveal> GetAll()
        {
            return list;
        }
    }
}
