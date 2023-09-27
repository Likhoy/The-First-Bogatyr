using PixelCrushers;
using PixelCrushers.DialogueSystem;
using static RandomExtensions;
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


    private PlayerDetailsSO playerDetails;
    private Player player;

    private DialogueSystemController dialogueSystemController;

    #region Tooltip
    [Tooltip("Populate in the order of scenes appearing in the game")]
    #endregion
    public LocationDetailsSO[] allLocationsDetails;

    #region Tooltip
    [Tooltip("Populate with all waves of endless mode")]
    #endregion
    [SerializeField] private WaveDetailsSO[] allWaveDetails;

    private int currentWaveNumber = 1;

    protected override void Awake()
    {
        // Call base class
        base.Awake();

        // Set player details - saved in current player scriptable object from the main menu
        playerDetails = GameResources.Instance.playerDetailsList[0]; // now we have only one player

        // Instantiate player
        InstantiatePlayer();

        dialogueSystemController = FindObjectOfType<DialogueSystemController>();

        // for testing endless mode
        Destroy(dialogueSystemController.gameObject);
        SetUpAndStartEndlessMode();
        //SetQuestUIActive();
    }

    private void SetQuestUIActive()
    {
        dialogueSystemController.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        dialogueSystemController.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        dialogueSystemController.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        dialogueSystemController.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("GiveWeaponToPlayer", this, SymbolExtensions.GetMethodInfo(() => GiveWeaponToPlayer(string.Empty, 0.0)));
        Lua.RegisterFunction("GiveChanceToAvoidDamageToCreature", this, SymbolExtensions.GetMethodInfo(() => GiveChanceToAvoidDamageToCreature(string.Empty, 0.0)));
        player.destroyedEvent.OnDestroyed += PlayerDestroyedEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("GiveWeaponToPlayer");
        Lua.UnregisterFunction("GiveChanceToAvoidDamageToCreature");
        player.destroyedEvent.OnDestroyed -= PlayerDestroyedEvent_OnDestroyed;
    }

    private void Start()
    {
        // Initialize Player
        player.Initialize(playerDetails);

        // Spawn enemies (maybe this will be placed in enemy controller class)
        EnemySpawner.Instance.SpawnEnemies();
    }


    /// <summary>
    /// Create player in scene at position
    /// </summary>
    private void InstantiatePlayer()
    {
        // Instantiate player
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        // Get Player
        player = playerGameObject.GetComponent<Player>();
    }

    private void PlayerDestroyedEvent_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        StopAllCoroutines();
        // should be expanded
        SceneManager.LoadScene("Menu");
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
                player.AddWeaponToPlayer(weaponDetails, (int)weaponAmmoAmount);
            }  
        }
    }

    public void GiveChanceToAvoidDamageToCreature(string creatureTag, double percent)
    {
        if (creatureTag == "Player")
            player.health.SetChanceToAvoidDamage((int)percent);
        else
        {
            // TODO
        }
    }

    private void SetUpAndStartEndlessMode()
    {
        StartCoroutine(LaunchWave());
    }

    public void TryLaunchNextWave()
    {
        if (currentWaveNumber < allWaveDetails.Length)
        {
            currentWaveNumber++;
            LaunchWave(currentWaveNumber);
        }
    }

    private IEnumerator LaunchWave(int waveNumber = 1)
    {
        WaveDetailsSO currentWaveDetails = allWaveDetails[waveNumber - 1];
        for (int i = 0; i < currentWaveDetails.enemyGroupsSpawnDatas.Count; i++)
        {
            EnemiesGroupWaveSpawnData groupSpawnData = currentWaveDetails.enemyGroupsSpawnDatas[i];
            yield return new WaitForSeconds(groupSpawnData.delayAfterPreviousSpawn);
            
            Vector2Int[] spawnPositions = ChooseRandomSpawnPositions(groupSpawnData.amountOfEnemiesToSpawn);
            for (int j = 0; j < groupSpawnData.enemiesBaseData.Length; j++)
            {
                EnemySpawner.Instance.SpawnEnemy(groupSpawnData.enemiesBaseData[j], spawnPositions[j]);
            }
        }
    }

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

    /// <summary>
    /// Get the player
    /// </summary>
    public Player GetPlayer()
    {
        return player;
    }

    public void GiveItem(GameObject itemPrefab)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory.ContainsItem(itemPrefab.GetComponent<Item>().itemID) >= 1)
            inventory.AddItem(itemPrefab.GetComponent<Item>());
        else
        {
            GameObject item = Instantiate(itemPrefab, player.transform);
            item.GetComponent<CircleCollider2D>().enabled = false;
            item.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            inventory.AddItem(item.GetComponent<Item>());
        }
    }

}
