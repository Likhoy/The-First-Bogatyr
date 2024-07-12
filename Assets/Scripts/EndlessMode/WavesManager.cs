using System.Collections;
using System.Linq;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with all waves of endless mode")]
    #endregion
    public WaveDetailsSO[] allWavesDetails;

    private int currentWaveNumber = 0;

    private void Start()
    {
        TryLaunchNextWave();
    }

    public void TryLaunchNextWave()
    {
        if (currentWaveNumber < allWavesDetails.Length)
        {
            currentWaveNumber++;
            StartCoroutine(LaunchWave(currentWaveNumber));
            StaticEventHandler.CallWaveLaunchedEvent(currentWaveNumber);
        }
    }

    private IEnumerator LaunchWave(int waveNumber = 1)
    {
        WaveDetailsSO currentWaveDetails = allWavesDetails[waveNumber - 1];

        for (int i = 0; i < currentWaveDetails.enemyGroupsSpawnDatas.Count; i++)
        {
            EnemiesGroupWaveSpawnData groupSpawnData = currentWaveDetails.enemyGroupsSpawnDatas[i];
            yield return new WaitForSeconds(groupSpawnData.delayAfterPreviousSpawn);

            Vector2Int[] spawnPositions = ChooseRandomSpawnPositions(groupSpawnData.amountOfEnemiesToSpawn);
            for (int j = 0; j < groupSpawnData.amountOfEnemiesToSpawn; j++)
            {
                EnemyModifiers enemyModifiers = CalculateEnemyModifiers(groupSpawnData.enemiesBaseData[j]); // get enemy modifiers
                EnemySpawner.Instance.SpawnEnemy(groupSpawnData.enemiesBaseData[j], spawnPositions[j], enemyModifiers, Enemy_OnDestroyed);
            }
        }
    }

    private EnemyModifiers CalculateEnemyModifiers(EnemyDetailsSO enemyDetails)
    {
        if (currentWaveNumber % Settings.waveAmountBetweenModifiers == 0)
        {
            int multiplier = currentWaveNumber / Settings.waveAmountBetweenModifiers;
            int healthModifierEffect = Mathf.RoundToInt(enemyDetails.baseHealthModifier / 100 * enemyDetails.startingHealthAmount);
            int damageModifierEffect = 0; // Mathf.RoundToInt(enemyDetails.baseDamageModifier / 100 * ...);

            EnemyModifiers enemyModifiers = new EnemyModifiers()
            {
                healthModifierEffect = healthModifierEffect * multiplier,
                damageModifierEffect = damageModifierEffect * multiplier
            };
            return enemyModifiers;
        }
        return null;
    }

    /// <summary>
    /// Choose needed number of spawn positions for spawning enemies in the endless mode
    /// </summary>
    private Vector2Int[] ChooseRandomSpawnPositions(int positionsNumber)
    {
        int[] possibleSpawnPositionsNums = Enumerable.Range(0, Settings.enemySpawnPossiblePositions.Length).ToArray();
        System.Random r = new System.Random();
        r.Shuffle(possibleSpawnPositionsNums);

        Vector2Int[] possibleSpawnPositions = new Vector2Int[positionsNumber];
        for (int i = 0; i < positionsNumber; i++)
        {
            possibleSpawnPositions[i] = Settings.enemySpawnPossiblePositions[possibleSpawnPositionsNums[i]];
        }
        return possibleSpawnPositions;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        if (EnemySpawner.Instance.CurrentEnemyCount <= 0)
        {
            // wave finished
            StaticEventHandler.CallWaveFinishedEvent(currentWaveNumber, allWavesDetails[currentWaveNumber - 1]);
            
            // launch next if possible
            TryLaunchNextWave();
        }
    }

}
