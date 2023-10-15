using UnityEngine;

[CreateAssetMenu(fileName = "PowerBonusDetails_", menuName = "Scriptable Objects/Bonuses/PowerBonusDetails")]
public class PowerBonusDetailsSO : BonusDetailsSO
{
    #region Tooltip
    [Tooltip("Percent of bonus effect")]
    #endregion
    public int bonusPercent;
    #region Tooltip
    [Tooltip("Bonus type - what effect will be applied")]
    #endregion
    public PowerBonusType bonusType;
}
