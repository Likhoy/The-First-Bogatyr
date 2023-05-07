using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MoneyIncreasedEvent))]
public class PlayerResources : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    private int playerMoney;
    public int PlayerMoney { get => playerMoney; }

    [HideInInspector] public MoneyIncreasedEvent moneyIncreasedEvent;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        moneyIncreasedEvent = GetComponent<MoneyIncreasedEvent>();
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer();
        playerMoney = player.playerDetails.initialPlayerMoneyAmount;
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
            GameManager.Instance.GiveItem(product.itemPrefab);
            player.productBoughtEvent.CallProductBoughtEvent(playerMoney);
        }
        else
        {
            // send error message
        }
    }

    internal void AddMoney(int moneyAmount)
    {
        if (moneyAmount > 0)
            playerMoney += moneyAmount;
    }
}
