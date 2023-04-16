using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMapPosition : MonoBehaviour
{
    private float mapWidth;
    private float mapHeight;

    private void Awake()
    {
        mapWidth = (transform.parent as RectTransform).rect.width;
        mapHeight = (transform.parent as RectTransform).rect.height;
    }

    private void OnEnable()
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        transform.localPosition = new Vector3(playerPosition.x * mapWidth / LocationInfo.realLocationWidth, playerPosition.y * mapHeight / LocationInfo.realLocationHeight, 0);
    }

    
}
