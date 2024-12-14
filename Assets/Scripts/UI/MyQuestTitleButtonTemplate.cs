using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyQuestTitleButtonTemplate : StandardUIQuestTitleButtonTemplate
{
    public Image newUpdateIcon; //<--Assign in inspector.

    public override void Assign(string questName, string displayName, ToggleChangedDelegate trackToggleDelegate)
    {
        base.Assign(questName, displayName, trackToggleDelegate);
        newUpdateIcon.enabled = !QuestLog.WasQuestViewed(questName);
    }
}
