using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public static class AStar
{
    /// <summary>
    /// Builds a path from the startGridPosition to the endGridPosition, and adds
    /// movement steps to the returned Stack. Returns null if no path is found.
    /// </summary>
    public static Stack<Vector3> BuildPath(Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // Adjust positions by lower bounds
        startGridPosition -= (Vector3Int)MainLocationInfo.locationLowerBounds;
        endGridPosition -= (Vector3Int)MainLocationInfo.locationLowerBounds;

        // Create open list and closed hashset
        PriorityQueue<Node, int> openNodeQueue = new PriorityQueue<Node, int>();
        HashSet<Node> openNodeHashSet = new HashSet<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();


        // Create gridnodes for path finding
        //GridNodes gridNodes = new GridNodes(LocationInfo.locationUpperBounds.x - LocationInfo.locationLowerBounds.x + 1, LocationInfo.locationUpperBounds.y - LocationInfo.locationLowerBounds.y + 1);
        GridNodes gridNodes = MainLocationInfo.GridNodes;
        MainLocationInfo.ClearGridNodes();
        /*Debug.Log(startGridPosition.x);
        Debug.Log(startGridPosition.y);

        Debug.Log(endGridPosition.x);
        Debug.Log(endGridPosition.y);*/

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeQueue, openNodeHashSet, closedNodeHashSet);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode);
        }

        return null;
    }

    /// <summary>
    /// Find the shortest path - returns the end Node if a path has been found, else returns null.
    /// </summary>
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, PriorityQueue<Node, int> openNodeQueue, HashSet<Node> openNodeHashSet, HashSet<Node> closedNodeHashSet)
    {
        // Add start node to open list
        openNodeQueue.Enqueue(startNode, startNode.FCost);
        openNodeHashSet.Add(startNode);

        // Loop through open node list until empty
        while (openNodeQueue.Count > 0)
        {
            // current node = the node in the open list with the lowest fCost
            Node currentNode = openNodeQueue.Dequeue();
            openNodeHashSet.Remove(currentNode);

            MainLocationInfo.spoiledNodes.Add(currentNode);

            // if the current node = target node then finish
            if (currentNode == targetNode)
            {
                MainLocationInfo.spoiledNodes.AddRange(openNodeHashSet);
                return currentNode;
            }

            // add current node to the closed list
            closedNodeHashSet.Add(currentNode);

            // evaluate fcost for each neighbour of the current node
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeQueue, openNodeHashSet, closedNodeHashSet);
        }

        return null;

    }


    /// <summary>
    ///  Create a Stack<Vector3> containing the movement path 
    /// </summary>
    private static Stack<Vector3> CreatePathStack(Node targetNode)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        // Get mid point of cell
        Vector3 cellMidPoint = MainLocationInfo.Grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            // Convert grid position to world position
            Vector3 worldPosition = MainLocationInfo.Grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + MainLocationInfo.locationLowerBounds.x, nextNode.gridPosition.y + MainLocationInfo.locationLowerBounds.y, 0));

            // Set the world position to the middle of the grid cell
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }

    /// <summary>
    /// Evaluate neighbour nodes
    /// </summary>
    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, PriorityQueue<Node, int> openNodeQueue, HashSet<Node> openNodeHashSet, HashSet<Node> closedNodeHashSet)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        // Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet);

                if (validNeighbourNode != null)
                {
                    // Calculate new gcost for neighbour
                    int newCostToNeighbour;

                    // Get the movement penalty
                    // Unwalkable paths have a value of 0. Default movement penalty is set in
                    // Settings and applies to other grid squares.
                    int movementPenaltyForGridSpace = MainLocationInfo.AStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                    bool isValidNeighbourNodeInOpenList = openNodeHashSet.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeQueue.Enqueue(validNeighbourNode, validNeighbourNode.FCost);
                            openNodeHashSet.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns the distance int between nodeA and nodeB
    /// </summary>
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  // 10 used instead of 1, and 14 is a pythagoras approximation SQRT(10*10 + 10*10) - to avoid using floats
        return 14 * dstX + 10 * (dstY - dstX);
    }

    /// <summary>
    /// Evaluate a neighbour node at neighbourNodeXPosition, neighbourNodeYPosition, using the
    /// specified gridNodes, closedNodeHashSet.  Returns null if the node isn't valid
    /// </summary>
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes, HashSet<Node> closedNodeHashSet)
    {
        // If neighbour node position is beyond grid then return null
        if (neighbourNodeXPosition >= Settings.defaultGridNodesWidthForPathBuilding || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= Settings.defaultGridNodesHeightForPathBuilding || neighbourNodeYPosition < 0) // for testing
        {
            return null;
        }

        // Get neighbour node
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        // check for obstacle at that position
        //int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];
        int movementPenaltyForGridSpace = MainLocationInfo.AStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        // check for moveable obstacle at that position
        //int itemObstacleForGridSpace = instantiatedRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];
        int itemObstacleForGridSpace = 5; // for testing

        // if neighbour is an obstacle or neighbour is in the closed list then skip
        if (movementPenaltyForGridSpace == 0 || itemObstacleForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }

    }
}
