using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PixelCrushers;
using PixelCrushers.DialogueSystem;

public class MainMenu : MonoBehaviour
{

    DialogueSystemController controller;

    private void Awake()
    {
        controller = FindObjectOfType<DialogueSystemController>();
        controller.transform.GetChild(0).gameObject.SetActive(false);
        controller.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void PlayPressed()
    {
        if (SaveSystem.HasSavedGameInSlot(1))
        {
            SaveSystem.LoadFromSlot(1);
        }
        else
            SceneManager.LoadScene("MainScene");

        controller.transform.GetChild(0).gameObject.SetActive(true);
        controller.transform.GetChild(1).gameObject.SetActive(true);

        // очистка сохранения при начале игры заново
        // SaveSystem.DeleteSavedGameInSlot(1);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    

}

