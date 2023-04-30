using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBestiariy : MonoBehaviour
{
    [SerializeField]
    GameObject bestiary;
    [SerializeField]
    GameObject map;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            bestiary.SetActive(!bestiary.activeInHierarchy);

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!map.activeInHierarchy)
                Time.timeScale = 0;
            else
                Time.timeScale = 1f;
            map.SetActive(!map.activeInHierarchy);
        }
            
    }
}
