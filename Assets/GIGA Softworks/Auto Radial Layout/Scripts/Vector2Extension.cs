using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIGA.AutoRadialLayout
{
    public static class Vector2Extension
    {
        /// <summary>
        /// Rotates a vector by given radians.
        /// </summary>
        public static Vector2 Rotate(this Vector2 v, float radians)
        {
            return new Vector2(
                v.x * Mathf.Cos(radians) - v.y * Mathf.Sin(radians),
                v.x * Mathf.Sin(radians) + v.y * Mathf.Cos(radians)
            );
        }
       

    }
}
