using PixelCrushers.DialogueSystem;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion Header GAMEOBJECT REFERENCES


    private PlayerDetailsSO playerDetails;
    private Player player;

    protected override void Awake()
    {
        // Call base class
        base.Awake();

        // Set player details - saved in current player scriptable object from the main menu
        playerDetails = GameResources.Instance.playerDetailsList[0]; // now we have only one player

        // Instantiate player
        InstantiatePlayer();
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

    /// <summary>
    /// Get the player
    /// </summary>
    public Player GetPlayer()
    {
        return player;
    }

}
