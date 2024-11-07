using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MoneyIncreasedEvent))]
[RequireComponent(typeof(ExperienceIncreasedEvent))]
public class PlayerResources : MonoBehaviour
{
    private Player player;
    // private Inventory inventory;
    public int PlayerMoney { get; private set; }
    public int PlayerExperience { get; private set; }

    [HideInInspector] public MoneyIncreasedEvent moneyIncreasedEvent;
    [HideInInspector] public ExperienceIncreasedEvent experienceIncreasedEvent;

    private AudioSource audioEffects;
    [SerializeField] private AudioClip[] CMoney;

    private void Awake()
    {
        // inventory = FindObjectOfType<Inventory>();
        moneyIncreasedEvent = GetComponent<MoneyIncreasedEvent>();
        experienceIncreasedEvent = GetComponent<ExperienceIncreasedEvent>();
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer();
        PlayerMoney = player.playerDetails.initialPlayerMoneyAmount;
        PlayerExperience = player.playerDetails.initialPlayerExperienceAmount;
        experienceIncreasedEvent.CallExperienceIncreasedEvent(PlayerExperience);
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
        /*if (inventory.ContainsItem(itemPrefab.GetComponent<Item>().itemID) >= 1)
            inventory.AddItem(itemPrefab.GetComponent<Item>());
        else
        {
            GameObject item = Instantiate(itemPrefab, transform);
            item.GetComponent<CircleCollider2D>().enabled = false;
            item.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            inventory.AddItem(item.GetComponent<Item>());
        }*/
    }

    public void TryBuyProduct(Product product) // Монеточки
    {
        if (SpendMoney(product.price))
        {
            System.Random rand = new System.Random();
            audioEffects.PlayOneShot(CMoney[rand.Next(CMoney.Length)]);

            SaveItem(product.itemPrefab);
            // player.productBoughtEvent.CallProductBoughtEvent(PlayerMoney);
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

            Weapon weapon = player.FindWeaponByName(weaponProduct.weaponDetails.weaponName);

            if (weapon != null)
            {
                if (weapon is RangedWeapon rangedWeapon)
                {
                    rangedWeapon.weaponRemainingAmmo += weaponProduct.weaponAmmoAmount;
                }
                else
                {
                    // do nothing now - found same melee weapon
                }
                
                // player.setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon, weapon is RangedWeapon);
            }
            else
            {
                player.AddWeaponToPlayer(weaponProduct.weaponDetails, weaponProduct.weaponAmmoAmount);
            }
            
            // player.productBoughtEvent.CallProductBoughtEvent(PlayerMoney);
        }
    }

    public void SetMoney(int newMoneyAmount)
    {
        PlayerMoney = 0;
        AddMoney(newMoneyAmount);
    }

    public void SetExperience(int newExperienceAmount)
    {
        PlayerExperience = 0;
        AddExperience(newExperienceAmount);
    }

    public void AddMoney(int moneyAmount)
    {
        if (moneyAmount > 0)
        {
            PlayerMoney += moneyAmount;
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
