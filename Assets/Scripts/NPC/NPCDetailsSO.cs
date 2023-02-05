using System.Collections.Generic;
using UnityEngine;

public class NPCDetailsSO : ScriptableObject
{
    #region Header NPC BASE DETAILS
    [Space(10)]
    [Header("NPC BASE DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The name of the npc")]
    #endregion
    public string npcName;

    #region Tooltip
    [Tooltip("Prefab gameobject for the NPC")]
    #endregion
    public GameObject npcPrefab;


    public List<Product> products;
}
