using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDetails_", menuName = "Scriptable Objects/NPC/NPC Details")]
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

    public bool hasDialogWithPlayer;

    public List<Product> products;

    public List<WeaponProduct> weaponProducts;

    public float moveSpeed;

    public bool movesRandomly;

    public bool movesToSomePoints;

    public bool movesCyclically;

    public Vector2[] pointsToMove;
}
