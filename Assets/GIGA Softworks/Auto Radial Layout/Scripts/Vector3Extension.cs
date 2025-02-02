using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIGA.AutoRadialLayout
{
    public static class Vector3Extension
    {
        public static float Magnitude2D(this Vector3 vec)
        {
            return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y);
        }

        public static Vector3 RoundToInt(this Vector3 vec)
        {
            return new Vector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
        }

    }
}
