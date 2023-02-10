using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    private int playerMoney;
    
    void Start()
    {
        player = GameManager.Instance.GetPlayer();
        inventory = FindObjectOfType<Inventory>();
        playerMoney = player.playerDetails.playerMoneyAmount;
    }

    private bool SpendMoney(int moneySpent)
    {
        if (playerMoney >= moneySpent)
        {
            playerMoney -= moneySpent;
            return true;
        }
        return false;    
    }

    public void TryBuyProduct(Product product)
    {
        if (SpendMoney(product.price))
        {
            inventory.AddItem(product.itemPrefab.GetComponent<Item>());
            player.productBoughtEvent.CallProductBoughtEvent(playerMoney);
        }
        else
        {
            // send error message
        }
    }
}
