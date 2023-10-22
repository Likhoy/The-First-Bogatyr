using UnityEngine;

public abstract class BonusDetailsSO : ScriptableObject
{
    #region Header BONUS BASE DETAILS
    [Space(10)]
    [Header("BONUS BASE DETAILS")]
    #endregion Header BONUS BASE DETAILS
    #region Tooltip
    [Tooltip("Bonus name")]
    #endregion Tooltip
    public string bonusName;
    #region Tooltip
    [Tooltip("The sprite for the bonus")]
    #endregion Tooltip
    public Sprite bonusSprite;
}
