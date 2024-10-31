using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MainLocationInfo
{
    private static Grid grid;

    public static Grid Grid
    {
        get
        {
            if (grid == null)
                grid = GameObject.FindObjectOfType<Grid>();
            return grid;
        }
    }



}
