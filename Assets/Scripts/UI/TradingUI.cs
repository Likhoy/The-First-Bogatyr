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
    }


    // maximum ten ordinary products and then weapons
    public void ShowTradingWindow(NPC npc)
    {
        
        this.gameObject.SetActive(true);
        Invoke("DisablePlayer", 0.1f);

        // ordinary products
        int i = 0;
        foreach (Product product in npc.npcDetails.products)
        {
            prices[i].text = product.itemName + "\n" + product.price;
            images[i + 1].sprite = product.sprite;
            buttons[i].onClick.RemoveAllListeners();
            // buttons[i].onClick.AddListener(delegate { GameManager.Instance.GetPlayer().playerResources.TryBuyProduct(product); });
            this.transform.GetChild(i).gameObject.SetActive(true);
            i++;
        }
        if (i > 10)
            Debug.Log("Больше 10 обычных продуктов не поддерживается.");
        
        // weapons
        i = 10;
        foreach (WeaponProduct weaponProduct in npc.npcDetails.weaponProducts)
        {
            string weaponAmmoAmount = weaponProduct.weaponAmmoAmount > 0 ? weaponProduct.weaponAmmoAmount.ToString() + " шт." : "";
            prices[i].text = $"{weaponProduct.weaponDetails.weaponName} {weaponAmmoAmount}\n{weaponProduct.price}";
            images[i + 1].sprite = weaponProduct.weaponDetails.weaponSprite;
            buttons[i].onClick.RemoveAllListeners();
            // buttons[i].onClick.AddListener(delegate { GameManager.Instance.GetPlayer().playerResources.TryBuyWeapon(weaponProduct); });
            this.transform.GetChild(i).gameObject.SetActive(true);
            i++;
        }
    }

    private void DisablePlayer()
    {
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();
    }

}
