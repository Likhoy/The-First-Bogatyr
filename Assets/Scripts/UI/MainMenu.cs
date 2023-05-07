using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    DialogueSystemController controller;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject exitButton;

    private void Awake()
    {
        if (!SaveSystem.HasSavedGameInSlot(2))
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

    public void PlayPressed()
    {
        Button b = continueButton.GetComponent<Button>();
        ColorBlock cb = b.colors;
        cb.normalColor = new Color(115, 115, 115);
        b.colors = cb;
        b.interactable = false;

        newGameButton.GetComponent<Button>().enabled = false;
        settingsButton.GetComponent<Button>().enabled = false;
        exitButton.GetComponent<Button>().enabled = false;

        SaveSystem.LoadFromSlot(2);

        controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        controller.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void StartNewGamePressed()
    {
        SaveSystem.DeleteSavedGameInSlot(2);
        SaveSystem.DeleteSavedGameInSlot(1);
        SaveSystem.RestartGame("MainScene");
        controller.ResetDatabase();
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    

}

