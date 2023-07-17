using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject inventory; // лучше использовать SerializeField в этом случае, чем GameObject.Find
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject Bestiary;
    [SerializeField] private GameObject Money;

    void Update()
    {
        if (Input.GetKeyDown(Settings.commandButtons[Command.OpenPauseMenu]))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        inventory.SetActive(true);
        HealthBar.SetActive(true);
        Bestiary.SetActive(true);
        Money.SetActive(true);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        inventory.SetActive(false);
        HealthBar.SetActive(false);
        Bestiary.SetActive(false);
        Money.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
  
}
