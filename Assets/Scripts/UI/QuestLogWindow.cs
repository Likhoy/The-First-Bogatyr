using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;

public class QuestLogWindow : StandardUIQuestLogWindow
{
    public override void OpenWindow(Action openedWindowHandler)
    {
        
        GameManager.Instance.GetPlayer().GetComponent<UiMaster>().CloseAllMenu();
        //Time.timeScale = 0.0f;
        //GlobalStatus.freezePlayer = true;
        //GlobalStatus.menuOn = true;
        base.OpenWindow(openedWindowHandler);
    }

    //public override void CloseWindow(Action closedWindowHandler)
    //{
    //    Time.timeScale = 1.0f;
    //    Debug.Log("help");
    //    GlobalStatus.freezePlayer = false;
    //    GlobalStatus.menuOn = false;
    //    base.CloseWindow(closedWindowHandler);
    //}
}
