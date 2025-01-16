using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour {
	public GameObject canVasUI;
	public static int saveSlot = 0;

	/*void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)){
			//Open Save Load Menu
			OnOffMenu();
		}
	}*/
	
	public void OnOffMenu(){
		if(!canVasUI){
			return;
		}
		if(GetComponent<UiMaster>()){
			GetComponent<UiMaster>().CloseAllMenu();
		}		
		//Freeze Time Scale to 0 if Window is Showing
		if(!canVasUI.activeSelf && Time.timeScale != 0.0f){
			canVasUI.SetActive(true);
			Time.timeScale = 0.0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}else if(canVasUI.activeSelf){
			canVasUI.SetActive(false);
			Time.timeScale = 1.0f;
			//Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
		}
	}

	public void SaveDataOnly(){
		saveSlot = GlobalStatus.saveSlot;
		PlayerPrefs.SetInt("PreviousSave" +saveSlot.ToString(), 10);
		PlayerPrefs.SetString("Name" +saveSlot.ToString(), GlobalStatus.characterName);
		PlayerPrefs.SetInt("PlayerID" +saveSlot.ToString(), GlobalStatus.characterId);
		PlayerPrefs.SetInt("PlayerLevel" +saveSlot.ToString(), GlobalStatus.level);
		PlayerPrefs.SetInt("PlayerATK" +saveSlot.ToString(), GlobalStatus.atk);
		PlayerPrefs.SetInt("PlayerDEF" +saveSlot.ToString(), GlobalStatus.def);
		PlayerPrefs.SetInt("PlayerMATK" +saveSlot.ToString(), GlobalStatus.matk);
		PlayerPrefs.SetInt("PlayerMDEF" +saveSlot.ToString(), GlobalStatus.mdef);
		PlayerPrefs.SetInt("PlayerEXP" +saveSlot.ToString(), GlobalStatus.exp);
		PlayerPrefs.SetInt("PlayerMaxEXP" +saveSlot.ToString(), GlobalStatus.maxExp);
		PlayerPrefs.SetInt("PlayerMaxHP" +saveSlot.ToString(), GlobalStatus.maxHealth);
		PlayerPrefs.SetInt("PlayerMaxMP" +saveSlot.ToString(), GlobalStatus.maxMana);
		PlayerPrefs.SetInt("PlayerSTP" +saveSlot.ToString(), GlobalStatus.statusPoint);
		PlayerPrefs.SetInt("PlayerSKP" +saveSlot.ToString(), GlobalStatus.skillPoint);

		PlayerPrefs.SetInt("PlayerHP" +saveSlot.ToString(), GlobalStatus.health);
		PlayerPrefs.SetInt("PlayerMP" +saveSlot.ToString(), GlobalStatus.mana);
		
		PlayerPrefs.SetInt("Cash" +saveSlot.ToString(), GlobalStatus.cash);
		int itemSize = GlobalStatus.itemSlot.Length;
		int a = 0;
		if(itemSize > 0){
			while(a < itemSize){
				PlayerPrefs.SetInt("Item" + a.ToString() +saveSlot.ToString(), GlobalStatus.itemSlot[a]);
				PlayerPrefs.SetInt("ItemQty" + a.ToString() +saveSlot.ToString(), GlobalStatus.itemQuantity[a]);
				a++;
			}
		}
		
		int equipSize = GlobalStatus.equipment.Length;
		a = 0;
		if(equipSize > 0){
			while(a < equipSize){
				PlayerPrefs.SetInt("Equipm" + a.ToString() +saveSlot.ToString(), GlobalStatus.equipment[a]);
				a++;
			}
		}
		PlayerPrefs.SetInt("WeaEquip" +saveSlot.ToString(), GlobalStatus.weaponEquip);
		PlayerPrefs.SetInt("SubEquip" +saveSlot.ToString(), GlobalStatus.subWeaponEquip);
		
		PlayerPrefs.SetInt("ArmoEquip" +saveSlot.ToString(), GlobalStatus.armorEquip);
		PlayerPrefs.SetInt("AccEquip" +saveSlot.ToString(), GlobalStatus.accessoryEquip);
		PlayerPrefs.SetInt("BootsEquip" +saveSlot.ToString(), GlobalStatus.bootsEquip);
		PlayerPrefs.SetInt("GloveEquip" +saveSlot.ToString(), GlobalStatus.glovesEquip);
		PlayerPrefs.SetInt("HatEquip" +saveSlot.ToString(), GlobalStatus.hatEquip);
		
		//Save Quest
		int questSize = GlobalStatus.questProgress.Length;
		PlayerPrefs.SetInt("QuestSize" +saveSlot.ToString(), questSize);
		a = 0;
		if(questSize > 0){
			while(a < questSize){
				PlayerPrefs.SetInt("Questp" + a.ToString() +saveSlot.ToString(), GlobalStatus.questProgress[a]);
				a++;
			}
		}
		int questSlotSize = GlobalStatus.questSlot.Length;
		PlayerPrefs.SetInt("QuestSlotSize" +saveSlot.ToString(), questSlotSize);
		a = 0;
		if(questSlotSize > 0){
			while(a < questSlotSize){
				PlayerPrefs.SetInt("Questslot" + a.ToString() +saveSlot.ToString(), GlobalStatus.questSlot[a]);
				a++;
			}
		}
		//Skill List Slot
		a = 0;
		while(a < GlobalStatus.skillListSlot.Length){
			PlayerPrefs.SetInt("SkillList" + a.ToString() +saveSlot.ToString(), GlobalStatus.skillListSlot[a]);
			a++;
		}
		//Shortcuts
		for(int b = 0; b < GlobalStatus.shottcutId.Length; b++){
			PlayerPrefs.SetInt("ShortcutId" + b.ToString() +saveSlot.ToString(), GlobalStatus.shottcutId[b]);
			PlayerPrefs.SetInt("ShortcutType" + b.ToString() +saveSlot.ToString(), GlobalStatus.shottcutType[b]);
		}
		
		for(int b = 0; b < EventSetting.globalInt.Length; b++){
			PlayerPrefs.SetInt("GlobalInt" + b.ToString() +saveSlot.ToString(), EventSetting.globalInt[b]);
		}
		for(int b = 0; b < EventSetting.globalBoolean.Length; b++){
			int val = 0;
			if(EventSetting.globalBoolean[b] == true){
				val = 1;
			}
			PlayerPrefs.SetInt("GlobalBool" + b.ToString() +saveSlot.ToString(), val);
		}
		
		PlayerPrefs.SetString("Scene" +saveSlot.ToString(), GlobalStatus.savePointMap);
		
		PlayerPrefs.SetFloat("PlayerX" +saveSlot.ToString(), GlobalStatus.savePosition.x);
		PlayerPrefs.SetFloat("PlayerY" +saveSlot.ToString(), GlobalStatus.savePosition.y);
		PlayerPrefs.SetFloat("PlayerZ" +saveSlot.ToString(), GlobalStatus.savePosition.z);
		print("Saved");
	}
	
	public void LoadDataOnly(){
		saveSlot = GlobalStatus.saveSlot;
		if(PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString()) <= 0){
			return;
		}
		SpawnPlayer.onLoadGame = true;

		GlobalStatus.savePosition.x = PlayerPrefs.GetFloat("PlayerX" +saveSlot.ToString());
		GlobalStatus.savePosition.y = PlayerPrefs.GetFloat("PlayerY" +saveSlot.ToString());
		GlobalStatus.savePosition.z = PlayerPrefs.GetFloat("PlayerZ" +saveSlot.ToString());
		GlobalStatus.savePointMap = PlayerPrefs.GetString("Scene" +saveSlot.ToString());
		
		GlobalStatus.characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
				
		GlobalStatus.level = PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString());
		GlobalStatus.atk = PlayerPrefs.GetInt("PlayerATK" +saveSlot.ToString());
		GlobalStatus.def = PlayerPrefs.GetInt("PlayerDEF" +saveSlot.ToString());
		GlobalStatus.matk = PlayerPrefs.GetInt("PlayerMATK" +saveSlot.ToString());
		GlobalStatus.mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		GlobalStatus.mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		GlobalStatus.exp = PlayerPrefs.GetInt("PlayerEXP" +saveSlot.ToString());
		GlobalStatus.maxExp = PlayerPrefs.GetInt("PlayerMaxEXP" +saveSlot.ToString());
		GlobalStatus.maxHealth = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		GlobalStatus.maxMana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		GlobalStatus.statusPoint = PlayerPrefs.GetInt("PlayerSTP" +saveSlot.ToString());
		GlobalStatus.skillPoint = PlayerPrefs.GetInt("PlayerSKP" +saveSlot.ToString());

		GlobalStatus.health = PlayerPrefs.GetInt("PlayerHP" +saveSlot.ToString());
		GlobalStatus.mana = PlayerPrefs.GetInt("PlayerMP" +saveSlot.ToString());

		//-------------------------------
		GlobalStatus.cash = PlayerPrefs.GetInt("Cash" +saveSlot.ToString());
		int itemSize = GlobalStatus.itemSlot.Length;
		int a = 0;
		if(itemSize > 0){
			while(a < itemSize){
				GlobalStatus.itemSlot[a] = PlayerPrefs.GetInt("Item" + a.ToString() +saveSlot.ToString());
				GlobalStatus.itemQuantity[a] = PlayerPrefs.GetInt("ItemQty" + a.ToString() +saveSlot.ToString());
				//-------
				a++;
			}
		}
		int equipSize = GlobalStatus.equipment.Length;
		a = 0;
		if(equipSize > 0){
			while(a < equipSize){
				GlobalStatus.equipment[a] = PlayerPrefs.GetInt("Equipm" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		GlobalStatus.weaponEquip = PlayerPrefs.GetInt("WeaEquip" +saveSlot.ToString());
		GlobalStatus.subWeaponEquip = PlayerPrefs.GetInt("SubEquip" +saveSlot.ToString());
		GlobalStatus.armorEquip = PlayerPrefs.GetInt("ArmoEquip" +saveSlot.ToString());
		GlobalStatus.accessoryEquip = PlayerPrefs.GetInt("AccEquip" +saveSlot.ToString());
		GlobalStatus.bootsEquip = PlayerPrefs.GetInt("BootsEquip" +saveSlot.ToString());
		GlobalStatus.glovesEquip = PlayerPrefs.GetInt("GloveEquip" +saveSlot.ToString());
		GlobalStatus.hatEquip = PlayerPrefs.GetInt("HatEquip" +saveSlot.ToString());
		//----------------------------------

		//Load Quest
		GlobalStatus.questProgress = new int[PlayerPrefs.GetInt("QuestSize" +saveSlot.ToString())];
		int questSize = GlobalStatus.questProgress.Length;
		a = 0;
		if(questSize > 0){
			while(a < questSize){
				GlobalStatus.questProgress[a] = PlayerPrefs.GetInt("Questp" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		
		GlobalStatus.questSlot = new int[PlayerPrefs.GetInt("QuestSlotSize" +saveSlot.ToString())];
		int questSlotSize = GlobalStatus.questSlot.Length;
		a = 0;
		if(questSlotSize > 0){
			while(a < questSlotSize){
				GlobalStatus.questSlot[a] = PlayerPrefs.GetInt("Questslot" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		
		//Skill List Slot
		a = 0;
		while(a < GlobalStatus.skillListSlot.Length){
			GlobalStatus.skillListSlot[a] = PlayerPrefs.GetInt("SkillList" + a.ToString() +saveSlot.ToString());
			a++;
		}

		//Shortcuts
		for(int b = 0; b < GlobalStatus.shottcutId.Length; b++){
			GlobalStatus.shottcutId[b] = PlayerPrefs.GetInt("ShortcutId" + b.ToString() +saveSlot.ToString());
			GlobalStatus.shottcutType[b] = PlayerPrefs.GetInt("ShortcutType" + b.ToString() +saveSlot.ToString());
		}

		//Global Event
		for(int b = 0; b < EventSetting.globalInt.Length; b++){
			EventSetting.globalInt[b] = PlayerPrefs.GetInt("GlobalInt" + b.ToString() +saveSlot.ToString());
		}
		for(int b = 0; b < EventSetting.globalBoolean.Length; b++){
			int val = PlayerPrefs.GetInt("GlobalBool" + b.ToString() +saveSlot.ToString());
			if(val >= 1){
				EventSetting.globalBoolean[b] = true;
			}
		}
	}

	public void SaveGame(){
		saveSlot = GlobalStatus.saveSlot;
		GlobalStatus.SavePlayerStatus(this.gameObject);
		GlobalStatus.SavePlayerPosition(this.gameObject);
		SaveDataOnly();
		OnOffMenu();
	}

	public void LoadGame(){
		saveSlot = GlobalStatus.saveSlot;
		if(PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString()) <= 0){
			return;
		}
		LoadDataOnly();
		OnOffMenu();
		GlobalStatus.LoadPlayerStatus(this.gameObject);

		if(GlobalStatus.savePointMap != SceneManager.GetActiveScene().name){
			SceneManager.LoadScene(GlobalStatus.savePointMap , LoadSceneMode.Single);
		}
		transform.position = GlobalStatus.savePosition;
	}
}
