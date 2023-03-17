using UnityEngine;

[CreateAssetMenu(fileName = "LocationDetails_", menuName = "Scriptable Objects/Location/LocationDetails")]
public class LocationDetailsSO : ScriptableObject
{

    #region Header ENEMY DETAILS
    [Space(10)]
    [Header("ENEMY DETAILS")]
    #endregion

    [Tooltip("TODO")]
    public EnemySpawnData[] enemiesToSpawnArray;
}
