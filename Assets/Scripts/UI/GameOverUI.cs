using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text statisticsText;

    private void OnEnable()
    {
        StaticEventHandler.OnWaveFinished += ShowGameOverPanel;
    }  

    private void OnDisable()
    {
        StaticEventHandler.OnWaveFinished -= ShowGameOverPanel;
    }

    private void ShowGameOverPanel(WaveFinishedEventArgs args)
    {
        if(args.waveNumber == Settings.wavesAmount)
        {
            Time.timeScale = 0f;

            panel.SetActive(true);

            nameText.text = "Победа";
            statisticsText.text = "Вы победили всех монстров";
        }
        
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Settings.menuSceneName);
    }
}
