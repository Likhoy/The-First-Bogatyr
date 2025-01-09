using PixelCrushers.DialogueSystem;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;
    public int CurrentEnemyCount { get; private set; }
    public int EnemiesSpawnedSoFar { get; set; }

    private void Start()
    {
        SpawnEnemiesImmediately();
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("SpawnEnemy", this, SymbolExtensions.GetMethodInfo(() => SpawnEnemy(string.Empty, string.Empty, string.Empty)));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("SpawnEnemy");
    }

    /// <summary>
    /// Spawn the enemies immediately (right after loading scene)
    /// </summary>
    private void SpawnEnemiesImmediately()
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
            

        enemiesToSpawn = currentLocationDetails.enemiesToSpawnImmediately.Length;

        // Check we have somewhere to spawn the enemies
        if (enemiesToSpawn > 0)
        {
            // Loop through to create all the enemeies
            for (int i = 0; i < enemiesToSpawn; i++)
            {

                // Create Enemy - Get next enemy type to spawn 
                CreateEnemy(currentLocationDetails.enemiesToSpawnImmediately[EnemiesSpawnedSoFar], EnemyPrefabType.MainStoryLine);
            }
        }
    }

    /// <summary>
    /// Spawn enemy by it's name at position passed as string in the following format - "{coord_x} {coord_y} {coord_z}"
    /// </summary>
    public void SpawnEnemy(string enemyName, string spawnPosition, string patrolAreaBounds)
    {
        int[] coords = spawnPosition.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(coord => int.Parse(coord)).ToArray();
        Vector3 spawnPositionVect = new Vector3(coords[0], coords[1], coords[2]);

        int[] coords2 = patrolAreaBounds.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(coord => int.Parse(coord)).ToArray();
        
        Vector2 patrolAreaLeftBottom = new Vector2(coords2[0], coords2[1]);

        Vector2 patrolAreaRightTop = new Vector2(coords2[2], coords2[3]);

        foreach (EnemyDetailsSO enemyDetails in GameResources.Instance.enemyDetailsList)
        {
            if (enemyDetails.enemyName == enemyName)
            {
                CreateEnemy(new EnemySpawnData() { enemyDetails = enemyDetails,
                    spawnPosition = spawnPositionVect, patrolingAreaLeftBottom = patrolAreaLeftBottom, 
                    patrolingAreaRightTop = patrolAreaRightTop}, EnemyPrefabType.MainStoryLine);
            }
                
        }
    }

    /// <summary>
    /// Spawn enemy variation for usage in the endless mode
    /// </summary>
    public GameObject SpawnEnemy(EnemySpawnData enemySpawnData, EnemyModifiers enemyModifiers = null, Action<DestroyedEvent, DestroyedEventArgs> callback = null)
    {
        return CreateEnemy(enemySpawnData, EnemyPrefabType.EndlessMode, enemyModifiers, callback);
    }


    /// <summary>
    /// Create an enemy in the specified position
    /// </summary>
    private GameObject CreateEnemy(EnemySpawnData enemySpawnData, EnemyPrefabType enemyPrefabType, EnemyModifiers enemyModifiers = null, Action<DestroyedEvent, DestroyedEventArgs> callback = null)
    {
        EnemyDetailsSO enemyDetails = enemySpawnData.enemyDetails;

        // keep track of the number of enemies spawned so far
        EnemiesSpawnedSoFar++;

        // Add one to the current enemy count - this is reduced when an enemy is destroyed
        CurrentEnemyCount++;

        // Instantiate enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefabs[(int)enemyPrefabType], enemySpawnData.spawnPosition, Quaternion.identity, transform);

        // Initialize Enemy
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, EnemiesSpawnedSoFar, enemyModifiers);

        DestroyedEvent destroyedEvent = enemy.GetComponent<DestroyedEvent>();

        // subscribe to enemy destroyed event
        destroyedEvent.OnDestroyed += Enemy_OnDestroyed;

        var monsterAi = enemy.GetComponent<MonsterAi>();

        monsterAi.patrolingAreaLeftBottom = enemySpawnData.patrolingAreaLeftBottom;
        monsterAi.patrolingAreaRightTop = enemySpawnData.patrolingAreaRightTop;

        // subscribe passed callbask as well
        if (callback != null)
        {
            destroyedEvent.OnDestroyed += callback;
        }

        return enemy;
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
        CurrentEnemyCount--;

        // add player experience
        var player = GameManager.Instance.GetPlayer();
        // player.playerResources.AddExperience(destroyedEventArgs.experience);
        
        
        if (CurrentEnemyCount <= 0)
        {
                
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