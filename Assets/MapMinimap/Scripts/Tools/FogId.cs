using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapMinimap
{
    /// <summary>
    /// FogId represent the position of a fog point, it has a x,y position and a zone_id
    /// </summary>

    [System.Serializable]
    public struct FogId
    {
        public int x;
        public int y;
        public int zone;

        public FogId(int iX, int iY, int z)
        {
            x = iX;
            y = iY;
            zone = z;
        }

        public override string ToString()
        {
            return String.Format("[{0}: {1}, {2}]", zone, x, y);
        }

        public static implicit operator Vector3Int(FogId rValue)
        {
            return new Vector3Int(rValue.x, rValue.y, rValue.zone);
        }

        public static implicit operator FogId(Vector3Int rValue)
        {
            return new FogId(rValue.x, rValue.y, rValue.z);
        }

        public static FogId zero { get { return new FogId(0, 0, 0); } }

    }

}
