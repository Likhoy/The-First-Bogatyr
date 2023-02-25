using PixelCrushers.DialogueSystem;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(DialogueSystemTrigger))]
#endregion
public class NPC : MonoBehaviour
{
    public NPCDetailsSO npcDetails;
    private DialogueSystemTrigger[] dialogTriggers;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        dialogTriggers = GetComponents<DialogueSystemTrigger>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void Start()
    {
        
        if (npcDetails.hasDialogWithPlayer)
            foreach (var dialogTrigger in dialogTriggers)
                dialogTrigger.conversationActor = GameManager.Instance.GetPlayer().gameObject.transform;
    }


}
