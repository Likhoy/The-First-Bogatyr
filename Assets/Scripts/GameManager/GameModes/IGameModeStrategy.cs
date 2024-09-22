
public interface IGameModeStrategy
{
    void StartGame(Player player);
    
    void UpdateGame();
    
    void EndGame();

    void HandlePlayerDeath(Player player);
}
