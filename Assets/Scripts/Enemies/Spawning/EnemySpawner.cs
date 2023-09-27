using PixelCrushers.DialogueSystem;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    public int EnemiesSpawnedSoFar { get; set; }
    private Grid grid;
    
    private void OnEnable()
    {
        Lua.RegisterFunction("SpawnEnemy", this, SymbolExtensions.GetMethodInfo(() => SpawnEnemy(string.Empty, string.Empty)));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("SpawnEnemy");
    }

    /// <summary>
    /// Spawn the enemies
    /// </summary>
    public void SpawnEnemies()
    {
        // Set gamestate engaging boss
        /*if (GameManager.Instance.gameState == GameState.bossStage)
        {
            GameManager.Instance.previousGameState = GameState.bossStage;
            GameManager.Instance.gameState = GameState.engagingBoss;
        }

        // Set gamestate engaging enemies
        else if(GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }*/

        LocationDetailsSO currentLocationDetails = null;
        foreach (LocationDetailsSO locationDetails in GameManager.Instance.allLocationsDetails)
        {
            if (SceneManager.GetActiveScene().name == locationDetails.sceneName)
                currentLocationDetails = locationDetails;
        }

        if (currentLocationDetails == null)
            throw new NullReferenceException("Не найдена нужная сцена в общем списке сцен.");
            

        grid = MainLocationInfo.Grid;
        enemiesToSpawn = currentLocationDetails.enemiesToSpawnImmediately.Length;

        // Check we have somewhere to spawn the enemies
        if (enemiesToSpawn > 0)
        {
            // Loop through to create all the enemeies
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                EnemyDetailsSO enemyDetails = currentLocationDetails.enemiesToSpawnImmediately[EnemiesSpawnedSoFar].enemyDetails;

                Vector3Int cellPosition = (Vector3Int)currentLocationDetails.enemiesToSpawnImmediately[EnemiesSpawnedSoFar].spawnPosition;

                // Create Enemy - Get next enemy type to spawn 
                CreateEnemy(enemyDetails, 1, grid.CellToWorld(cellPosition));
            }
        }
    }

    public void SpawnEnemy(string enemyName, string spawnPosition)
    {
        int[] coords = spawnPosition.Split(" ").Select(coord => int.Parse(coord)).ToArray();
        Vector3Int spawnPositionVect = new Vector3Int(coords[0], coords[1], coords[2]);
        foreach (EnemyDetailsSO enemyDetails in GameResources.Instance.enemyDetailsList)
        {
            if (enemyDetails.enemyName == enemyName)
            {
                CreateEnemy(enemyDetails, 1, grid.CellToWorld(spawnPositionVect));
            }
                
        }
    }

    /// <summary>
    /// spawn enemy variation for usage in the endless mode
    /// </summary>
    public void SpawnEnemy(EnemyDetailsSO enemyDetails, Vector2Int spawnPosition)
    {
        CreateEnemy(enemyDetails, 2, grid.CellToWorld((Vector3Int)spawnPosition));
    }


    /// <summary>
    /// Create an enemy in the specified position
    /// </summary>
    private void CreateEnemy(EnemyDetailsSO enemyDetails, int enemyPrefabNum, Vector3 position)
    {
        // keep track of the number of enemies spawned so far 
        EnemiesSpawnedSoFar++;

        // Add one to the current enemy count - this is reduced when an enemy is destroyed
        currentEnemyCount++;

        // Instantiate enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefabs[enemyPrefabNum - 1], position, Quaternion.identity, transform);

        // Initialize Enemy
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, EnemiesSpawnedSoFar);

        // subscribe to enemy destroyed event
        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;

    }

    /// <summary>
    /// Process enemy destroyed
    /// </summary>
    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        // Unsubscribe from event
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        // TODO - adjust architecture (for the first quest)
        if (EnemiesSpawnedSoFar == 1)
            GetComponent<DialogueSystemTrigger>().OnUse();

        // reduce current enemy count
        currentEnemyCount--;

        if (currentEnemyCount <= 0)
        {

            GameManager.Instance.TryLaunchNextWave();

            // Set game state
            /*if (GameManager.Instance.gameState == GameState.engagingEnemies)
            {
                GameManager.Instance.gameState = GameState.playingLevel;
                GameManager.Instance.previousGameState = GameState.engagingEnemies;
            }

            else if (GameManager.Instance.gameState == GameState.engagingBoss)
            {
                GameManager.Instance.gameState = GameState.bossStage;
                GameManager.Instance.previousGameState = GameState.engagingBoss;
            }*/
        }
    }

}