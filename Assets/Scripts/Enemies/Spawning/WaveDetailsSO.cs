using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDetails_", menuName = "Scriptable Objects/Endless Mode/WaveDetails")]
public class WaveDetailsSO : ScriptableObject
{
    #region Tooltip
    [Tooltip("This is a real wave number according to waves order in the game")]
    #endregion
    public int waveNumber;

    #region Tooltip
    [Tooltip("Populate with enemies to spawn during the wave considering the order (one list element might contain several enemies to be spawned simultaneously)")]
    #endregion
    public List<EnemiesGroupWaveSpawnData> enemyGroupsSpawnDatas;

    public List<BonusDetailsSO> possibleBonuses;
}
