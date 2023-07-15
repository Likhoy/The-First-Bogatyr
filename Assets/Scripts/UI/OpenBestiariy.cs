using UnityEngine;

public class OpenBestiariy : MonoBehaviour // ����� ���� �������� � ��� ����� �������, �������� ������ � PlayerController
{
    [SerializeField] GameObject bestiary; 
    [SerializeField] GameObject map; // ����� ������ ������

    void Update()
    {
        if (Input.GetKeyDown(Settings.commandButtons[Command.OpenBestiary]))
            bestiary.SetActive(!bestiary.activeInHierarchy);

        if (Input.GetKeyDown(Settings.commandButtons[Command.OpenMap]))
        {
            if (!map.activeInHierarchy)
                Time.timeScale = 0;
            else
                Time.timeScale = 1f;
            map.SetActive(!map.activeInHierarchy);
        }
            
    }
}
