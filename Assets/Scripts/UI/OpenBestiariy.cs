using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBestiariy : MonoBehaviour
{
    [SerializeField]
    GameObject bestiary;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            bestiary.SetActive(!bestiary.activeInHierarchy);
    }
}
