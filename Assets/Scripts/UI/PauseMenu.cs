using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;

    private DialogueSystemController controller;
    private Player player;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject mainUI;

    void Awake() {
        player = GameManager.Instance.GetPlayer();

    }
    
    void Update()
    {
        // controller = FindObjectOfType<DialogueSystemController>();
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
        settingsUI.SetActive(false);
        mainUI.SetActive(true);
        //controller.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        //controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        //controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        //controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        //controller.transform.GetChild(1).gameObject.SetActive(true);
        //player.transform.GetChild(1).gameObject.SetActive(true);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        mainUI.SetActive(false);

        //controller.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        //controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        //controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        //controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        //controller.transform.GetChild(1).gameObject.SetActive(false);
        //player.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Settings.menuSceneName);
    }
  
}
