using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovementAI : MonoBehaviour
{
    private NPC npc;
    private int positionIndex;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Coroutine moveNPCRoutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public int updateFrameNumber = 2; // default value.  This is set by the npc spawner.

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }

    private void Start()
    {
        // Create waitforfixed update for use in coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();
        positionIndex = 0;
    }

    void Update()
    {
        if (npc.npcDetails.movesToSomePoints)
        {
            MoveAlongThePath();
        }
        else if (npc.npcDetails.movesRandomly)
        {
            MoveRandomly();
        }
    }

    private void MoveAlongThePath()
    {
        // Only process A Star path rebuild on certain frames to spread the load between npc
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        if (positionIndex == npc.npcDetails.pointsToMove.Length && npc.npcDetails.movesCyclically) 
                positionIndex = 0;
        
        if (positionIndex < npc.npcDetails.pointsToMove.Length)
        {
            // Move the npc using AStar pathfinding
            CreatePath(positionIndex);

            // If a path has been found move the npc
            if (movementSteps != null)
            {
                if (moveNPCRoutine != null)
                {
                    // Trigger idle event
                    // enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveNPCRoutine);
                }

                // Move npc along the path using a coroutine
                moveNPCRoutine = StartCoroutine(MoveNPCRoutine(movementSteps));
            }
        }
    }

    /// <summary>
    /// Use the AStar static class to create a path for the npc
    /// </summary>
    private void CreatePath(int positionIndex)
    {
        Grid grid = LocationInfo.Grid;

        // Get next position on the grid
        Vector3 nextPosition = npc.npcDetails.pointsToMove[positionIndex];

        Vector3Int nextGridPosition = LocationInfo.Grid.WorldToCell(nextPosition);

        // Get npc position on the grid
        Vector3Int npcGridPosition = grid.WorldToCell(transform.position);

        // Build a path for the npc to move on
        movementSteps = AStar.BuildPath(npcGridPosition, nextGridPosition);

        // Take off first step on path - this is the grid square the npc is already on
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
    }

    /// <summary>
    /// Coroutine to move the npc to the next location on the path
    /// </summary>
    private IEnumerator MoveNPCRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            // while not very close continue to move - when close move onto the next step
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                // Trigger movement event
                npc.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, npc.npcDetails.moveSpeed, (nextPosition - transform.position).normalized);

                yield return waitForFixedUpdate;  // moving the enmy using 2D physics so wait until the next fixed update
            }

            yield return waitForFixedUpdate;
        }

        // End of path steps - trigger the enemy idle event
        positionIndex++;
        // npc.idleEvent.CallIdleEvent();
    }

    private void MoveRandomly()
    {
        throw new NotImplementedException();
    }
}
