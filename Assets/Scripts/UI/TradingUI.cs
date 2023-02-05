using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUI : MonoBehaviour
{
    public void ShowTradingWindow(NPC npc)
    {
        this.gameObject.SetActive(true);
        // Vector2 firstPos = Settings.firstProductPosition;
        int count = 0;
        foreach (Product product in npc.npcDetails.products)
        {
            /*float xOffset = Settings.productsXOffset * count % Settings.productsInRowAmount;
            float yOffset = Settings.productsYOffset * (count / Settings.productsInRowAmount);
            GameObject productGameObject = Instantiate(productPrefab, firstPos + new Vector2(xOffset, yOffset), Quaternion.identity, this.transform);
            Image productImage = productGameObject.GetComponent<Image>();
            TextMeshProUGUI productTextComponent = productGameObject.GetComponentInChildren<TextMeshProUGUI>();
            productTextComponent.text = product.price + " монет";
            productImage.sprite = product.sprite;
            count++;*/
        }
    }

}
