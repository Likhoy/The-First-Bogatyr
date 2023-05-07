using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrushers;
using PixelCrushers.DialogueSystem;

public class MainMenu : MonoBehaviour
{

    DialogueSystemController controller;
    [SerializeField] private GameObject continueButton;

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
        if (SaveSystem.HasSavedGameInSlot(2))
        {
            SaveSystem.LoadFromSlot(2);
        }
        else if (SaveSystem.HasSavedGameInSlot(1))
            SaveSystem.LoadFromSlot(1);

        Invoke("ActivateDialogueSystem", 3);
    }

    private void ActivateDialogueSystem()
    {
        controller.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        controller.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        controller.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void StartNewGamePressed()
    {
        SaveSystem.DeleteSavedGameInSlot(2);
        SaveSystem.RestartGame("MainScene");
        controller.ResetDatabase();
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    

}

