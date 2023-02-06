using System;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    private TextMeshProUGUI moneyAmount;

    private void Awake()
    {
        moneyAmount = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        moneyAmount.text = GameManager.Instance.GetPlayer().playerDetails.playerMoneyAmount.ToString() + " монет";
    }

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().productBoughtEvent.OnBuyProduct += ProductBoughtEvent_OnBuyProduct;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().productBoughtEvent.OnBuyProduct -= ProductBoughtEvent_OnBuyProduct;
    }

    /// <summary>
    /// Product buying event handler
    /// </summary>
    private void ProductBoughtEvent_OnBuyProduct(ProductBoughtEvent productBoughtEvent, ProductBoughtEventArgs productBoughtEventArgs)
    {
        UpdateMoneyAmount(productBoughtEventArgs.playerMoney);
    }

    /// <summary>
    /// Update money amount text on UI
    /// </summary>
    public void UpdateMoneyAmount(int newMoneyAmount)
    {
        moneyAmount.text = newMoneyAmount.ToString() + " монет";
    }


}
