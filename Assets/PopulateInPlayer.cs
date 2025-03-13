using UnityEngine;
using UnityEngine.UI;

public class PopulateInPlayer : MonoBehaviour
{
    private void Awake()
    {
        Player player = GameManager.Instance.GetPlayer();

        var attackTrigger = player.GetComponent<AttackTrigger>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            var children = new Transform[] { child.GetChild(1), child.GetChild(3), child.GetChild(3).GetChild(0), child.GetChild(2) };

            attackTrigger.shortcutUi[i].iconImage = children[0].GetComponent<Image>();
            attackTrigger.shortcutUi[i].coolDownBackground = children[1].gameObject;
            attackTrigger.shortcutUi[i].coolDownText = children[2].GetComponent<Text>();
            attackTrigger.shortcutUi[i].quantityText = children[3].GetComponent<Text>();
        }

        
    }
}
