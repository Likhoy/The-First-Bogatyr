using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMap : MonoBehaviour
{
    [SerializeField] GameObject map;
    [SerializeField] GameObject bestiary;

    void Update()
    {
        if (Input.GetKeyDown(Settings.commandButtons[Command.OpenMap]))
        {
            if (!map.activeInHierarchy)
                Time.timeScale = 0;
            else
                Time.timeScale = 1f;
            map.SetActive(!map.activeInHierarchy);
        }

        if(map.activeInHierarchy)
            bestiary.SetActive(false);
    }
}
