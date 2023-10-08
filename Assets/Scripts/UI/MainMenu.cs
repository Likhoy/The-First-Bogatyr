using UnityEngine;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private DialogueSystemController dialogSystemController;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject endlessModeGameButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject exitButton;

    private GameObject[] allButtons; // auxiliary container

    private void Awake()
    {
        if (!SaveSystem.HasSavedGameInSlot(1))
            continueButton.SetActive(false);
        dialogSystemController = FindObjectOfType<DialogueSystemController>();
        dialogSystemController.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    private void Start()
    {
        allButtons = new GameObject[4] { continueButton, newGameButton, settingsButton, exitButton };

        dialogSystemController.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        dialogSystemController.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        dialogSystemController.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ContinueGamePressed()
    {
        DeactivateButtonsAfterClick(continueButton);

        SaveSystem.LoadFromSlot(1);

        dialogSystemController.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        dialogSystemController.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        dialogSystemController.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        dialogSystemController.transform.GetChild(1).gameObject.SetActive(true);
    }

    /// <summary>
    /// Start endless mode 
    /// </summary>
    public void StartEndlessModePressed()
    {
        DeactivateButtonsAfterClick(endlessModeGameButton);

        GameManager.Instance.PrepareEndlessMode();
        SaveSystem.RestartGame(Settings.mainSceneName); 
    }

    /// <summary>
    /// Start main story line
    /// </summary>
    public void StartNewGamePressed()
    {
        DeactivateButtonsAfterClick(newGameButton);

        SaveSystem.DeleteSavedGameInSlot(1);

        GameManager.Instance.PrepareMainStoryLine(); // preparations of game states are obligatory
        SaveSystem.RestartGame(Settings.mainSceneName);
        dialogSystemController.ResetDatabase();
    }

    private void DeactivateButtonsAfterClick(GameObject buttonPressed)
    {
        Button button = buttonPressed.GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(115, 115, 115);
        button.colors = cb;
        button.interactable = false;

        foreach (GameObject anotherButton in allButtons)
        {
            if (anotherButton != buttonPressed)
            {
                anotherButton.GetComponent<Button>().enabled = false;
            }
        }
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    

}

