using UnityEngine;

public class OpenBestiariy : MonoBehaviour // можно было обойтись и без этого скрипта, поместив логику в PlayerController
{
    [SerializeField] GameObject bestiary; 
    [SerializeField] GameObject map; // нужно убрать отсюда

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
