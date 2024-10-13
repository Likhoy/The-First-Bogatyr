using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapMinimap
{

    /// <summary>
    /// Attach to any object to have the map position centered on that object 
    /// (for example, attach to your camera to have the minimap scrolling move to the position of the camera)
    /// </summary>

    public class MapCenter : MonoBehaviour
    {
        private Transform trans;

        private static MapCenter instance;

        void Awake()
        {
            instance = this;
            trans = transform;
        }

        public Vector3 GetWorldPos()
        {
            return trans.position;
        }

        public static MapCenter Get()
        {
            return instance;
        }
    }
}
