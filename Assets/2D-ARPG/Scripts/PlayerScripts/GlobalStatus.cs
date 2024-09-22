using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalStatus : MonoBehaviour {
	public static GameObject mainPlayer;
	public static int[] eventVar = new int[20]; //Stored all event condition variable
	public static bool freezeAll = false;
	public static bool freezePlayer = false;
	public static bool interacting = false;
	public static bool freezeCam = false;
	public static bool menuOn = false;
	public static int saveSlot = 0;

	public static string characterName = "";
	public static int characterId = 0;
	public static int level = 1;
	public static int atk = 0;
	public static int def = 0;
	public static int matk = 0;
	public static int mdef = 0;
	public static int exp = 0;
	public static int maxExp = 100;
	public static int maxHealth = 100;
	public static int maxMana = 100;
	public static int statusPoint = 0;
	public static int skillPoint = 0;

	public static int health = 100;
	public static int mana = 100;
	
	public static int cash = 0;
	public static int[] itemSlot = new int[20];
	public static int[] itemQuantity = new int[20];
	public static int[] equipment = new int[12];
	public static int weaponEquip = 0;
	public static int subWeaponEquip = 0;
	public static int armorEquip = 0;
	public static int hatEquip = 0;
	public static int glovesEquip = 0;
	public static int bootsEquip = 0;
	public static int accessoryEquip = 0;
	
	public static int[] shottcutId = new int[8];
	public static int[] shottcutType = new int[8];

	public static int[] skillListSlot = new int[30];
	
	public static int[] questProgress = new int[20];
	public static int[] questSlot = new int[5];
	
	public static Vector3 savePosition;
	public static string savePointMap;

	public static void SavePlayerStatus(GameObject player){
		//savePosition = player.transform.position;
		//savePointMap = SceneManager.GetActiveScene().name;

		Status stat = player.GetComponent<Status>();
		characterName = stat.characterName;
		characterId = stat.characterId;
		level = stat.level;
		atk = stat.atk;
		def = stat.def;
		matk = stat.matk;
		mdef = stat.mdef;
		exp = stat.exp;
		maxExp = stat.maxExp;
		maxHealth = stat.maxHealth;
		maxMana = stat.maxMana;

		health = stat.health;
		mana = stat.mana;
		
		statusPoint = stat.statusPoint;
		skillPoint = stat.skillPoint;
		
		Inventory inv = player.GetComponent<Inventory>();
		cash = inv.cash;
		itemSlot = inv.itemSlot;
		itemQuantity = inv.itemQuantity;
		equipment = inv.equipment;
		weaponEquip = inv.weaponEquip;
		subWeaponEquip = inv.subWeaponEquip;
		armorEquip = inv.armorEquip;
		hatEquip = inv.hatEquip;
		glovesEquip = inv.glovesEquip;
		bootsEquip = inv.bootsEquip;
		accessoryEquip = inv.accessoryEquip;
		
		SkillStatus sk = player.GetComponent<SkillStatus>();
		skillListSlot = sk.skillListSlot;
		
		questProgress = player.GetComponent<QuestStat>().questProgress;
		questSlot = player.GetComponent<QuestStat>().questSlot;

		AttackTrigger at = player.GetComponent<AttackTrigger>();
		shottcutId = new int[at.shortcuts.Length];
		shottcutType = new int[at.shortcuts.Length];
		for(int a = 0; a < at.shortcuts.Length; a++){
			shottcutId[a] = at.shortcuts[a].id;
			shottcutType[a] = (int)at.shortcuts[a].type;
		}
	}

	public static void SavePlayerPosition(GameObject player){
		savePosition = player.transform.position;
		savePointMap = SceneManager.GetActiveScene().name;
	}
	
	public static void LoadPlayerStatus(GameObject player){
		Status stat = player.GetComponent<Status>();
		stat.characterName = characterName;
		stat.characterId = characterId;
		stat.level = level;
		stat.atk = atk;
		stat.def = def;
		stat.matk = matk;
		stat.mdef = mdef;
		stat.exp = exp;
		stat.maxExp = maxExp;
		stat.maxHealth = maxHealth;
		stat.maxMana = maxMana;
		stat.statusPoint = statusPoint;
		stat.skillPoint = skillPoint;

		stat.health = health;
		stat.mana = mana;
		
		Inventory inv = player.GetComponent<Inventory>();
		inv.cash = cash;
		inv.itemSlot = itemSlot;
		inv.itemQuantity = itemQuantity;
		inv.equipment = equipment;
		inv.weaponEquip = weaponEquip;
		inv.subWeaponEquip = subWeaponEquip;
		inv.armorEquip = armorEquip;
		inv.hatEquip = hatEquip;
		inv.glovesEquip = glovesEquip;
		inv.bootsEquip = bootsEquip;
		inv.accessoryEquip = accessoryEquip;

		SkillStatus sk = player.GetComponent<SkillStatus>();
		sk.skillListSlot = skillListSlot;
		
		player.GetComponent<QuestStat>().questProgress = questProgress;
		player.GetComponent<QuestStat>().questSlot = questSlot;

		AttackTrigger at = player.GetComponent<AttackTrigger>();
		//shottcutId = new int[at.shortcuts.Length];
		//shottcutType = new int[at.shortcuts.Length];
		for(int a = 0; a < at.shortcuts.Length; a++){
			at.shortcuts[a].id = shottcutId[a];
			if(shottcutType[a] == 0){
				at.shortcuts[a].type = AttackTrigger.ShortcutType.None;
			}
			if(shottcutType[a] == 1){
				at.shortcuts[a].type = AttackTrigger.ShortcutType.UsableItem;
			}
			if(shottcutType[a] == 2){
				at.shortcuts[a].type = AttackTrigger.ShortcutType.Equipment;
			}
			if(shottcutType[a] == 3){
				at.shortcuts[a].type = AttackTrigger.ShortcutType.Skill;
			}
		}
		at.SetupInitialShortcut();
		inv.InitialSetting();
	}
}
