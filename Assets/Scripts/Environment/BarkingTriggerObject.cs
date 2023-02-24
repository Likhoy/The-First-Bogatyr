using PixelCrushers.DialogueSystem;
using UnityEngine;

public class BarkingTriggerObject : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<DialogueSystemTrigger>().barker = GameManager.Instance.GetPlayer().transform;
    }
}
