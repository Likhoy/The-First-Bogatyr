using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MoneyIncreasedEvent))]
public class PlayerResources : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    public int PlayerMoney { get; set; }

    [HideInInspector] public MoneyIncreasedEvent moneyIncreasedEvent;

    private AudioSource audioEffects;
    [SerializeField] private AudioClip[] CMoney;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        moneyIncreasedEvent = GetComponent<MoneyIncreasedEvent>();
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer();
        PlayerMoney = player.playerDetails.initialPlayerMoneyAmount;
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
    }

    private bool SpendMoney(int moneySpent)
    {
        if (PlayerMoney >= moneySpent)
        {
            PlayerMoney -= moneySpent;
            return true;
        }
        return false;    
    }

    public void TryBuyProduct(Product product) // Монеточки
    {
        if (SpendMoney(product.price))
        {
            System.Random rand = new System.Random();
            audioEffects.PlayOneShot(CMoney[rand.Next(CMoney.Length)]);
            GameManager.Instance.GiveItem(product.itemPrefab);
            player.productBoughtEvent.CallProductBoughtEvent(PlayerMoney);
        }
        else
        {
            // send error message
        }
    }

    internal void AddMoney(int moneyAmount)
    {
        if (moneyAmount > 0)
            PlayerMoney += moneyAmount;
    }
}
