using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    private GameObject inventory;
    private GameObject HealthBar;
    private GameObject Bestiary;

    private void Start()
    {
        inventory = GameObject.Find("Inventory");
        HealthBar = GameObject.FindGameObjectWithTag("HealthBar");
        Bestiary = GameObject.Find("BestiaryButton");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        inventory.SetActive(false);
        HealthBar.SetActive(false);
        Bestiary.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
  
}
