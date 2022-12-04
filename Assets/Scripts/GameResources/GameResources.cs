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

    //Добавить инвентарь
    /*
     #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("Player details list - populate the list with the playerdetails scriptable objects")]
    #endregion Tooltip
    public List<PlayerDetailsSO> playerDetailsList;
     */
}
