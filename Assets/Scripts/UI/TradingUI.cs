using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUI : MonoBehaviour
{
    private Image[] images;
    private TextMeshProUGUI[] prices;
    private Button[] buttons;

    private void Awake()
    {
        // Cash components
        images = GetComponentsInChildren<Image>(true);
        prices = GetComponentsInChildren<TextMeshProUGUI>(true);
        buttons = GetComponentsInChildren<Button>(true);
        buttons.Last().onClick.AddListener(delegate { GameManager.Instance.GetPlayer().playerControl.EnablePlayer(); });
    }


    public void ShowTradingWindow(NPC npc)
    {
        int i = 0;
        this.gameObject.SetActive(true);
        foreach (Product product in npc.npcDetails.products)
        {
            prices[i].text = product.price.ToString();
            images[i + 1].sprite = product.sprite;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(delegate { GameManager.Instance.GetPlayer().playerResources.TryBuyProduct(product); });
            this.transform.GetChild(i).gameObject.SetActive(true);
            i++;
        }
        
    }

    

}
