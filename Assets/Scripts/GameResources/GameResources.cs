using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<GameResources>("GameResources");
            return instance;
        }
    }

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("Player details list - populate the list with the playerdetails scriptable objects")]
    #endregion Tooltip
    public List<PlayerDetailsSO> playerDetailsList;

    #region Header PLAYER_WEAPONS
    [Space(10)]
    [Header("PLAYER_WEAPONS")]
    #endregion Header PLAYER_WEAPONS
    #region Tooltip
    [Tooltip("Player weapons details list - populate the list with the weapondetails scriptable objects")]
    #endregion Tooltip
    public List<WeaponDetailsSO> weaponDetailsList;

    #region Header ENEMIES
    [Space(10)]
    [Header("ENEMIES")]
    #endregion Header ENEMIES
    #region Tooltip
    [Tooltip("Populate with enemies, which would be spawned in quests")]
    #endregion Tooltip
    public List<EnemyDetailsSO> enemyDetailsList;

    #region Header INVENTORY
    [Space(10)]
    [Header("INVENTORY")]
    #endregion Header INVENTORY
    #region Tooltip
    [Tooltip("Poplate this with working Inventory gameobject")]
    #endregion Tooltip
    public Inventory inventory;
    
}
