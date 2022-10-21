using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   
    public void PlayPressed()
    {
        SceneManager.LoadScene("Game");
    }


    public void ExitPressed()
    {
        Application.Quit();
    }
}
