using PixelCrushers.DialogueSystem;
using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(DialogueSystemTrigger))]
#endregion
public class NPC : MonoBehaviour
{
    public NPCDetailsSO npcDetails;
    private DialogueSystemTrigger dialogTrigger;

    private void Awake()
    {
        dialogTrigger = GetComponent<DialogueSystemTrigger>();
    }

    private void Start()
    {
        dialogTrigger.conversationActor = GameManager.Instance.GetPlayer().gameObject.transform;
    }


}
