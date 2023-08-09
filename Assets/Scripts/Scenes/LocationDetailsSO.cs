using UnityEngine;

[CreateAssetMenu(fileName = "LocationDetails_", menuName = "Scriptable Objects/Location/LocationDetails")]
public class LocationDetailsSO : ScriptableObject
{
    #region Header BASE DETAILS
    [Space(10)]
    [Header("BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Fill in this location scene name")]
    #endregion
    public string sceneName;

    #region Header ENEMY DETAILS
    [Space(10)]
    [Header("ENEMY DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Enemies to spawn immediately in the current location")]
    #endregion
    public EnemySpawnData[] enemiesToSpawnImmediately;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(sceneName), sceneName);
    }
#endif
    #endregion
}
