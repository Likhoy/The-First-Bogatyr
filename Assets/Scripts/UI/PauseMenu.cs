using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;

    private DialogueSystemController controller;
    private Player player;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject inventory; // лучше использовать SerializeField в этом случае, чем GameObject.Find
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject BestiaryButton;
    [SerializeField] private GameObject Bestiary;
    [SerializeField] private GameObject Map;
    [SerializeField] private GameObject Money;
    [SerializeField] private GameObject TradingUI;
    [SerializeField] private GameObject ButtonsHelper;
    [SerializeField] private GameObject EffectsImage;
    [SerializeField] private GameObject SettingsUI;
    [SerializeField] private GameObject SettingsControlUI;
    [SerializeField] private GameObject WeaponSlot;
    [SerializeField] private GameObject ExperienceBar;




    void Update()
    {
        controller = FindObjectOfType<DialogueSystemController>();
        player = GameManager.Instance.GetPlayer();

        if (Input.GetKeyDown(Settings.commandButtons[Command.OpenPauseMenu]))
        {
            if (SettingsUI.activeInHierarchy)
            {
                GameIsPaused = false;
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
        
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        inventory.SetActive(true);
        HealthBar.SetActive(true);
        BestiaryButton.SetActive(true);
        Bestiary.SetActive(true);
        Map.SetActive(true);
        Money.SetActive(true);
        TradingUI.SetActive(true);
		if (ButtonsHelper != null)
			ButtonsHelper.SetActive(true);
        EffectsImage.SetActive(true);
        SettingsUI.SetActive(false);
        SettingsControlUI.SetActive(false);
        WeaponSlot.SetActive(true);
        ExperienceBar.SetActive(true);

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
        inventory.SetActive(false);
        HealthBar.SetActive(false);
        BestiaryButton.SetActive(false);
        Bestiary.SetActive(false);
        Map.SetActive(false);
        Money.SetActive(false);
        TradingUI.SetActive(false);
        if (ButtonsHelper != null)
            ButtonsHelper.SetActive(false);
        EffectsImage.SetActive(false);
        SettingsUI.SetActive(false);
        WeaponSlot.SetActive(false);
        ExperienceBar.SetActive(false);


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
