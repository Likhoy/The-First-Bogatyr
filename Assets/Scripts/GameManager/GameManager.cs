using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
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

    protected override void Awake()
    {
        // Call base class
        base.Awake();

        // Set player details - saved in current player scriptable object from the main menu
        playerDetails = GameResources.Instance.playerDetailsList[0]; // now we have only one player

        // Instantiate player
        InstantiatePlayer();

        dialogueSystemController = FindObjectOfType<DialogueSystemController>();

        SetQuestUIActive();
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
        Lua.RegisterFunction("GiveWeaponToPlayer", this, SymbolExtensions.GetMethodInfo(() => GiveWeaponToPlayer(string.Empty)));
        Lua.RegisterFunction("GiveChanceToAvoidDamageToCreature", this, SymbolExtensions.GetMethodInfo(() => GiveChanceToAvoidDamageToCreature(string.Empty, 0.0)));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("GiveWeaponToPlayer");
        Lua.UnregisterFunction("GiveChanceToAvoidDamageToCreature");
    }

    private void Start()
    {
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

        // Initialize Player
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
    }

    public void LetShowSceneTransitionImage()
    {
        dialogueSystemController.GetComponent<CustomSceneTransitionManager>().areScenesCorrect = true;
    }

    public void GiveWeaponToPlayer(string weaponName)
    {
        foreach (WeaponDetailsSO weaponDetails in GameResources.Instance.weaponDetailsList)
        {
            if (weaponDetails.weaponName == weaponName)
                player.AddWeaponToPlayer(weaponDetails);
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
