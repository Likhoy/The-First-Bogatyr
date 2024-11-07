
using PixelCrushers.DialogueSystem;
using PixelCrushers;
using UnityEngine;

public class MainStoryModeStrategy : IGameModeStrategy
{
    public void StartGame(Player player)
    {
        SetQuestUIActive();

        player.Initialize(GameResources.Instance.playerDetailsList[0]);
    }

    public void UpdateGame()
    {
        
    }

    public void EndGame()
    {
        
    }

    public void HandlePlayerDeath(Player player)
    {
        SaveSystem.LoadFromSlot(1);
        Object.Destroy(player.gameObject);
    }

    private void SetQuestUIActive()
    {
        DialogueManager.Instance.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        DialogueManager.Instance.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        DialogueManager.Instance.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        DialogueManager.Instance.transform.GetChild(1).gameObject.SetActive(true);
    }
}
