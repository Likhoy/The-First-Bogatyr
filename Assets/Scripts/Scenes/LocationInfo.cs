using System.Collections.Generic;
using UnityEngine;

public static class LocationInfo
{
    private static Grid grid;

    // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
    private static int[,] aStarMovementPenalty = new int[0, 0];

    // use to store position of moveable items that are obstacles
    private static int[,] aStarItemObstacles = new int[0, 0];

    public const int realLocationWidth = 292;
    public const int realLocationHeight = 212;

    public static readonly Vector2Int locationLowerBounds = new Vector2Int(-200, -200);
    public static readonly Vector2Int locationUpperBounds = new Vector2Int(200, 200);

    private static GridNodes gridNodes = null;

    internal static HashSet<Node> spoiledNodes = new HashSet<Node>();

    public static GridNodes GridNodes
    {
        get
        {
            if (gridNodes == null)
                gridNodes = new GridNodes(locationUpperBounds.x - locationLowerBounds.x + 1, locationUpperBounds.y - locationLowerBounds.y + 1);
            return gridNodes;
        }
    }

    public static void ClearGridNodes()
    {
        foreach (Node node in spoiledNodes)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.parentNode = null;
        }
        spoiledNodes.Clear();
    }

    public static Grid Grid
    {
        get
        {
            if (grid == null)
                grid = GameObject.FindObjectOfType<Grid>();
            return grid;
        }
    }

    public static int[,] AStarMovementPenalty
    {
        get 
        { 
            if (aStarMovementPenalty.Length == 0)
                AddObstacles();
            return aStarMovementPenalty;
        }
    } 
    public static int[,] AStarItemObstacles
    {
        get; set;
    }



    /// <summary>
    /// Update obstacles used by AStar pathfinmding.
    /// </summary>
    private static void AddObstacles()
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
