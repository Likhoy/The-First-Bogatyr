using PixelCrushers.DialogueSystem;
using System;

public class QuestLogWindow : StandardUIQuestLogWindow
{
    public override void OpenWindow(Action openedWindowHandler)
    {
        GameManager.Instance.GetPlayer().GetComponent<UiMaster>().CloseAllMenu();
        base.OpenWindow(openedWindowHandler);
    }
}
