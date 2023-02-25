using UnityEngine;
using UnityEngine.UI;

public class TradingButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate { GameManager.Instance.GetPlayer().playerControl.DisablePlayer(); });
    }
}
