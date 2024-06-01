using UnityEngine;

[CreateAssetMenu(fileName = "ResurrectorBonusDetails_", menuName = "Scriptable Objects/Bonuses/ResurrectorBonusDetails")]
public class ResurrectorBonusDetailsSO : BonusDetailsSO
{
    #region Tooltip
    [Tooltip("Number of times to resurrect player")]
    #endregion
    public int livesReserve;
}
