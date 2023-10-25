using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]

public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion Header GAMEOBJECT REFERENCES

    [SerializeField] private DialogueSystemController dialogueSystemController;

    #region Tooltip
    [Tooltip("Populate in the order of scenes appearing in the game")]
    #endregion
    public LocationDetailsSO[] allLocationsDetails;

    #region Tooltip
    [Tooltip("Populate with all waves of endless mode")]
    #endregion
    public WaveDetailsSO[] allWavesDetails;

    private int currentWaveNumber = 1;

    public GameState gameState { get; set; }

    protected override void Awake() 
    {
        // Call base class
        base.Awake();

        Lua.RegisterFunction("GiveWeaponToPlayer", this, SymbolExtensions.GetMethodInfo(() => GiveWeaponToPlayer(string.Empty, 0.0)));
        Lua.RegisterFunction("GiveChanceToAvoidDamageToCreature", this, SymbolExtensions.GetMethodInfo(() => GiveChanceToAvoidDamageToCreature(string.Empty, 0.0)));
    }

    public void PrepareMainStoryLine()
    {
        SceneManager.sceneLoaded += OnSceneLoaded_StartMainStoryLine;
    }

    public void PrepareEndlessMode()
    {
        SceneManager.sceneLoaded += OnSceneLoaded_StartEndlessMode;
    }

    private void OnSceneLoaded_StartEndlessMode(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartEndlessMode();
        SceneManager.sceneLoaded -= OnSceneLoaded_StartEndlessMode;
    }

    private void OnSceneLoaded_StartMainStoryLine(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartMainStoryLine();
        SceneManager.sceneLoaded -= OnSceneLoaded_StartMainStoryLine;
    }

    private void SetQuestUIActive()
    {
        dialogueSystemController.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        dialogueSystemController.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        dialogueSystemController.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        dialogueSystemController.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void StartMainStoryLine()
    {
        gameState = GameState.MainStoryLine;

        // Spawn enemies (maybe this will be placed in enemy controller class)
        EnemySpawner.Instance.SpawnEnemies();

        SetQuestUIActive();
    }

    private void OnDestroy()
    {
        Lua.UnregisterFunction("GiveWeaponToPlayer");
        Lua.UnregisterFunction("GiveChanceToAvoidDamageToCreature");
    }

    public void PlayerDestroyedEvent_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        StopAllCoroutines();
        // should be expanded
        SceneManager.LoadScene(Settings.menuSceneName);
        GetPlayer().destroyedEvent.OnDestroyed -= PlayerDestroyedEvent_OnDestroyed;
    }

    public void LetShowSceneTransitionImage()
    {
        dialogueSystemController.GetComponent<CustomSceneTransitionManager>().areScenesCorrect = true;
    }

    public void GiveWeaponToPlayer(string weaponName, double weaponAmmoAmount)
    {
        foreach (WeaponDetailsSO weaponDetails in GameResources.Instance.weaponDetailsList)
        {
            if (weaponDetails.weaponName == weaponName)
            {
                GetPlayer().AddWeaponToPlayer(weaponDetails, (int)weaponAmmoAmount);
            }  
        }
    }

    public void GiveChanceToAvoidDamageToCreature(string creatureTag, double percent)
    {
        if (creatureTag == "Player")
            GetPlayer().health.AddChanceToAvoidDamage((int)percent);
        else
        {
            // TODO
        }
    }

    private void StartEndlessMode()
    {
        // for testing endless mode
        Destroy(dialogueSystemController.gameObject);

        gameState = GameState.EndlessMode;
        StartCoroutine(LaunchWave());
    }

    public void TryLaunchNextWave()
    {
        if (currentWaveNumber < allWavesDetails.Length)
        {
            currentWaveNumber++;
            StartCoroutine(LaunchWave(currentWaveNumber));
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
                EnemySpawner.Instance.SpawnEnemy(groupSpawnData.enemiesBaseData[j], spawnPositions[j], enemyModifiers);
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

            EnemyModifiers enemyModifiers = new EnemyModifiers() { healthModifierEffect = healthModifierEffect * multiplier, 
                damageModifierEffect = damageModifierEffect * multiplier };
            return enemyModifiers;
        }
        return null;
    }

    /// <summary>
    /// Choose needed number of spawn positions for spawning enemies in the endless mode
    /// </summary>
    private Vector2Int[] ChooseRandomSpawnPositions(int positionsNumber)
    {
        int[] possibleSpawnPositionsNums = Enumerable.Range(0, Settings.enemySpawnPossiblePositions.Length - 1).ToArray();
        System.Random r = new System.Random();
        r.Shuffle(possibleSpawnPositionsNums);
        
        Vector2Int[] possibleSpawnPositions = new Vector2Int[positionsNumber];
        for (int i = 0; i < positionsNumber; i++)
        {
            possibleSpawnPositions[i] = Settings.enemySpawnPossiblePositions[possibleSpawnPositionsNums[i]];
        }
        return possibleSpawnPositions;
    }

    public IEnumerator FinishGameRoutine()
    {
        SaveSystem.DeleteSavedGameInSlot(1);
        GameObject transitionImage = GameObject.FindGameObjectWithTag("transitionImage");
        Animator animator = transitionImage.GetComponent<Animator>();
        animator.SetTrigger("Finish");
        SceneManager.LoadScene(allLocationsDetails[0].sceneName);
        yield return null; // исправить
    }

    public void HandlePlayerDeath()
    {
        if (gameState == GameState.MainStoryLine)
        {
            SaveSystem.LoadFromSlot(1);
            Destroy(GetPlayer().gameObject);
        }
        else if (gameState == GameState.EndlessMode)
        {
            if (GetPlayer().health.UseExtraLive())
            {
                // do something
            }
            // TODO: else pause game and show statistics instead of this
            else
            {
                SceneManager.LoadScene(Settings.menuSceneName);
            }
        }
    }

    /// <summary>
    /// Get the player
    /// </summary>
    public Player GetPlayer()
    {
        return PlayerSpawner.Instance.GetPlayer();
    }

    public void GiveItem(GameObject itemPrefab)
    {
        GetPlayer().playerResources.SaveItem(itemPrefab);
    }

}
