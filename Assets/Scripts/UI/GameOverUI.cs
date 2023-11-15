using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text statisticsText;

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().destroyedEvent.OnDestroyed += ShowGameOverPanel;
    }
  
    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().destroyedEvent.OnDestroyed -= ShowGameOverPanel;
    }

    private void ShowGameOverPanel(DestroyedEvent @event, DestroyedEventArgs args)
    {
        Time.timeScale = 0f;

        panel.SetActive(true);

        statisticsText.text = "¬ы упорно сражались, но злые силы победили";
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Settings.menuSceneName);
    }
}
