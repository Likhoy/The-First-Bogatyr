using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("2D Action-RPG Kit/Create Shop")]

public class ShopUi : MonoBehaviour {
	public ShopBuyInfo[] shopSlot = new ShopBuyInfo[8];
	public ShopButton[] shopUi = new ShopButton[8];
	public GameObject menuPanel;
	public GameObject shopPanel;
	public GameObject buyErrorPanel;
	public Text cashText;
	public Text buyErrorText;
	public Text pageText;
	private int maxPage = 1;
	private int page = 0;
	private int cPage = 0;
	
	//private int playerMaxPage = 0; // For Shop Sell
	
	public GameObject tooltip;
	public Image tooltipIcon;
	public Text tooltipName;
	public Text tooltipText1;

	public ConfirmationUI buyConfirmation;
	public ConfirmationUI sellConfirmation;
	
	public ItemData database;
	private int mode = 0;//0 = Shop Buy , 1 = Usable Sell , 2 = Equipment Sell

	[System.Serializable]
	public class ShopButton{
		public Image itemIcons;
		public Text itemNameText;
		public Text priceText;
	}
	
	[System.Serializable]
	public class ShopBuyInfo{
		public int itemId = 0;
		public ItType itemType = ItType.Usable; 
	}
	
	[System.Serializable]
	public class ConfirmationUI{
		public GameObject basePanel;
		public Text priceText;
		public InputField inputField;
	}
	
	[HideInInspector]
	public GameObject player;
	// Use this for initialization
	void Start(){
		player = GlobalStatus.mainPlayer;
		SetMaxPage();
		UpdateUi();
	}
	
	public void OpenShop(){
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		GlobalStatus.freezeAll = true;
		GlobalStatus.mainPlayer.GetComponent<UiMaster>().CloseAllMenu();
		menuPanel.SetActive(true);
	}
	
	void SetMaxPage(){
		if(!player){
			return;
		}
		//Set Initial Page
		page = 0;
		cPage = 0;
		//Set Max Page
		if(mode == 0){
			//Shop Page
			maxPage = shopSlot.Length / shopUi.Length;
			if(shopSlot.Length % shopUi.Length != 0){
				maxPage += 1;
			}
		}
		if(mode == 1){
			//Sell Usable Item
			maxPage = player.GetComponent<Inventory>().itemSlot.Length / shopUi.Length;
			if(player.GetComponent<Inventory>().itemSlot.Length % shopUi.Length != 0){
				maxPage += 1;
			}
		}
		if(mode == 2){
			//Sell Equipment
			maxPage = player.GetComponent<Inventory>().equipment.Length / shopUi.Length;
			if(player.GetComponent<Inventory>().equipment.Length % shopUi.Length != 0){
				maxPage += 1;
			}
		}
		//print(maxPage);
	}
	
	void Update(){
		if(tooltip && tooltip.activeSelf == true) {
			Vector2 tooltipPos = Input.mousePosition;
			tooltipPos.x += 7;
			tooltip.transform.position = tooltipPos;
		}
		
		if(buyConfirmation.basePanel.activeSelf || sellConfirmation.basePanel.activeSelf){
			if(Input.GetKeyDown(KeyCode.UpArrow)){
				QuantityPlus(10);
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)){
				QuantityPlus(1);
			}
			if(Input.GetKeyDown(KeyCode.DownArrow)){
				QuantitySubtract(10);
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)){
				QuantitySubtract(1);
			}
		}
	}
	
	public void SwitchMode(int m){
		player = GlobalStatus.mainPlayer;
		mode = m;
		SetMaxPage();
		UpdateUi();
		if(pageText){
			pageText.GetComponent<Text>().text = "1";
		}
		if(cashText){
			cashText.text = "$ : " + player.GetComponent<Inventory>().cash.ToString();
		}
	}
	
	public void ShopBuy(){
		if(!player){
			player = GlobalStatus.mainPlayer;
		}
		int price = 0;
		int id = shopSlot[pickupSlot].itemId;
		if(shopSlot[pickupSlot].itemType == ItType.Usable){
			price = database.usableItem[shopSlot[pickupSlot].itemId].price * pickupQuan;
		}else{
			price = database.equipment[shopSlot[pickupSlot].itemId].price;
		}
		
		if(player.GetComponent<Inventory>().cash < price){
			//If not enough cash
			buyErrorPanel.SetActive(true);
			buyErrorText.text = "Not Enough Cash";
			return;
		}
		
		if(shopSlot[pickupSlot].itemType == ItType.Usable){
			//Buy Usable Item	
			bool full = player.GetComponent<Inventory>().AddItem(id , pickupQuan);
			if(full){
				buyErrorPanel.SetActive(true);
				buyErrorText.text = "Inventory Full";
				return;
			}
		}else{
			//Buy Equipment
			bool full = player.GetComponent<Inventory>().AddEquipment(id);
			if(full){
				buyErrorText.text = "Inventory Full";
				return;
			}
		}
		//Remove Cash
		player.GetComponent<Inventory>().cash -= price;
		if(cashText){
			cashText.text = "$ : " + player.GetComponent<Inventory>().cash.ToString();
		}
	}
	
	public void ShopSell(){
		int price = 0;
		if(mode == 1){
			//Sell Usable Item
			int id = player.GetComponent<Inventory>().itemSlot[pickupSlot];
			price = database.usableItem[id].price * pickupQuan / 2;
			
			if(pickupQuan >= player.GetComponent<Inventory>().itemQuantity[pickupSlot]){
				pickupQuan = player.GetComponent<Inventory>().itemQuantity[pickupSlot];
			}
			player.GetComponent<Inventory>().itemQuantity[pickupSlot] -= pickupQuan;
			if(player.GetComponent<Inventory>().itemQuantity[pickupSlot] <= 0){
				player.GetComponent<Inventory>().itemSlot[pickupSlot] = 0;
				player.GetComponent<Inventory>().itemQuantity[pickupSlot] = 0;
				player.GetComponent<Inventory>().AutoSortItem();
			}
			player.GetComponent<Inventory>().UpdateAmmoUI();
			//Add Cash
			player.GetComponent<Inventory>().cash += price;
			
		}else if(mode == 2){
			//Sell Equipment
			int id = player.GetComponent<Inventory>().equipment[pickupSlot];
			price = database.equipment[id].price / 2;
			
			player.GetComponent<Inventory>().equipment[pickupSlot] = 0;
			player.GetComponent<Inventory>().AutoSortEquipment();
			
			//Add Cash
			player.GetComponent<Inventory>().cash += price;
		}
		UpdateUi();
		if(cashText){
			cashText.text = "$ : " + player.GetComponent<Inventory>().cash.ToString();
		}
	}
	
	public void UpdateUi(){
		if(!player){
			return;
		}
		if(mode == 0){
			//Shop Buy
			for(int a = 0; a < shopUi.Length; a++){
				if(a + cPage < shopSlot.Length && shopSlot[a + cPage].itemId > 0){
					if(shopSlot[a + cPage].itemType == ItType.Usable){
						//Usable Item Shop
						shopUi[a].itemIcons.sprite = database.usableItem[shopSlot[a + cPage].itemId].icon;

						shopUi[a].itemNameText.text = database.usableItem[shopSlot[a + cPage].itemId].itemName;
						shopUi[a].priceText.text = ": " + (database.usableItem[shopSlot[a + cPage].itemId].price).ToString();
					}else{
						//Equipment Shop
						shopUi[a].itemIcons.sprite = database.equipment[shopSlot[a + cPage].itemId].icon;

						shopUi[a].itemNameText.text = database.equipment[shopSlot[a + cPage].itemId].itemName;
						shopUi[a].priceText.text = ": " + (database.equipment[shopSlot[a + cPage].itemId].price).ToString();
					}
				}else{
					//Out of Range
					shopUi[a].itemIcons.sprite = database.usableItem[0].icon;
					shopUi[a].itemNameText.text = "";
					shopUi[a].priceText.text = "";
				}
			}
		}
		if(mode == 1){
			//Sell Usable Item
			for(int a = 0; a < shopUi.Length; a++){
				if(a + cPage < player.GetComponent<Inventory>().itemSlot.Length && player.GetComponent<Inventory>().itemSlot[a + cPage] > 0){
					shopUi[a].itemIcons.sprite = database.usableItem[player.GetComponent<Inventory>().itemSlot[a + cPage]].icon;

					shopUi[a].itemNameText.text = database.usableItem[player.GetComponent<Inventory>().itemSlot[a + cPage]].itemName + " x " + player.GetComponent<Inventory>().itemQuantity[a + cPage].ToString();
					shopUi[a].priceText.text = ": " + (database.usableItem[player.GetComponent<Inventory>().itemSlot[a + cPage]].price /2).ToString();
				}else{
					//Out of Range
					shopUi[a].itemIcons.sprite = database.usableItem[0].icon;
					shopUi[a].itemNameText.text = "";
					shopUi[a].priceText.text = "";
				}
			}
		}
		if(mode == 2){
			//Sell Equipment
			for(int a = 0; a < shopUi.Length; a++){
				if(a + cPage < player.GetComponent<Inventory>().equipment.Length && player.GetComponent<Inventory>().equipment[a + cPage] > 0){
					shopUi[a].itemIcons.sprite = database.equipment[player.GetComponent<Inventory>().equipment[a + cPage]].icon;

					shopUi[a].itemNameText.text = database.equipment[player.GetComponent<Inventory>().equipment[a + cPage]].itemName;
					shopUi[a].priceText.text = ": " + (database.equipment[player.GetComponent<Inventory>().equipment[a + cPage]].price /2).ToString();
				}else{
					//Out of Range
					shopUi[a].itemIcons.sprite = database.equipment[0].icon;
					shopUi[a].itemNameText.text = "";
					shopUi[a].priceText.text = "";
				}
			}
		}
	}
	
	public void ShowTooltip(int slot){
		if(!tooltip || !player || mode == 0 && slot + cPage >= shopSlot.Length){
			return;
		}
		slot += cPage;
		if(mode == 0 && shopSlot[slot].itemType == ItType.Usable){
			if(shopSlot[slot].itemId <= 0 || slot >= shopSlot.Length){
				HideTooltip();
				return;
			}
			tooltipIcon.sprite = database.usableItem[shopSlot[slot].itemId].icon;
			tooltipName.text = database.usableItem[shopSlot[slot].itemId].itemName;
			
			tooltipText1.text = database.usableItem[shopSlot[slot].itemId].description;

			tooltip.SetActive(true);
		}
		if(mode == 0 && shopSlot[slot].itemType == ItType.Equipment){
			if(shopSlot[slot].itemId <= 0 || slot >= shopSlot.Length){
				HideTooltip();
				return;
			}
			tooltipIcon.sprite = database.equipment[shopSlot[slot].itemId].icon;
			tooltipName.text = database.equipment[shopSlot[slot].itemId].itemName;
			
			tooltipText1.text = database.equipment[shopSlot[slot].itemId].description;

			tooltip.SetActive(true);
		}
		
		if(mode == 1){
			if(player.GetComponent<Inventory>().itemSlot[slot] <= 0 || slot >= player.GetComponent<Inventory>().itemSlot.Length){
				HideTooltip();
				return;
			}
			
			tooltipIcon.sprite = database.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].icon;
			tooltipName.text = database.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].itemName;
			
			tooltipText1.text = database.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].description;

			tooltip.SetActive(true);
		}
		if(mode == 2){
			if(player.GetComponent<Inventory>().equipment[slot] <= 0 || slot >= player.GetComponent<Inventory>().equipment.Length){
				HideTooltip();
				return;
			}
			
			tooltipIcon.sprite = database.equipment[player.GetComponent<Inventory>().equipment[slot]].icon;
			tooltipName.text = database.equipment[player.GetComponent<Inventory>().equipment[slot]].itemName;
			
			tooltipText1.text = database.equipment[player.GetComponent<Inventory>().equipment[slot]].description;

			tooltip.SetActive(true);
		}
	}
	private int pickupSlot = 0;
	private int pickupQuan = 1;
	
	public void ButtonClick(int slot){
		pickupSlot = slot + cPage;
		pickupQuan = 1;
		buyErrorPanel.SetActive(false);
		if(pickupSlot >= shopSlot.Length){
			return;
		}
		
		if(mode == 0){
			shopPanel.SetActive(false);
			if(shopSlot[pickupSlot].itemType == ItType.Usable){
				buyConfirmation.basePanel.SetActive(true);
				if(buyConfirmation.inputField){
					buyConfirmation.inputField.gameObject.SetActive(true);
					buyConfirmation.inputField.text = pickupQuan.ToString();
				}
				if(buyConfirmation.priceText){
					buyConfirmation.priceText.text = database.usableItem[shopSlot[pickupSlot].itemId].price.ToString();
				}
			}
			if(shopSlot[pickupSlot].itemType == ItType.Equipment){
				buyConfirmation.basePanel.SetActive(true);
				if(buyConfirmation.inputField){
					buyConfirmation.inputField.gameObject.SetActive(false);
				}
				if(buyConfirmation.priceText){
					buyConfirmation.priceText.text = database.equipment[shopSlot[pickupSlot].itemId].price.ToString();
				}
			}
			
		}
		if(mode == 1){
			if(player.GetComponent<Inventory>().itemSlot[pickupSlot] <= 0){
				return;
			}
			shopPanel.SetActive(false);
			sellConfirmation.basePanel.SetActive(true);
			if(sellConfirmation.inputField){
				sellConfirmation.inputField.gameObject.SetActive(true);
				sellConfirmation.inputField.text = pickupQuan.ToString();
			}
			if(sellConfirmation.priceText){
				sellConfirmation.priceText.text = (database.usableItem[player.GetComponent<Inventory>().itemSlot[pickupSlot]].price / 2).ToString();
			}
		}
		if(mode == 2){
			if(player.GetComponent<Inventory>().equipment[pickupSlot] <= 0){
				return;
			}
			shopPanel.SetActive(false);
			sellConfirmation.basePanel.SetActive(true);
			if(sellConfirmation.inputField){
				sellConfirmation.inputField.gameObject.SetActive(false);
			}
			if(sellConfirmation.priceText){
				sellConfirmation.priceText.text = (database.equipment[player.GetComponent<Inventory>().equipment[pickupSlot]].price / 2).ToString();
			}
		}
	}
	
	public void HideTooltip(){
		if(!tooltip){
			return;
		}
		tooltip.SetActive(false);
	}
	
	public void CloseMenu(){
		Time.timeScale = 1.0f;
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		GlobalStatus.freezeAll = false;
		//gameObject.SetActive(false);
	}
	
	public void NextPage(){
		if(page < maxPage -1){
			page++;
			cPage = page * shopUi.Length;
		}
		if(pageText){
			int p = page + 1;
			pageText.GetComponent<Text>().text = p.ToString();
		}
		UpdateUi();
	}
	
	public void PreviousPage(){
		if(page > 0){
			page--;
			cPage = page * shopUi.Length;
		}
		if(pageText){
			int p = page + 1;
			pageText.GetComponent<Text>().text = p.ToString();
		}
		UpdateUi();
	}
	
	public void QuantityPlus(int val){
		int currentQty = pickupQuan;
		pickupQuan += val;
		if(mode == 0){
			if(pickupQuan > 99){
				pickupQuan = 1;
			}
			if(buyConfirmation.priceText && shopSlot[pickupSlot].itemType == ItType.Usable){
				buyConfirmation.priceText.text = (database.usableItem[shopSlot[pickupSlot].itemId].price * pickupQuan).ToString();
			}
		}
		if(mode == 1){
			if(pickupQuan > player.GetComponent<Inventory>().itemQuantity[pickupSlot] && currentQty == player.GetComponent<Inventory>().itemQuantity[pickupSlot]){
				pickupQuan = 1;
			}else if(pickupQuan > player.GetComponent<Inventory>().itemQuantity[pickupSlot]){
				pickupQuan = player.GetComponent<Inventory>().itemQuantity[pickupSlot];
			}
			if(sellConfirmation.priceText){
				sellConfirmation.priceText.text = (database.usableItem[player.GetComponent<Inventory>().itemSlot[pickupSlot]].price * pickupQuan / 2).ToString();
			}
		}
		
		if(buyConfirmation.inputField){
			buyConfirmation.inputField.text = pickupQuan.ToString();
		}
		if(sellConfirmation.inputField){
			sellConfirmation.inputField.text = pickupQuan.ToString();
		}
	}
	
	public void QuantitySubtract(int val){
		int currentQty = pickupQuan;
		pickupQuan -= val;
		if(mode == 0){
			if(pickupQuan <= 0){
				pickupQuan = 99;
			}
			if(buyConfirmation.priceText && shopSlot[pickupSlot].itemType == ItType.Usable){
				buyConfirmation.priceText.text = (database.usableItem[shopSlot[pickupSlot].itemId].price * pickupQuan).ToString();
			}
		}
		if(mode == 1){
			if(pickupQuan <= 0 && currentQty == 1){
				pickupQuan = player.GetComponent<Inventory>().itemQuantity[pickupSlot];
			}else if(pickupQuan <= 0){
				pickupQuan = 1;
			}
			if(sellConfirmation.priceText){
				sellConfirmation.priceText.text = (database.usableItem[player.GetComponent<Inventory>().itemSlot[pickupSlot]].price * pickupQuan / 2).ToString();
			}
		}
		
		if(buyConfirmation.inputField){
			buyConfirmation.inputField.text = pickupQuan.ToString();
		}
		if(sellConfirmation.inputField){
			sellConfirmation.inputField.text = pickupQuan.ToString();
		}
	}
	
	public void QuantityInput(string val){
		if(val == ""){
			val = "1";
		}
		int v = int.Parse(val);
		pickupQuan = v;
		if(mode == 0){
			if(pickupQuan < 1){
				pickupQuan = 1;
			}
			if(buyConfirmation.priceText && shopSlot[pickupSlot].itemType == ItType.Usable){
				buyConfirmation.priceText.text = (database.usableItem[shopSlot[pickupSlot].itemId].price * pickupQuan).ToString();
			}
		}
		if(mode == 1){
			if(pickupQuan > player.GetComponent<Inventory>().itemQuantity[pickupSlot]){
				pickupQuan = player.GetComponent<Inventory>().itemQuantity[pickupSlot];
			}
			if(pickupQuan < 1){
				pickupQuan = 1;
			}
			if(sellConfirmation.priceText){
				sellConfirmation.priceText.text = (database.usableItem[player.GetComponent<Inventory>().itemSlot[pickupSlot]].price * pickupQuan / 2).ToString();
			}
		}
		
		if(buyConfirmation.inputField){
			buyConfirmation.inputField.text = pickupQuan.ToString();
		}
		if(sellConfirmation.inputField){
			sellConfirmation.inputField.text = pickupQuan.ToString();
		}
	}
}
