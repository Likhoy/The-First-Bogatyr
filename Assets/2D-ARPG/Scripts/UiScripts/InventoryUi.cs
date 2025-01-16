using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour {
	public GameObject player;
	
	public Text moneyText;
	
	public Image[] itemIcons = new Image[16];
	public Text[] itemQty = new Text[16];
	public Image[] equipmentIcons = new Image[16];
	
	public Image weaponIcons;
	public Image subWeaponIcons;
	public Image armorIcons;
	public Image accIcons;
	public Image helmIcons;
	public Image glovesIcons;
	public Image bootsIcons;
	
	public GameObject tooltip;
	public Image tooltipIcon;
	public Text tooltipName;
	public Text tooltipText1;

	public GameObject usableTab;
	public GameObject equipmentTab;
	public Text pageText;
	
	public GameObject database;
	private ItemData db;
	
	private int maxPage = 1;
	private int maxPageEq = 1;
	private int page = 0;
	private int cPage = 0;
	private int ePage = 0;
	private int itemLength = 16;
	private int eqLength = 8;
	
	void Start(){
		db = database.GetComponent<ItemData>();
		
		//Set Max Page
		if(!player && GlobalStatus.mainPlayer){
			player = GlobalStatus.mainPlayer;
		}
		if(player){
			itemLength = player.GetComponent<Inventory>().itemSlot.Length;
			eqLength = player.GetComponent<Inventory>().equipment.Length;
		}
		maxPage = itemLength / itemIcons.Length;
		if(itemLength % itemIcons.Length != 0){
			maxPage += 1;
		}
		//maxPage -= 1;
		
		maxPageEq = eqLength / equipmentIcons.Length;
		if(eqLength % equipmentIcons.Length != 0){
			maxPageEq += 1;
		}
		//maxPageEq -= 1;
		print(maxPage + " " + maxPageEq);
	}
	
	void Update(){
		if(tooltip && tooltip.activeSelf == true){
			Vector2 tooltipPos = Input.mousePosition;
			tooltipPos.x += 7;
			tooltip.transform.position = tooltipPos;
		}
		/*if(draggingItemIcon && draggingItemIcon.gameObject.activeSelf == true){
			Vector2 dragIconPos = Input.mousePosition;
			dragIconPos.y -= 0.55f;
			draggingItemIcon.transform.position = dragIconPos;
			if(Input.GetKeyUp(KeyCode.Mouse0)){
				OnDropItem();
			}
		}*/
		if(!player){
			return;
		}

		for(int a = 0; a < itemIcons.Length; a++){
			if(a + cPage < itemLength){
				itemIcons[a].GetComponent<Image>().sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[a + cPage]].icon;
			}else{
				itemIcons[a].GetComponent<Image>().sprite = db.usableItem[0].icon;
			}
		}
		
		for(int q = 0; q < itemQty.Length; q++){
			if(q + cPage < itemLength){
				string qty = player.GetComponent<Inventory>().itemQuantity[q + cPage].ToString();
				if(qty == "0"){
					qty = "";
				}
				itemQty[q].GetComponent<Text>().text = qty;
			}else{
				itemQty[q].GetComponent<Text>().text = "";
			}
		}
		
		for(int b = 0; b < equipmentIcons.Length; b++){
			if(b + ePage < eqLength){
				equipmentIcons[b].GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().equipment[b + ePage]].icon;
			}else{
				equipmentIcons[b].GetComponent<Image>().sprite = db.equipment[0].icon;
			}
		}
		
		if(weaponIcons){
			weaponIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().weaponEquip].icon;
		}
		if(subWeaponIcons){
			subWeaponIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().subWeaponEquip].icon;
		}
		if(armorIcons){
			armorIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().armorEquip].icon;
		}
		if(accIcons){
			accIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().accessoryEquip].icon;
		}
		if(helmIcons){
			helmIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().hatEquip].icon;
		}
		if(glovesIcons){
			glovesIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().glovesEquip].icon;
		}
		if(bootsIcons){
			bootsIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().bootsEquip].icon;
		}
		if(moneyText){
			moneyText.GetComponent<Text>().text = player.GetComponent<Inventory>().cash.ToString();
		}
	}
	
	public void ShowItemTooltip(int slot){
		if(!tooltip || !player || slot + cPage >= itemLength){
			return;
		}
		slot += cPage;
		if(player.GetComponent<Inventory>().itemSlot[slot] <= 0){
			HideTooltip();
			return;
		}
		
		tooltipIcon.GetComponent<Image>().sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].icon;
		tooltipName.GetComponent<Text>().text = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].itemName;
		
		tooltipText1.GetComponent<Text>().text = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].description;

		tooltip.SetActive(true);
	}
	
	public void ShowEquipmentTooltip(int slot){
		if(!tooltip || !player || slot + ePage >= eqLength){
			return;
		}
		slot += ePage;
		if(player.GetComponent<Inventory>().equipment[slot] <= 0){
			HideTooltip();
			return;
		}
		
		tooltipIcon.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().equipment[slot]].icon;
		tooltipName.GetComponent<Text>().text = db.equipment[player.GetComponent<Inventory>().equipment[slot]].itemName;
		
		tooltipText1.GetComponent<Text>().text = db.equipment[player.GetComponent<Inventory>().equipment[slot]].description;

		tooltip.SetActive(true);
	}
	
	public void ShowOnEquipTooltip(int type){
		if(!tooltip || !player){
			return;
		}
		//0 = Weapon, 1 = Armor, 2 = Accessories , 3 = Sub Weapon
		//4 = Headgear , 5 = Gloves , 6 = Boots
		int id = 0;
		if(type == 0){
			id = player.GetComponent<Inventory>().weaponEquip;
		}
		if(type == 1){
			id = player.GetComponent<Inventory>().armorEquip;
		}
		if(type == 2){
			id = player.GetComponent<Inventory>().accessoryEquip;
		}
		if(type == 3){
			id = player.GetComponent<Inventory>().subWeaponEquip;
		}
		if(type == 4){
			id = player.GetComponent<Inventory>().hatEquip;
		}
		if(type == 5){
			id = player.GetComponent<Inventory>().glovesEquip;
		}
		if(type == 6){
			id = player.GetComponent<Inventory>().bootsEquip;
		}
		
		if(id <= 0){
			HideTooltip();
			return;
		}
		
		tooltipIcon.GetComponent<Image>().sprite = db.equipment[id].icon;
		tooltipName.GetComponent<Text>().text = db.equipment[id].itemName;
		
		tooltipText1.GetComponent<Text>().text = db.equipment[id].description;

		tooltip.SetActive(true);
	}
	
	public void HideTooltip(){
		if(!tooltip){
			return;
		}
		tooltip.SetActive(false);
	}
	
	public void UseItem(int itemSlot){
		if(!player || itemSlot + cPage >= itemLength){
			return;
		}
		player.GetComponent<Inventory>().UseItem(itemSlot + cPage);
		ShowItemTooltip(itemSlot + cPage);
	}
	
	public void EquipItem(int itemSlot){
		if(!player || itemSlot + ePage >= eqLength){
			return;
		}
		player.GetComponent<Inventory>().EquipItem(player.GetComponent<Inventory>().equipment[itemSlot + ePage] , itemSlot + ePage);
		ShowEquipmentTooltip(itemSlot + ePage);
	}
	
	public void ResetPage(){
		page = 0;
		cPage = 0;
		ePage = 0;
		if(pageText){
			int p = page + 1;
			pageText.GetComponent<Text>().text = p.ToString();
		}
	}
	
	public void NextPage(){
		if(page < maxPage - 1){
			page++;
			cPage = page * itemIcons.Length;
		}
		if(pageText){
			int p = page + 1;
			pageText.GetComponent<Text>().text = p.ToString();
		}
	}
	
	public void NextPageEq(){
		if(page < maxPageEq -1){
			page++;
			ePage = page * equipmentIcons.Length;
		}
		if(pageText){
			int p = page + 1;
			pageText.GetComponent<Text>().text = p.ToString();
		}
	}
	
	public void PreviousPage(){
		if(page > 0){
			page--;
			cPage = page * itemIcons.Length;
			ePage = page * equipmentIcons.Length;
		}
		if(pageText){
			int p = page + 1;
			pageText.GetComponent<Text>().text = p.ToString();
		}
	}
	
	public void UnEquip(int type){
		//0 = Weapon, 1 = Armor, 2 = Accessories
		//3 = Headgear , 4 = Gloves , 5 = Boots
		if(!player){
			return;
		}
		int id = 0;
		if(type == 0){
			id = player.GetComponent<Inventory>().weaponEquip;
		}
		if(type == 1){
			id = player.GetComponent<Inventory>().armorEquip;
		}
		if(type == 2){
			id = player.GetComponent<Inventory>().accessoryEquip;
		}
		if(type == 4){
			id = player.GetComponent<Inventory>().hatEquip;
		}
		if(type == 5){
			id = player.GetComponent<Inventory>().glovesEquip;
		}
		if(type == 6){
			id = player.GetComponent<Inventory>().bootsEquip;
		}
		//print(id);
		if(id > 0){
			player.GetComponent<Inventory>().UnEquip(id);
		}
		ShowOnEquipTooltip(type);
	}
	
	public void SwapWeapon(){
		if(!player){
			return;
		}
		player.GetComponent<Inventory>().SwapWeapon();
		ShowOnEquipTooltip(3);
	}
	
	public void CloseMenu(){
		Time.timeScale = 1.0f;
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		GlobalStatus.menuOn = false;
		gameObject.SetActive(false);
		/*if(draggingItemIcon){
			draggingItemIcon.gameObject.SetActive(false);
		}*/
		//onDiscardArea = false;
	}
	
	public void OpenUsableTab(){
		ResetPage();
		usableTab.SetActive(true);
		equipmentTab.SetActive(false);
	}
	
	public void OpenEquipmentTab(){
		ResetPage();
		usableTab.SetActive(false);
		equipmentTab.SetActive(true);
	}
	
	
	//public Image draggingItemIcon;
	//private int draggingItemId = 0;
	//private int draggingItemType = 0; //0 = Item , 1 = Equipment
	//public GameObject discardItemConfirmation;
	//private bool onDiscardArea = false;
	
	public void OnDragItem(int itemSlot){
		if(!player || itemSlot + cPage >= itemLength){
			return;
		}
		if(player.GetComponent<Inventory>().itemSlot[itemSlot + cPage] == 0){
			return;
		}
		AttackTrigger atk = player.GetComponent<AttackTrigger>();
		atk.draggingItemIcon.gameObject.SetActive(true);
		atk.draggingItemIcon.sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[itemSlot + cPage]].icon;
		//draggingItemId = player.GetComponent<Inventory>().itemSlot[itemSlot + cPage];
		//draggingItemType = 0;

		player.GetComponent<AttackTrigger>().PickupForShortcut(player.GetComponent<Inventory>().itemSlot[itemSlot + cPage] , 1);
	}
	
	/*public void OnDropItem(){
		AttackTrigger atk = player.GetComponent<AttackTrigger>();
		atk.draggingItemIcon.gameObject.SetActive(false);
		if(!player || !onDiscardArea){
			return;
		}
		if(discardItemConfirmation){
			discardItemConfirmation.SetActive(true);
		}else{
			DiscardItem();
		}
	}
	
	public void DiscardItem(){
		if(!player){
			return;
		}
		if(draggingItemType == 0){
			player.GetComponent<Inventory>().RemoveItem(draggingItemId , 9999999);
		}else{
			player.GetComponent<Inventory>().RemoveEquipment(draggingItemId);
		}
		if(discardItemConfirmation){
			discardItemConfirmation.SetActive(false);
		}
		player.GetComponent<AttackTrigger>().DiscardShortcut();
	}*/
	
	/*public void EnterDiscardArea(bool d){
		onDiscardArea = d;
		print ("Dragging Area = " + d);
	}*/

	public void OnDragEquipment(int itemSlot){
		if(!player || itemSlot + ePage >= eqLength){
			return;
		}
		if(player.GetComponent<Inventory>().equipment[itemSlot + ePage] == 0){
			return;
		}
		AttackTrigger atk = player.GetComponent<AttackTrigger>();
		atk.draggingItemIcon.gameObject.SetActive(true);
		atk.draggingItemIcon.sprite = db.equipment[player.GetComponent<Inventory>().equipment[itemSlot + ePage]].icon;
		//draggingItemId = player.GetComponent<Inventory>().equipment[itemSlot + ePage];
		//draggingItemType = 1;

		player.GetComponent<AttackTrigger>().PickupForShortcut(player.GetComponent<Inventory>().equipment[itemSlot + ePage] , 2);
	}
}
