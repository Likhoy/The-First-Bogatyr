using UnityEngine;

#region REQUIRE COMPONENTS

#endregion REQUIRE COMPONENTS


public class DialogTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag && DialogManager.Instance.isDialogReady)
            GameManager.Instance.GetPlayer().dialogStartedEvent.CallDialogStartedEvent();
    }
}
