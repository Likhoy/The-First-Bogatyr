using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER BASE DETAILS
    [Space(10)]
    [Header("PLAYER BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Prefab gameobject for the player")]
    #endregion
    public GameObject playerPrefab;

    #region Header HEALTH
    [Space(10)]
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("Player starting health amount")]
    #endregion
    public int playerHealthAmount;
    #region Tooltip
    [Tooltip("Select if has some effect period immediately after being hit.  If so specify the effect time in seconds in the other field")]
    #endregion
    public bool hasHitEffect = false;
    #region Tooltip
    [Tooltip("Hit effect time in seconds after being hit")]
    #endregion
    public float hitEffectTime;

}
