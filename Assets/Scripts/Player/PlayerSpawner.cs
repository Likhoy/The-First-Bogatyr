using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSpawner : SingletonMonobehaviour<PlayerSpawner>
{
    private PlayerDetailsSO playerDetails;
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        // Set player details - saved in current player scriptable object from the main menu
        playerDetails = GameResources.Instance.playerDetailsList[0]; // now we have only one player

        InstantiatePlayer();
    }

    private void Start()
    {
        // Initialize Player
        player.Initialize(playerDetails);
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

        player.destroyedEvent.OnDestroyed += GameManager.Instance.PlayerDestroyedEvent_OnDestroyed;
    }

    public Player GetPlayer()
    {
        return player;
    }
}
