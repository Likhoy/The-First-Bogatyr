using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUi : MonoBehaviour {
    public GameObject player;

    public Text moneyText;

    public List<Image> itemIcons;
    public List<Text> itemQty;
    public Image[] equipmentIcons = new Image[25];

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

    public GameObject database;
    private ItemData db;

    private int itemLength;
    private int eqLength = 8;
    
    public Image draggingItemIcon;
    private int draggingItemId;
    private int draggingItemType; // 0 = Item , 1 = Equipment
    public GameObject discardItemConfirmation;
    
    private int sourceSlot;
    private int targetSlot;
    private AttackTrigger atk;

    void Start() {
        db = database.GetComponent<ItemData>();

        if (!player && GlobalStatus.mainPlayer) {
            player = GlobalStatus.mainPlayer;
        }

        if (!player) return;
        atk = player.GetComponent<AttackTrigger>();
        itemLength = player.GetComponent<Inventory>().itemSlot.Length;
        eqLength = player.GetComponent<Inventory>().equipment.Length;
        
    }

    private void OnEnable() {
        var usableItemTab = transform.Find("ItemsBG/UsableItemTab");

        if (usableItemTab == null) return;
        itemIcons.Clear();
        itemQty.Clear();

        foreach (Transform itemSlot in usableItemTab) {
            var iconTransform = itemSlot.Find("Icon");
            var qtyTransform = itemSlot.Find("QtyText");

            if (iconTransform == null || qtyTransform == null) continue;
            var icon = iconTransform.GetComponent<Image>();
            var quantity = qtyTransform.GetComponent<Text>();

            if (icon == null || quantity == null) continue;
            itemIcons.Add(icon);
            itemQty.Add(quantity);
        }
    }

    void Update() {
        if (tooltip && tooltip.activeSelf) {
            Vector2 tooltipPos = Input.mousePosition;
            tooltipPos.x += 7;
            tooltip.transform.position = tooltipPos;
        }

        if (!player) return;
        
        for (var i = 0; i < itemIcons.Count; i++) {
            itemIcons[i].GetComponent<Image>().sprite = i < itemLength ? 
                db.usableItem[player.GetComponent<Inventory>().itemSlot[i]].icon : 
                db.usableItem[0].icon;
        }
        
        for (var i = 0; i < itemQty.Count; i++) {
            if (i < itemLength) {
                var qty = player.GetComponent<Inventory>().itemQuantity[i].ToString();
                if (qty == "0") {
                    qty = "";
                }
                itemQty[i].GetComponent<Text>().text = qty;
            }
            else {
                itemQty[i].GetComponent<Text>().text = "";
            }
        }
        
        for (var i = 0; i < equipmentIcons.Length; i++) {
            equipmentIcons[i].GetComponent<Image>().sprite = i < eqLength ? 
                db.equipment[player.GetComponent<Inventory>().equipment[i]].icon : 
                db.equipment[0].icon;
        }

        if (weaponIcons) {
            weaponIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().weaponEquip].icon;
        }
        if (subWeaponIcons) {
            subWeaponIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().subWeaponEquip].icon;
        }
        if (armorIcons) {
            armorIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().armorEquip].icon;
        }
        if (accIcons) {
            accIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().accessoryEquip].icon;
        }
        if (helmIcons) {
            helmIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().hatEquip].icon;
        }
        if (glovesIcons) {
            glovesIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().glovesEquip].icon;
        }
        if (bootsIcons) {
            bootsIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().bootsEquip].icon;
        }
        if (moneyText) {
            moneyText.GetComponent<Text>().text = player.GetComponent<Inventory>().cash.ToString();
        }

    }

    public void OnBeginDrag(GameObject itemSlot) {
        int slot = itemSlot.transform.GetSiblingIndex();
        sourceSlot = slot;
    }

    public void OnEndDrag(GameObject itemSlot) {
        if (sourceSlot != targetSlot) {
            SwapItems(sourceSlot, targetSlot);
        }
    }
    
    /*ShowTooltip*/
    public void OnPointerEnter(GameObject itemSlot) {
        int slot = itemSlot.transform.GetSiblingIndex();
        targetSlot = slot;
        if (!tooltip || !player) return; 
        if (slot >= itemLength) return;
        if (player.GetComponent<Inventory>().itemSlot[slot] <= 0) {
            OnPointerExit();
            return;
        }

        tooltipIcon.GetComponent<Image>().sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].icon;
        tooltipName.GetComponent<Text>().text = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].itemName;
        tooltipText1.GetComponent<Text>().text = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].description;

        tooltip.SetActive(true);
    }

    public void ShowEquipmentTooltip(int slot) {
        if (!tooltip || !player || slot >= eqLength) {
            return;
        }
        if (player.GetComponent<Inventory>().equipment[slot] <= 0) {
            OnPointerExit();
            return;
        }
        
        tooltipIcon.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().equipment[slot]].icon;
        tooltipName.GetComponent<Text>().text = db.equipment[player.GetComponent<Inventory>().equipment[slot]].itemName;

        tooltipText1.GetComponent<Text>().text = db.equipment[player.GetComponent<Inventory>().equipment[slot]].description;

        tooltip.SetActive(true);
    }

    public void ShowOnEquipTooltip(int type) {
        if (!tooltip || !player) {
            return;
        }
        // 0 = Weapon, 1 = Armor, 2 = Accessories , 3 = Sub Weapon
        // 4 = Headgear , 5 = Gloves , 6 = Boots
        var id = type switch {
            0 => player.GetComponent<Inventory>().weaponEquip,
            1 => player.GetComponent<Inventory>().armorEquip,
            2 => player.GetComponent<Inventory>().accessoryEquip,
            3 => player.GetComponent<Inventory>().subWeaponEquip,
            4 => player.GetComponent<Inventory>().hatEquip,
            5 => player.GetComponent<Inventory>().glovesEquip,
            6 => player.GetComponent<Inventory>().bootsEquip,
            _ => 0
        };

        if (id <= 0) {
            OnPointerExit();
            return;
        }
        
        tooltipIcon.GetComponent<Image>().sprite = db.equipment[id].icon;
        tooltipName.GetComponent<Text>().text = db.equipment[id].itemName;

        tooltipText1.GetComponent<Text>().text = db.equipment[id].description;
        
        tooltip.SetActive(true);
    }

    /*HideTooltip*/
    public void OnPointerExit() {
        if (!tooltip) return;
        tooltip.SetActive(false);
    }

    public void OnClickItemSlot(GameObject itemSlot) {
        int slot = itemSlot.transform.GetSiblingIndex();
        if (!player || slot >= itemLength) {
            return;
        }
        player.GetComponent<Inventory>().UseItem(slot);
        OnPointerEnter(itemSlot);
    }

    public void EquipItem(int itemSlot) {
        if (!player || itemSlot >= eqLength) {
            return;
        }
        player.GetComponent<Inventory>().EquipItem(player.GetComponent<Inventory>().equipment[itemSlot], itemSlot);
        ShowEquipmentTooltip(itemSlot);
    }
    
    public void UnEquip(int type) {
        // 0 = Weapon, 1 = Armor, 2 = Accessories, 3 = Headgear , 4 = Gloves , 5 = Boots
        if (!player) {
            return;
        }

        var id = type switch {
            0 => player.GetComponent<Inventory>().weaponEquip,
            1 => player.GetComponent<Inventory>().armorEquip,
            2 => player.GetComponent<Inventory>().accessoryEquip,
            4 => player.GetComponent<Inventory>().hatEquip,
            5 => player.GetComponent<Inventory>().glovesEquip,
            6 => player.GetComponent<Inventory>().bootsEquip,
            _ => 0
        };
        if (id > 0) {
            player.GetComponent<Inventory>().UnEquip(id);
        }
        ShowOnEquipTooltip(type);
    }
    
    public void SwapWeapon() {
		if (!player) {
			return;
		}
		player.GetComponent<Inventory>().SwapWeapon();
		ShowOnEquipTooltip(3);
	}

    public void CloseMenu() {
        Time.timeScale = 1.0f;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        GlobalStatus.menuOn = false;
        gameObject.SetActive(false);
        if (draggingItemIcon) {
			draggingItemIcon.gameObject.SetActive(false);
		}
    }
    
    public void OpenUsableTab() {
        usableTab.SetActive(true);
        equipmentTab.SetActive(false);
    }

    public void OpenEquipmentTab() {
        usableTab.SetActive(false);
        equipmentTab.SetActive(true);
    }
    
	public void OnDragItem(GameObject itemSlot) {
        int slot = itemSlot.transform.GetSiblingIndex();
		if (!player || slot >= itemLength) return;
		if (player.GetComponent<Inventory>().itemSlot[slot] == 0) return;
		atk.draggingItemIcon.gameObject.SetActive(true);
		atk.draggingItemIcon.sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].icon;
        
		draggingItemType = 0;

		player.GetComponent<AttackTrigger>().PickupForShortcut(player.GetComponent<Inventory>().itemSlot[slot] , 1);
	}
    
    public void SwapItems(int slot1, int slot2) {
        if (!player) return;
        var inventory = player.GetComponent<Inventory>();
        if (slot1 >= itemLength || slot2 >= itemLength) return;
        (inventory.itemSlot[slot1], inventory.itemSlot[slot2]) = (inventory.itemSlot[slot2], inventory.itemSlot[slot1]);
        (inventory.itemQuantity[slot1], inventory.itemQuantity[slot2]) = (inventory.itemQuantity[slot2], inventory.itemQuantity[slot1]);
    }

    private void DiscardItem() {
        if (!player) return;
        if (draggingItemType == 0) {
            player.GetComponent<Inventory>().RemoveItem(draggingItemId , 9999999);
        } else {
            player.GetComponent<Inventory>().RemoveEquipment(draggingItemId);
        }
        if (discardItemConfirmation) {
            discardItemConfirmation.SetActive(false);
        }
        player.GetComponent<AttackTrigger>().DiscardShortcut();
    }

    public void OnDragEquipment(int itemSlot) {
        if (!player || itemSlot >= eqLength) {
            return;
        }
        if (player.GetComponent<Inventory>().equipment[itemSlot] == 0) {
            return;
        }
        AttackTrigger atk = player.GetComponent<AttackTrigger>();
        atk.draggingItemIcon.gameObject.SetActive(true);
        atk.draggingItemIcon.sprite = db.equipment[player.GetComponent<Inventory>().equipment[itemSlot]].icon;
        draggingItemId = player.GetComponent<Inventory>().equipment[itemSlot];
        draggingItemType = 1;

        player.GetComponent<AttackTrigger>().PickupForShortcut(player.GetComponent<Inventory>().equipment[itemSlot] , 2);
    }
}
