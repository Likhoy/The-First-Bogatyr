using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
