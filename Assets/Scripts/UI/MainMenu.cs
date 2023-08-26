using UnityEngine;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private DialogueSystemController controller;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject exitButton;

    private GameObject[] allButtons; // auxiliary container

    private void Awake()
    {
        if (!SaveSystem.HasSavedGameInSlot(1))
            continueButton.SetActive(false);
        controller = FindObjectOfType<DialogueSystemController>();
        controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    private void Start()
    {
        allButtons = new GameObject[4] { continueButton, newGameButton, settingsButton, exitButton };

        controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        controller.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ContinueGamePressed()
    {
        DeactivateButtonsAfterClick(continueButton);

        SaveSystem.LoadFromSlot(1);

        controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        controller.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void StartNewGamePressed()
    {
        DeactivateButtonsAfterClick(newGameButton);

        SaveSystem.DeleteSavedGameInSlot(1);
        SaveSystem.RestartGame("MainScene");
        controller.ResetDatabase();
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

