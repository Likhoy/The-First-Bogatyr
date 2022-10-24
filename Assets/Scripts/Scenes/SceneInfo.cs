using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class SceneInfo
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

    public static int[,] aStarMovementPenalty;  // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
    public static int[,] aStarItemObstacles; // use to store position of moveable items that are obstacles

    /// <summary>
    /// Update obstacles used by AStar pathfinmding.
    /// </summary>
    private static void AddObstaclesAndPreferredPaths()
    {
        // this array will be populated with wall obstacles 
        aStarMovementPenalty = new int[Settings.defaultGridNodesHeightForPathBuilding, Settings.defaultGridNodesWidthForPathBuilding];


        // Loop thorugh all grid squares
        for (int x = 0; x < Settings.defaultGridNodesHeightForPathBuilding; x++)
        {
            for (int y = 0; y < Settings.defaultGridNodesWidthForPathBuilding; y++)
            {
                // Set default movement penalty for grid sqaures
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                // Add obstacles for collision tiles the enemy can't walk on
                /*TileBase tile = collisionTilemap.GetTile(new Vector3Int(x, y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                // Add preferred path for enemies (1 is the preferred path value, default value for
                // a grid location is specified in the Settings).
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }*/

            }
        }

    }
}
