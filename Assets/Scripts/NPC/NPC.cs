using PixelCrushers.DialogueSystem;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(DialogueSystemTrigger))]
[RequireComponent(typeof(TradingStageLaunchedEvent))]
#endregion
public class NPC : MonoBehaviour
{
    public NPCDetailsSO npcDetails;
    private DialogueSystemTrigger dialogTrigger;
    public TradingStageLaunchedEvent tradingStageLaunchedEvent;

    private void Awake()
    {
        dialogTrigger = GetComponent<DialogueSystemTrigger>();
        tradingStageLaunchedEvent = GetComponent<TradingStageLaunchedEvent>();
    }

    private void Start()
    {
        dialogTrigger.conversationActor = GameManager.Instance.GetPlayer().gameObject.transform;
    }


}
