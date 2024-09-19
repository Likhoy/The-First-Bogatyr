using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUi : MonoBehaviour {
	public int[] craftingListId = new int[3];
	public CraftingButton[] craftMenuUi = new CraftingButton[8];
	public CraftingButton[] ingredientUi = new CraftingButton[5];
	public Text pageText;
	
	public GameObject menuPanel;
	public GameObject ingredientPanel;
	public Text gotItemText;
	
	private CraftData[] craftingData;
	public ItemData itemDatabase;
	public CraftingData craftingDatabase;
	private GameObject player;
	
	public GameObject tooltip;
	public Image tooltipIcon;
	public Text tooltipName;
	public Text tooltipText1;

	private int uiPage = 0;
	private int page = 0;
	private int maxPage = 1;
	private int selection = 0;
	private int cPage = 0;
	
	private int itemQty1;
	private int itemQty2;
	private int itemQty3;
	private int itemQty4;
	private int itemQty5;
	
	public Color haveAllItemColor = Color.black;
	public Color notEnoughtItemColor = Color.red;
	
	void Start(){
		player = GlobalStatus.mainPlayer;
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
		GetCraftingData();
		//uiPage = 1;
	}
	
	public void OpenCraftMenu(){
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		GlobalStatus.freezeAll = true;
		uiPage = 0;
		UpdateUi();
		menuPanel.SetActive(true);
		ingredientPanel.SetActive(false);
		gotItemText.gameObject.SetActive(false);
	}
	
	public void CloseIngredientPanel(){
		uiPage = 0;
		menuPanel.SetActive(true);
		ingredientPanel.SetActive(false);
		gotItemText.gameObject.SetActive(false);
	}
	
	void GetCraftingData(){
		craftingData = new CraftData[craftingListId.Length];
		int a = 0;
		while(a < craftingData.Length){
			craftingData[a] = craftingDatabase.craftingData[craftingListId[a]];
			a++;
		}
		//Set Max Page
		maxPage = craftingData.Length / craftMenuUi.Length;
		if(craftingData.Length % craftMenuUi.Length != 0){
			maxPage += 1;
		}
		print(maxPage);
	}
	
	public void UpdateUi(){
		if(!GlobalStatus.mainPlayer){
			return;
		}
		if(uiPage == 0){
			for(int a = 0; a < craftMenuUi.Length; a++){
				if(a + cPage < craftingListId.Length){
					if(craftingDatabase.craftingData[craftingListId[a + cPage]].gotItem.itemType == ItType.Usable){
						//Usable Item Shop
						craftMenuUi[a].itemIcons.sprite = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[a + cPage]].gotItem.itemId].icon;
						craftMenuUi[a].itemNameText.text = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[a + cPage]].gotItem.itemId].itemName;
					}else{
						//Equipment Shop
						craftMenuUi[a].itemIcons.sprite = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[a + cPage]].gotItem.itemId].icon;
						craftMenuUi[a].itemNameText.text = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[a + cPage]].gotItem.itemId].itemName;
					}
				}else{
					//Out of Range
					craftMenuUi[a].itemIcons.sprite = itemDatabase.usableItem[0].icon;
					craftMenuUi[a].itemNameText.text = "";
				}
			}
		}
		
		//Show Ingredient
		if(uiPage == 1){
			for(int a = 0; a < ingredientUi.Length; a++){
				if(a < craftingDatabase.craftingData[craftingListId[selection]].ingredient.Length){
					if(craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemType == ItType.Usable){
						string qty = craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].quantity.ToString();
						string itemQty = ShowItemQuantity(a).ToString();
						ingredientUi[a].itemNameText.text = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemId].itemName + " x " + qty + " (" + itemQty + ")";
						ingredientUi [a].itemIcons.sprite = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemId].icon;
						
						if(int.Parse(itemQty) >= int.Parse(qty)){
							ingredientUi[a].itemNameText.color = haveAllItemColor;
						}else{
							ingredientUi[a].itemNameText.color = notEnoughtItemColor;
						}
					}else{
						string itemQty = CheckEquipment(a);
						ingredientUi[a].itemNameText.text = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemId].itemName + " x " + 1 + " (" + itemQty + ")";
						ingredientUi [a].itemIcons.sprite = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemId].icon;
						
						if(int.Parse(itemQty) >= 1){
							ingredientUi[a].itemNameText.color = haveAllItemColor;
						}else{
							ingredientUi[a].itemNameText.color = notEnoughtItemColor;
						}
					}
				}else{
					ingredientUi[a].itemNameText.text = "";
					//ingredientUi[a].itemIcons.sprite = null;
					ingredientUi[a].itemIcons.sprite = itemDatabase.usableItem[0].icon;
				}
			}
		}
	}
	
	int ShowItemQuantity(int b){
		player = GlobalStatus.mainPlayer;

		int qty = 0;
		int a = player.GetComponent<Inventory>().FindItemSlot(craftingDatabase.craftingData[craftingListId[selection]].ingredient[b].itemId);
		if(a <= player.GetComponent<Inventory>().itemSlot.Length){
			qty = player.GetComponent<Inventory>().itemQuantity[a];
		}
		return qty;
	}
	
	string CheckEquipment(int b){
		player = GlobalStatus.mainPlayer;

		string qty = "0";
		int a = player.GetComponent<Inventory>().FindEquipmentSlot(craftingDatabase.craftingData[craftingListId[selection]].ingredient[b].itemId);
		if(a <= player.GetComponent<Inventory>().equipment.Length){
			qty = "1";
		}
		return qty;
	}
	
	// Update is called once per frame
	void Update(){
		if(tooltip && tooltip.activeSelf == true){
			Vector2 tooltipPos = Input.mousePosition;
			tooltipPos.x += 7;
			tooltip.transform.position = tooltipPos;
		}
	}
	
	public void ButtonClick(int slot){
		selection = slot + cPage;
		if(selection >= craftingListId.Length){
			return;
		}
		uiPage = 1;
		menuPanel.SetActive(false);
		ingredientPanel.SetActive(true);
		UpdateUi();
		gotItemText.gameObject.SetActive(false);
		HideTooltip();
	}
	
	public void NextPage(){
		if(page < maxPage -1){
			page++;
			cPage = page * craftMenuUi.Length;
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
			cPage = page * craftMenuUi.Length;
		}
		if(pageText){
			int p = page + 1;
			pageText.GetComponent<Text>().text = p.ToString();
		}
		UpdateUi();
	}
	
	public void CloseMenu(){
		Time.timeScale = 1.0f;
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		GlobalStatus.freezeAll = false;
		menuPanel.gameObject.SetActive(false);
		ingredientPanel.gameObject.SetActive(false);
		gotItemText.gameObject.SetActive(false);
		//gameObject.SetActive(false);
	}
	
	public void CraftItem(){
		bool canCraft = CheckIngredients();
		if(canCraft){
			AddandRemoveItem();
		}
		//Show Items Quantity
		for(int a = 0; a < craftingDatabase.craftingData[craftingListId[selection]].ingredient.Length; a++){
			if(craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemType == ItType.Usable){
				string qty = craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].quantity.ToString();
				string itemQty = ShowItemQuantity(a).ToString();
				ingredientUi[a].itemNameText.text = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemId].itemName + " x " + qty + " (" + itemQty + ")";
				if(int.Parse(itemQty) >= int.Parse(qty)){
					ingredientUi[a].itemNameText.color = haveAllItemColor;
				}else{
					ingredientUi[a].itemNameText.color = notEnoughtItemColor;
				}
			}else{
				string itemQty = CheckEquipment(a);
				ingredientUi[a].itemNameText.text = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[selection]].ingredient[a].itemId].itemName + " x " + 1 + " (" + itemQty + ")";
				if(int.Parse(itemQty) >= 1){
					ingredientUi[a].itemNameText.color = haveAllItemColor;
				}else{
					ingredientUi[a].itemNameText.color = notEnoughtItemColor;
				}
			}
		}
	}
	
	bool CheckIngredients(){
		player = GlobalStatus.mainPlayer;

		int a = 0;
		while(a < craftingData[selection].ingredient.Length){
			bool item = player.GetComponent<Inventory>().CheckItem(craftingData[selection].ingredient[a].itemId , (int)craftingData[selection].ingredient[a].itemType, craftingData[selection].ingredient[a].quantity);
			if(!item){
				//showError = "Not enought items";
				return false;
			}
			a++;
		}
		return true;
	}
	
	void AddandRemoveItem(){
		player = GlobalStatus.mainPlayer;

		bool full = false;
		if((int)craftingData[selection].gotItem.itemType == 0){
			full = player.GetComponent<Inventory>().AddItem(craftingData[selection].gotItem.itemId , craftingData[selection].gotItem.quantity);
		}else if((int)craftingData[selection].gotItem.itemType == 1){
			full = player.GetComponent<Inventory>().AddEquipment(craftingData[selection].gotItem.itemId);
		}
		//Remove Ingrediant Items
		if(!full){
			int a = 0;
			while(a < craftingData[selection].ingredient.Length){
				if((int)craftingData[selection].ingredient[a].itemType == 0){
					player.GetComponent<Inventory>().RemoveItem(craftingData[selection].ingredient[a].itemId , craftingData[selection].ingredient[a].quantity);
					//------------------
				}else if((int)craftingData[selection].ingredient[a].itemType == 1){
					player.GetComponent<Inventory>().RemoveEquipment(craftingData[selection].ingredient[a].itemId);
					//------------------
				}
				a++;
			}
			//showError = "You Got " + craftingData[selection].itemName;
			gotItemText.gameObject.SetActive(true);
			gotItemText.text = "You Got " + craftingData[selection].itemName;
		}else{
			//showError = "Inventory Full";
			gotItemText.gameObject.SetActive(true);
			gotItemText.text = "Inventory Full";
		}
	}
	
	public void ShowTooltip(int slot){
		player = GlobalStatus.mainPlayer;

		if(!tooltip || !player || slot + cPage >= craftingListId.Length) {
			return;
		}
		slot += cPage;
		if(craftingDatabase.craftingData[craftingListId [slot]].gotItem.itemType == ItType.Usable){
			if(slot >= craftingListId.Length) {
				HideTooltip();
				return;
			}
			tooltipIcon.sprite = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[slot]].gotItem.itemId].icon;
			tooltipName.text = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[slot]].gotItem.itemId].itemName;
			
			tooltipText1.text = itemDatabase.usableItem[craftingDatabase.craftingData[craftingListId[slot]].gotItem.itemId].description;

			tooltip.SetActive (true);
		}else{
			if(slot >= craftingListId.Length) {
				HideTooltip();
				return;
			}
			tooltipIcon.sprite = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[slot]].gotItem.itemId].icon;
			tooltipName.text = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[slot]].gotItem.itemId].itemName;
			
			tooltipText1.text = itemDatabase.equipment[craftingDatabase.craftingData[craftingListId[slot]].gotItem.itemId].description;

			tooltip.SetActive (true);
		}
	}
	public void HideTooltip(){
		if(!tooltip){
			return;
		}
		tooltip.SetActive(false);
	}
}


[System.Serializable]
public class CraftingButton{
	public Image itemIcons;
	public Text itemNameText;
}