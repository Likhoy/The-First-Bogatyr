using UnityEngine;

public class EndlessModeStrategy : IGameModeStrategy
{
    public void StartGame(Player player)
    {
        player.Initialize(GameResources.Instance.playerDetailsList[1]);
    }

    public void UpdateGame()
    {
        //  од дл€ обновлени€ бесконечного режима
    }

    public void EndGame()
    {
        //  од дл€ завершени€ бесконечного режима
    }

    public void HandlePlayerDeath(Player player)
    {
        GameObject.FindGameObjectWithTag("GameOverPanel").transform.Find("Panel").gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}