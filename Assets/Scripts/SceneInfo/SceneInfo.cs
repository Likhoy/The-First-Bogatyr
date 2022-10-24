using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneInfo
{
    public static Grid grid;

    public static int[,] aStarMovementPenalty;  // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
    public static int[,] aStarItemObstacles; // use to store position of moveable items that are obstacles
}
