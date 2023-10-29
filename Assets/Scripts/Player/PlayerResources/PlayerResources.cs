using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MoneyIncreasedEvent))]
[RequireComponent(typeof(ExperienceIncreasedEvent))]
public class PlayerResources : MonoBehaviour
{
    private Player player;
    private Inventory inventory;
    public int PlayerMoney { get; set; }
    public int PlayerExperience { get; set; }

    [HideInInspector] public MoneyIncreasedEvent moneyIncreasedEvent;
    [HideInInspector] public ExperienceIncreasedEvent experienceIncreasedEvent;

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
        PlayerExperience = player.playerDetails.initialPlayerExperienceAmount;
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
    
    private bool SpendExperience(int experienceSpent)
    {
        if (PlayerExperience >= experienceSpent)
        {
            PlayerExperience -= experienceSpent;
            return true;
        }
        return false;
    }

    public void SaveItem(GameObject itemPrefab)
    {
        if (inventory.ContainsItem(itemPrefab.GetComponent<Item>().itemID) >= 1)
            inventory.AddItem(itemPrefab.GetComponent<Item>());
        else
        {
            GameObject item = Instantiate(itemPrefab, transform);
            item.GetComponent<CircleCollider2D>().enabled = false;
            item.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            inventory.AddItem(item.GetComponent<Item>());
        }
    }

    public void TryBuyProduct(Product product) // Монеточки
    {
        if (SpendMoney(product.price))
        {
            System.Random rand = new System.Random();
            audioEffects.PlayOneShot(CMoney[rand.Next(CMoney.Length)]);

            SaveItem(product.itemPrefab);
            player.productBoughtEvent.CallProductBoughtEvent(PlayerMoney);
        }
        else
        {
            // send error message
        }
    }

    public void TryBuyWeapon(WeaponProduct weaponProduct)
    {
        if (SpendMoney(weaponProduct.price))
        {
            System.Random rand = new System.Random();
            audioEffects.PlayOneShot(CMoney[rand.Next(CMoney.Length)]);

            player.AddWeaponToPlayer(weaponProduct.weaponDetails, weaponProduct.weaponAmmoAmount);
            player.productBoughtEvent.CallProductBoughtEvent(PlayerMoney);
        }
    }

    public void AddMoney(int moneyAmount)
    {
        if (moneyAmount > 0)
        {
            PlayerExperience += moneyAmount;
            moneyIncreasedEvent.CallMoneyIncreasedEvent(PlayerMoney);
        }
    }
    
    public void AddExperience(int experienceAmount)
    {
        if (experienceAmount > 0)
        {
            PlayerExperience += experienceAmount;
            experienceIncreasedEvent.CallExperienceIncreasedEvent(PlayerExperience);
        }
    }
}
