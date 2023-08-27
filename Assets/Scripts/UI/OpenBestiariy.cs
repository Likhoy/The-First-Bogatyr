using UnityEngine;

public class OpenBestiariy : MonoBehaviour // ����� ���� �������� � ��� ����� �������, �������� ������ � PlayerController
{
    [SerializeField] GameObject bestiary;
    [SerializeField] GameObject map;
   
    void Update()
    {
        if (Input.GetKeyDown(Settings.commandButtons[Command.OpenBestiary]))
            bestiary.SetActive(!bestiary.activeInHierarchy); 

        if(bestiary.activeInHierarchy)
            map.SetActive(false);           
    }
}
