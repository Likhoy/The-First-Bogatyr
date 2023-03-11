using PixelCrushers.DialogueSystem;
using UnityEngine;

public class BarkingTriggerObject : MonoBehaviour
{
    private void Awake()
    {
        var dialogTrigger = GetComponent<DialogueSystemTrigger>();
        var barkOnIdle = GetComponent<BarkOnIdle>();
        if (dialogTrigger != null)
            dialogTrigger.barker = GameManager.Instance.GetPlayer().transform;
        if (barkOnIdle != null)
            barkOnIdle.conversant = GameManager.Instance.GetPlayer().transform;
    }
}
