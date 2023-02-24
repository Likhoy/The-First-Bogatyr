using PixelCrushers.DialogueSystem;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(DialogueSystemTrigger))]
[RequireComponent(typeof(MovementToPositionEvent))]
#endregion
public class NPC : MonoBehaviour
{
    public NPCDetailsSO npcDetails;
    private DialogueSystemTrigger dialogTrigger;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        dialogTrigger = GetComponent<DialogueSystemTrigger>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void Start()
    {
        dialogTrigger.conversationActor = GameManager.Instance.GetPlayer().gameObject.transform;
    }


}
