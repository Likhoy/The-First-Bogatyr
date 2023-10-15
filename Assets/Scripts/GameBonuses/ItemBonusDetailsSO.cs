using UnityEngine;

[CreateAssetMenu(fileName = "ItemBonusDetails_", menuName = "Scriptable Objects/Bonuses/ItemBonusDetails")]
public class ItemBonusDetailsSO : BonusDetailsSO
{
    #region Tooltip
    [Tooltip("Item which will be given to the player")]
    #endregion
    public GameObject itemGiven;
    #region Tooltip
    [Tooltip("Item number to give")]
    #endregion
    public int itemNumber;
}
