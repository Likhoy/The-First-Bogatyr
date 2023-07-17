using PixelCrushers.DialogueSystem;

public class CustomGameSaver : GameSaver
{
    public void SaveGameAtCheckpoint(int checkpointIndex)
    {
        if (!GameProgressData.checkpointReached[checkpointIndex])
        {
            DialogueManager.instance.ShowAlert("Сохранение...");
            GameProgressData.checkpointReached[checkpointIndex] = true;
            SaveGame(slot);
        }
    }
}
