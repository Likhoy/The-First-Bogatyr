using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private GameMode gameMode;

    private IGameModeStrategy gameModeStrategy;

    #region Tooltip
    [Tooltip("Populate in the order of scenes appearing in the game")]
    #endregion
    public LocationDetailsSO[] allLocationsDetails;

    private void OnEnable()
    {
        Lua.RegisterFunction("GiveItemToPlayer", this, SymbolExtensions.GetMethodInfo(() => GiveItemToPlayer(string.Empty, 0.0)));
        Lua.RegisterFunction("GiveWeaponToPlayer", this, SymbolExtensions.GetMethodInfo(() => GiveWeaponToPlayer(string.Empty, 0.0)));
        Lua.RegisterFunction("IncreaseChanceToAvoidDamageOfCharacter", this, SymbolExtensions.GetMethodInfo(() => IncreaseChanceToAvoidDamageOfCharacter(string.Empty, 0.0)));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("GiveItemToPlayer");
        Lua.UnregisterFunction("GiveWeaponToPlayer");
        Lua.UnregisterFunction("IncreaseChanceToAvoidDamageOfCharacter");
    }


    private void Start()
    {
        gameModeStrategy = GameModeFactory.CreateStrategy(gameMode);
        gameModeStrategy.StartGame(player);
    }

    

    public void PlayerDestroyedEvent_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        // should be expanded
        // SceneManager.LoadScene(Settings.menuSceneName);
        player.destroyedEvent.OnDestroyed -= PlayerDestroyedEvent_OnDestroyed;
    }

    public void LetShowSceneTransitionImage()
    {
        DialogueManager.Instance.GetComponent<CustomSceneTransitionManager>().areScenesCorrect = true;
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

    public void IncreaseChanceToAvoidDamageOfCharacter(string creatureTag, double percent)
    {
        if (creatureTag == "Player")
        {
            PowerBonusDetailsSO bonusDetails = ScriptableObject.CreateInstance(typeof(PowerBonusDetailsSO)) as PowerBonusDetailsSO;
            bonusDetails.bonusPercent = (int)percent;
            bonusDetails.bonusType = PowerBonusType.DamageReflector;

            Protection.AddProtection<DamageReflector>(player.health, bonusDetails);
        }
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
        yield return new WaitForSeconds(13);
        SceneManager.LoadScene(Settings.menuSceneName);
    }

    public void EndGame()
    {
        gameModeStrategy.EndGame();
    }

    public void HandlePlayerDeath()
    {
        gameModeStrategy.HandlePlayerDeath(player);
    }

    /// <summary>
    /// Get the player
    /// </summary>
    public Player GetPlayer()
    {
        return player;
    }

    // bad realisation - needs refactoring
    public void GiveItemToPlayer(string itemName, double itemCount)
    {
        foreach (GameObject itemPrefab in GameResources.Instance.items)
        {
            if (itemPrefab.GetComponent<Item>().itemName == itemName)
            {
                for (int i = 0; i < (int)itemCount; i++)
                {
                    GiveItem(itemPrefab);
                }
            }
        }
    }

    public void GiveItem(GameObject itemPrefab)
    {
        player.playerResources.SaveItem(itemPrefab);
    }

}
