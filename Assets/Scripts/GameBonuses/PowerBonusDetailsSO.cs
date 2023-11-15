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
    #region Tooltip
    [Tooltip("Bonus level - higher level makes bonus effect more powerful")]
    #endregion
    public BonusLevel bonusLevel;
    #region Tooltip
    [Tooltip("Durability means \"health points\" for bonus")]
    #endregion
    public int durability;
}
