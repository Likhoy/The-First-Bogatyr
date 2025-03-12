using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;

    private DialogueSystemController controller;
    private Player player;
    private UiMaster uiMaster;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsUI;

    void Awake() {
        player = GameManager.Instance.GetPlayer();
        uiMaster = player.gameObject.GetComponent<UiMaster>();
        controller = FindObjectOfType<DialogueSystemController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(Settings.commandButtons[Command.OpenPauseMenu]))
        {
            if (controller.isConversationActive)
                return;
            if (GlobalStatus.menuOn )
            {
                uiMaster.CloseAllMenu();
                return;
            }
            if (settingsUI.activeSelf)
            {
                settingsUI.SetActive(false);
                return;
            }

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
        Time.timeScale = 1f;
        GameIsPaused = false;
        settingsUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        controller.Unpause();
        GlobalStatus.freezePlayer = false;     
    }

    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
        controller.Pause();
        GlobalStatus.freezePlayer = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Settings.menuSceneName);
    }
  
}
