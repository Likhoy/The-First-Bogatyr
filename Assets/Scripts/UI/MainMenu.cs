using UnityEngine;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private DialogueSystemController controller;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject exitButton;

    private void Awake()
    {
        if (!SaveSystem.HasSavedGameInSlot(1))
            continueButton.SetActive(false);
        controller = FindObjectOfType<DialogueSystemController>();
        controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    private void Start()
    {
        controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        controller.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ContinueGamePressed()
    {
        Button b = continueButton.GetComponent<Button>();
        ColorBlock cb = b.colors;
        cb.normalColor = new Color(115, 115, 115);
        b.colors = cb;
        b.interactable = false;

        newGameButton.GetComponent<Button>().enabled = false;
        settingsButton.GetComponent<Button>().enabled = false;
        exitButton.GetComponent<Button>().enabled = false;

        SaveSystem.LoadFromSlot(1);

        controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        controller.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void StartNewGamePressed()
    {
        SaveSystem.DeleteSavedGameInSlot(1);
        SaveSystem.RestartGame("MainScene");
        controller.ResetDatabase();
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    

}

