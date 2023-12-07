using UnityEngine;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
        
        DialogueManager.Instance.gameObject.SetActive(true); // might be disabled in endless mode
        
        DialogueManager.Instance.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    private void Start()
    {
        allButtons = new GameObject[5] { continueButton, newGameButton, endlessModeGameButton, settingsButton, exitButton };

        // this way of placing disabling of dialogueManager children is intentional
        DialogueManager.Instance.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        DialogueManager.Instance.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        DialogueManager.Instance.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ContinueGamePressed()
    {
        DeactivateButtonsAfterClick(continueButton);

        SaveSystem.LoadFromSlot(1);

        DialogueManager.Instance.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        DialogueManager.Instance.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        DialogueManager.Instance.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        DialogueManager.Instance.transform.GetChild(1).gameObject.SetActive(true);
    }

    /// <summary>
    /// Start endless mode 
    /// </summary>
    public void StartEndlessModePressed()
    {
        DeactivateButtonsAfterClick(endlessModeGameButton);

        GameManager.Instance.PrepareEndlessMode();
        //SaveSystem.RestartGame(Settings.mainSceneName); 
        SceneManager.LoadScene(Settings.mainSceneName);
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
        DialogueManager.Instance.ResetDatabase();
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

