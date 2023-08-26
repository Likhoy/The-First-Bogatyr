using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<DestroyedEvent>().OnDestroyed += GameFinishTrigger_OnDestroyed;
    }

    private void OnDisable()
    {
        GetComponent<DestroyedEvent>().OnDestroyed -= GameFinishTrigger_OnDestroyed;
    }

    private void GameFinishTrigger_OnDestroyed(DestroyedEvent arg1, DestroyedEventArgs arg2)
    {
        FinishGame();
    }

    private void FinishGame()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.FinishGameRoutine());
        SaveSystem.DeleteSavedGameInSlot(2);
    }
}
