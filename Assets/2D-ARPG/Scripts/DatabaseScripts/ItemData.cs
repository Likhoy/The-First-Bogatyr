using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemData : MonoBehaviour {
	[System.Serializable]
	public class Usable {
		public string itemName = "";
		public Sprite icon;
		public AddItem dropPrefab;
		public Transform useEffect;
		[TextArea]
		public string description = "";
		public int price = 10;
		public int hpRecover = 0;
		public int mpRecover = 0;
		public bool unusable = false;
		public bool unlimited = false;
		public string sendMsg = "";
		public AtkItemSet attackItem;
	} 

	[System.Serializable]
	public class Equip {
		public string itemName = "";
		public Sprite icon;
		public AddItem dropPrefab;
		public bool canBlock = false;
		[TextArea]
		public string description = "";
		public int price = 10;

		public int attack = 0;
		public int defense = 0;
		public int magicAttack = 0;
		public int magicDefense = 0;
		public int hpBonus = 0;
		public int mpBonus = 0;

		public EqType EquipmentType = EqType.Weapon; 
		
		[Tooltip("Ignore it, if the equipment type is not weapon.")]
		public BulletStatus[] attackPrefab = new BulletStatus[1];
		public string[] attackAnimationTrigger = new string[3];
		public string blockingAnimationTrigger ;

		public WhileAtk whileAttack = WhileAtk.Immobile;
		public float attackCast = 0.18f;
		public float attackDelay = 0.12f;
		public AudioClip soundEffect;
		public int weaponType = 0;
		public ChargeAtk[] chargeAttack;

		public ElementalResist elementlResistance;
		public Resist statusResist;
		public bool canDoubleJump = false;
		[Range(0 , 100)]
		public int autoGuard = 0;
		[Range(0 , 100)]
		public int drainTouch = 0;
		[Range(0 , 100)]
		public int mpReduce = 0;
		[Tooltip("Set to 0 if not require any item when attacking.")]
		public int requireItemId = 0;
	} 
	
	public Usable[] usableItem = new Usable[3];
	public Equip[] equipment = new Equip[3];
}

public enum EqType {
	Weapon = 0,
	Armor = 1,
	Accessory = 2,
	Headgear = 3,
	Gloves = 4,
	Boots = 5
}

public enum WhileAtk{
	MeleeFwd = 0,
	Immobile = 1,
	WalkFree = 2
}

[System.Serializable]
public class AtkItemSet{
	public bool enable = false;
	public BulletStatus attackPrefab;
	public string atkAnimationTrigger;
	public float attackCast = 0.18f;
	public float attackDelay = 0.12f;
}
