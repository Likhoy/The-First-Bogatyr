using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region ANIMATOR PARAMETERS
    
    // Animator parameters - Dialog
    public static int spaceOpen = Animator.StringToHash("spaceOpen");

    // Animator parameters - Player
    public static int lookUp = Animator.StringToHash("lookUp");
    public static int lookDown = Animator.StringToHash("lookDown");
    public static int lookRight = Animator.StringToHash("lookRight");
    public static int lookLeft = Animator.StringToHash("lookLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");

    public static float baseSpeedForPlayerAnimations = 8f;

    #endregion ANIMATOR PARAMETERS

    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    public const string NPCTag = "NPC";
    #endregion

    #region FIRING CONTROL
    public const float useAimAngleDistance = 3.5f; // if the target distance is less than this then the aim angle will be used (calculated from player), else the weapon aim angle will be used (calculated from the weapon). 
    #endregion

    #region ASTAR PATHFINDING PARAMETERS
    public const int defaultGridNodesWidthForPathBuilding = 400;
    public const int defaultGridNodesHeightForPathBuilding = 400;

    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;

    #endregion

    #region ENEMY PARAMETERS
    public const int defaultEnemyHealth = 50;
    #endregion

    // added for testing purposes
    #region CONTACT DAMAGE PARAMETERS
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion 
}
