using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [RequireComponent(typeof (Inventory))]
[RequireComponent(typeof (SkillStatus))]
[RequireComponent(typeof (UiMaster))]
[RequireComponent(typeof (Status))]
[RequireComponent(typeof (QuestStat))]
[RequireComponent(typeof (SaveLoad))]
[RequireComponent(typeof (PlayerInputManager))]

public class AttackTrigger : MonoBehaviour{
	public Transform attackPoint;
	public bool aimAtMouse = true;
	//public Vector2 limitAimAngle = new Vector2(-75 , 75);

	public BulletStatus[] attackPrefab = new BulletStatus[1];
	public int weaponType = 0;

	public bool notActive = false;

	public int requireItemId = 0;
	public string requireItemName = "";
	public AudioClip attackSoundEffect;

	public string[] attackAnimationTrigger = new string[3];
	public string blockingAnimationTrigger ;
	
	public WhileAtk whileAttack = WhileAtk.Immobile;
	public bool canBlock = false;
	public float attackCast = 0.18f;
	public float attackDelay = 0.12f;

	[HideInInspector]
	public bool meleefwd = false;
	[HideInInspector]
	public bool onAttacking = false;
	private int c = 0;
	private float nextFire = 0.0f;

	[HideInInspector]
	public GameObject actvateObj;
	[HideInInspector]
	public string actvateMsg = "";
	[HideInInspector]
	public string buttonText = "";
	[HideInInspector]
	public bool showButton = false;

	public ChargeAtk[] charge;
	[HideInInspector]
	public bool charging = false;
	[HideInInspector]
	public GameObject chargingEffect;
	[HideInInspector]
	public int ch;

	[System.Serializable]
	public class CanvasObj{
		public GameObject activatorButton;
		public Text activatorText;
	}
	public CanvasObj canvasElement;
	public CameraFollowPlayer2D mainCameraPrefab;
	public static GameObject mainCam;

	//----------Sounds-------------
	[System.Serializable]
	public class AtkSound {
		public AudioClip[] attackComboVoice = new AudioClip[3];
		public AudioClip magicCastVoice;
	}
	public AtkSound sound;

	[System.Serializable]
	public class ShortcutData{
		public ShortcutType type = ShortcutType.None; //0 Empty , 1 Items , 2 Equipment , 3 Skills
		public int id = 0;
		public SkillSetting skill;
		[HideInInspector]
		public int onCoolDown = 0;
		[HideInInspector]
		public float wait = 0;
		public Sprite icon;
	}
	public enum ShortcutType{
		None = 0,
		UsableItem = 1,
		Equipment = 2,
		Skill = 3
	}
	public ShortcutData[] shortcuts = new ShortcutData[8];
	private Status stat;

	public bool mobileMode = false;
	[HideInInspector]
	public bool facingRight = true;
	private int skSelect = 0;
	private Inventory inv;

	[System.Serializable]
	public class ShortcutGUI{
		public Image iconImage;
		public GameObject coolDownBackground;
		public Text coolDownText;
		public Text quantityText;
	}
	public ShortcutGUI[] shortcutUi = new ShortcutGUI[8];
	public Text textPrinter;

	private ItemData itemDB;
	private SkillData skillDB;
	// public bool ignoreMonsterCollision = true;
	private bool onButtonMenu = false;
	[HideInInspector]
	public Transform dropItemPrefab;

	[HideInInspector]
	public Transform minion;

	private Rigidbody2D rb;

	void Awake(){
		if(!GlobalStatus.mainPlayer){
			GlobalStatus.mainPlayer = this.gameObject;
		}
		DontDestroyOnLoad(transform.gameObject);
		rb = GetComponent<Rigidbody2D>();
		//Create new Attack Point if you didn't have one.
		if(!attackPoint){
			attackPoint = new GameObject().transform;
			attackPoint.position = transform.position;
			attackPoint.rotation = transform.rotation;
			attackPoint.parent = this.transform;
			attackPoint.name = "AttackPoint";
		}
		stat = GetComponent<Status>();
		inv = GetComponent<Inventory>();
		itemDB = GetComponent<Inventory>().database;
		skillDB = GetComponent<SkillStatus>().database;

		gameObject.tag = "Player";
		// gameObject.layer = 8; //Set to Character Layer
		// Physics2D.IgnoreLayerCollision(8, 8, ignoreMonsterCollision); 

		if(transform.eulerAngles.y == 0){
			facingRight = true;
		}
		/*if(mainCameraPrefab){
			GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera"); 
			foreach(GameObject cam2 in cam){ 
				if(cam2){
					Destroy(cam2.gameObject);
				}
			}
			Transform newCam = Instantiate(mainCameraPrefab.transform, transform.position , Quaternion.identity) as Transform;
			newCam.GetComponent<CameraFollowPlayer2D>().player = this.transform;
			mainCam = newCam.gameObject;
		}*/
		SetupInitialShortcut(); 
		if(!GetComponent<AudioSource>()){
			gameObject.AddComponent<AudioSource>();
		}

		//Set Z Axis to 0
		Vector3 pos = transform.position;
		pos.z = 0;
		transform.position = pos;

        if(!GetComponent<PlayerInputManager>()) {
			gameObject.AddComponent<PlayerInputManager>();
        }

		//UpdateShortcut();
		//gameObject.layer = 10;
		//Physics.IgnoreLayerCollision(10 , 11 , true);
	}
	
	void Update(){
		if(draggingItemIcon && draggingItemIcon.gameObject.activeSelf == true){
			Vector2 dragIconPos = Input.mousePosition;
			dragIconPos.y -= 0.55f;
			draggingItemIcon.transform.position = dragIconPos;
			if(Input.GetKeyUp(KeyCode.Mouse0)){
				SetShortcut();
			}
		}

		//Skill Cooldown
		for(int s = 0; s < shortcuts.Length; s++){
			if(shortcuts[s].onCoolDown > 0){
				if(shortcuts[s].wait >= 1){
					shortcuts[s].onCoolDown--;
					shortcuts[s].wait = 0;
				}else{
					shortcuts[s].wait += Time.deltaTime;
				}	
			}
		}
		for(int a = 0; a < shortcutUi.Length; a++){
			if(shortcuts[a].onCoolDown > 0){
				if(shortcutUi[a].coolDownText){
					shortcutUi[a].coolDownText.gameObject.SetActive(true);
					shortcutUi[a].coolDownText.text = shortcuts[a].onCoolDown.ToString();
				}
				if(shortcutUi[a].coolDownBackground){
					shortcutUi[a].coolDownBackground.gameObject.SetActive(true);
				}
			}else{
				if(shortcutUi[a].coolDownText){
					shortcutUi[a].coolDownText.gameObject.SetActive(false);
				}
				if(shortcutUi[a].coolDownBackground){
					shortcutUi[a].coolDownBackground.SetActive(false);
				}
			}
		}
		
		if(showButton){
			if(!canvasElement.activatorButton.activeSelf){
				canvasElement.activatorButton.SetActive(true);
			}
			if(GlobalStatus.freezeAll || Time.timeScale == 0 || !actvateObj || GlobalStatus.interacting){
				if(canvasElement.activatorButton.activeSelf){
					canvasElement.activatorButton.SetActive(false);
				}
			}
		}
		if(!showButton && canvasElement.activatorButton && canvasElement.activatorButton.activeSelf){
			canvasElement.activatorButton.SetActive(false);
		}

		//Guard Button
		if(stat.block && GlobalStatus.freezeAll || stat.block && GlobalStatus.freezePlayer){
			stat.GuardBreak("cancelGuard");
		}

		//------Aiming---------
		if(attackPoint && aimAtMouse){
			Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(attackPoint.position);
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			/*if(limitAimAngle != Vector2.zero){
				angle = Mathf.Clamp(angle, limitAimAngle.x, limitAimAngle.y);
			}*/
			attackPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}

		//Release Charging
		/*if(Input.GetButtonUp("Fire1") && charging && !mobileMode){
			charging = false;
			if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || stat.block || stat.flinch){
				if(chargingEffect){
					Destroy(chargingEffect.gameObject);
				}
				c = 0;
				return;
			}
			int b = charge.Length -1;
			if(chargingEffect){
				Destroy(chargingEffect.gameObject);
			}
			while(b >= 0){
				if(Time.time > charge[b].currentChargeTime){
					//Charge Attack!!
					if(Time.time > (nextFire + 0.5f)){
						c = 0;
					}
					StartCoroutine(ChargeAttack());
					b = -1;
				}else{
					b--;
				}
			}
		}*/
		
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer){
			return;
		}
		if(stat.flinch){
			rb.velocity = stat.knock * stat.knockForce;
			return;
		}
		if(stat.block){
			if(!stat.flinch){
				rb.velocity = Vector2.zero;
			}
			return;
		}

		if(meleefwd){
			if(rb.gravityScale > 0){
				rb.velocity = new Vector2(0 , rb.velocity.y);
			}else{
				rb.velocity = Vector2.zero;
			}
			if(aimAtMouse && rb.gravityScale == 0 || GetComponent<TopDownFourDirection>()){
				Vector3 dir = attackPoint.TransformDirection(Vector3.right);
				//rb.AddForce(dir * 3200 * Time.deltaTime);

				rb.velocity = dir * 2.5f;
			}else{
				Vector3 dir = transform.TransformDirection(Vector3.right);

				if(rb.gravityScale > 0){
					rb.AddForce(dir * 3200 * Time.deltaTime);
				}else{
					rb.velocity = dir * 2.5f;
				}
			}
		}

		if(notActive){
			return;
		}
		if(draggingItemIcon && draggingItemIcon.gameObject.activeSelf == true){
			return;
		}

		//Normal Trigger
		/*if(Input.GetButton("Fire1") && Time.time > nextFire && !onAttacking && !mobileMode && !onShortCutArea && !GlobalStatus.menuOn && !charging && !onButtonMenu){
			if(Time.time > (nextFire + 0.5f)){
				c = 0;
			}
			//Attack Combo
			if(attackAnimationTrigger.Length >= 1){
				StartCoroutine(AttackCombo());
			}

			//Charging Weapon if the Weapon can charge and player hold the Attack Button
			if(charge.Length > 0 && !charging && Time.time > nextFire /2){
				charging = true;
				int b = charge.Length -1;
				while(b >= 0){
					charge[b].currentChargeTime = Time.time + charge[b].chargeTime;
					b--;
				}
			}
		}*/

		//Charging Effect
		if(charging){
			int b = charge.Length -1;
			while(b >= 0){
				if(Time.time > charge[b].currentChargeTime){
					if(charge[b].chargeEffect && chargingEffect != charge[b].chargeEffect){
						if(!chargingEffect || ch != b){
							if(chargingEffect){
								Destroy(chargingEffect.gameObject);
							}
							chargingEffect = Instantiate(charge[b].chargeEffect , transform.position, transform.rotation) as GameObject;
							chargingEffect.transform.parent = this.transform;
							ch = b;
						}
					}
					b = -1;
				}else{
					b--;
				}
			}
		}

	}

	public void UseShortcut(int slot){
        if(onAttacking) {
			return;
        }
		if(shortcuts.Length < slot +1){
			return;
		}
		if(shortcuts[slot].type == ShortcutType.Skill){
			//Skill
			skSelect = slot;
			TriggerSkill(skSelect);
		}
		if(shortcuts[slot].type == ShortcutType.Equipment){
			//Equipment
			inv.EquipItemFromID(shortcuts[slot].id);
			UpdateShortcut();
		}
		if(shortcuts[slot].type == ShortcutType.UsableItem){
			//Item
			inv.UseItemFromID(shortcuts[slot].id);
			UpdateShortcut();
		}
	}

	public Image draggingItemIcon;
	private int pickupShortcutId = 0;
	private int pickupShortcutType = 0;
	private bool onShortCutArea = false;
	private bool onDiscardArea = false;
	private int onShortCutSlot = 0;
	private bool onSwapping = false;
	private int swapSlot = 0;
	private ShortcutData tempShortcut;

	public void UpdateShortcut(){
		for(int a = 0; a < shortcutUi.Length; a++){
			if(shortcuts[a].type == ShortcutType.None){
				shortcutUi[a].iconImage.gameObject.SetActive(false);
				shortcutUi[a].coolDownBackground.SetActive(false);
				shortcutUi[a].quantityText.gameObject.SetActive(false);
			}
			if(shortcuts[a].type == ShortcutType.UsableItem){
				int s = inv.FindItemSlot(shortcuts[a].id);
				if(s < inv.itemSlot.Length){
					shortcutUi[a].iconImage.gameObject.SetActive(true);
					shortcutUi[a].coolDownBackground.SetActive(false);
					shortcutUi[a].quantityText.gameObject.SetActive(true);
					shortcutUi[a].iconImage.sprite = itemDB.usableItem[shortcuts[a].id].icon;
					shortcutUi[a].quantityText.text = inv.itemQuantity[s].ToString();
				}else{
					shortcutUi[a].iconImage.gameObject.SetActive(false);
					shortcutUi[a].coolDownBackground.SetActive(false);
					shortcutUi[a].quantityText.gameObject.SetActive(false);
				}
			}
			if(shortcuts[a].type == ShortcutType.Equipment){
				bool s = inv.CheckItem(shortcuts[a].id , 1 , 1);
				if(s){
					shortcutUi[a].iconImage.gameObject.SetActive(true);
					shortcutUi[a].coolDownBackground.SetActive(false);
					shortcutUi[a].quantityText.gameObject.SetActive(false);
					shortcutUi[a].iconImage.sprite = itemDB.equipment[shortcuts[a].id].icon;
				}else{
					shortcutUi[a].iconImage.gameObject.SetActive(false);
					shortcutUi[a].coolDownBackground.SetActive(false);
					shortcutUi[a].quantityText.gameObject.SetActive(false);
				}
			}
			if(shortcuts[a].type == ShortcutType.Skill){
				shortcutUi[a].iconImage.gameObject.SetActive(true);
				//shortcutUi[a].coolDownBackground.SetActive(false);
				shortcutUi[a].quantityText.gameObject.SetActive(false);
				shortcutUi[a].iconImage.sprite = skillDB.skill[shortcuts[a].id].icon;
			}
		}
	}

	public void GuardUp() {
		if(canBlock && !onAttacking && !stat.block) {
			stat.mainSprite.ResetTrigger("cancelGuard");
			stat.GuardUp(blockingAnimationTrigger);
			rb.velocity = Vector2.zero;
		}
	}

	public void EnterShortcutArea(int slot){
		print("Shortcut Area = " + slot);
		onShortCutArea = true;
		onShortCutSlot = slot;
		onDiscardArea = false;
	}

	public void ExitShortcutArea(){
		if(Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer){
			onShortCutArea = false;
		}
	}

	public void EnterDiscardArea(){
		onDiscardArea = true;
		onShortCutArea = false;
	}
	
	public void ExitSDiscardArea(){
		if(Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer){
			onDiscardArea = false;
		}
	}

	public void PickupForShortcut(int id , int type){
		pickupShortcutId = id;
		pickupShortcutType = type;
		draggingItemIcon.gameObject.SetActive(true);
	}

	public void PickupForSwap(int slot){
		pickupShortcutId = shortcuts[slot].id;
		pickupShortcutType = (int)shortcuts[slot].type;
		draggingItemIcon.sprite = shortcutUi[slot].iconImage.sprite;
		draggingItemIcon.gameObject.SetActive(true);
		swapSlot = slot;
		onSwapping = true;
	}

	public void SetShortcut(){
		draggingItemIcon.gameObject.SetActive(false);
		if(onDiscardArea){
			if(pickupShortcutType == 1 || pickupShortcutType == 2){
				DropItem();
			}
			onSwapping = false;
			return;
		}
		if(!onShortCutArea){
			onSwapping = false;
			return;
		}
		if(onSwapping){
			tempShortcut = shortcuts[onShortCutSlot];
			shortcuts[onShortCutSlot] = shortcuts[swapSlot];
			shortcuts[swapSlot] = tempShortcut;
			onSwapping = false;
			DiscardShortcut();
			UpdateShortcut();
			return;
		}
		if(shortcuts[onShortCutSlot].onCoolDown > 0){
			StartCoroutine(ShowPrintingText("This Skill is on Cooldown!!"));
			return;
		}
		shortcuts[onShortCutSlot].id = pickupShortcutId;
		if(pickupShortcutType == 1){
			shortcuts[onShortCutSlot].type = ShortcutType.UsableItem;
		}
		if(pickupShortcutType == 2){
			shortcuts[onShortCutSlot].type = ShortcutType.Equipment;
		}
		if(pickupShortcutType == 3){
			shortcuts[onShortCutSlot].type = ShortcutType.Skill;
			GetComponent<SkillStatus>().AssignSkillByID(onShortCutSlot , pickupShortcutId);
		}
		onSwapping = false;
		CheckSameShortcut();
		DiscardShortcut();
		UpdateShortcut();
	}

	void CheckSameShortcut(){
		int n = 0;
		while(n < shortcuts.Length){
			if(shortcuts[n].id == pickupShortcutId && (int)shortcuts[n].type == pickupShortcutType && n != onShortCutSlot){
				shortcuts[n].type = ShortcutType.None;
				shortcuts[n].skill.manaCost = 0;
				shortcuts[n].skill.skillPrefab = null;

				shortcuts[n].skill.icon = null;
				shortcuts[n].skill.sendMsg = "";
				shortcuts[n].skill.castEffect = null;
				
				shortcuts[n].skill.castTime = 0;
				shortcuts[n].skill.skillDelay = 0;
				shortcuts[n].skill.coolDown = 0;

				shortcuts[n].skill.requireWeapon = false;
				shortcuts[n].skill.requireWeaponType = 0;
				
				shortcuts[n].skill.soundEffect = null;
				
				if(shortcuts[n].onCoolDown > 0){
					shortcuts[onShortCutSlot].onCoolDown = shortcuts[n].onCoolDown;
				}
				shortcuts[n].onCoolDown = 0;
			}
			n++;
		}
	}

	public void SetupInitialShortcut(){
		for(int a = 0; a < shortcuts.Length; a++){
			if(shortcuts[a].type == ShortcutType.Skill){
				GetComponent<SkillStatus>().AssignSkillByID(a , shortcuts[a].id);
			}
		}
		UpdateShortcut();
	}

	public void EnterButtonMenu(bool c){
		onButtonMenu = c;
	}

	public void DropItem(){
		draggingItemIcon.gameObject.SetActive(false);
		if(!onDiscardArea){
			return;
		}
		if(pickupShortcutType == 1){
			int slot = GetComponent<Inventory>().FindItemSlot(pickupShortcutId);
			if(slot < GetComponent<Inventory>().itemSlot.Length){
				int qty = GetComponent<Inventory>().itemQuantity[slot];
				GetComponent<Inventory>().RemoveItem(pickupShortcutId , qty);
				if(itemDB.usableItem[pickupShortcutId].dropPrefab){
					Vector3 dropPos = transform.position;
					int ran = Random.Range(0 , 100);
					if(rb.gravityScale > 0){
						if(ran >= 50){
							dropPos.x += Random.Range(1 , 1.8f);
						}else{
							dropPos.x -= Random.Range(1 , 1.8f);
						}
						dropPos.y += Random.Range(1.1f , 1.4f);
					}else{
						if(ran >= 75){
							dropPos.x += Random.Range(1.2f , 1.5f);
							dropPos.y += Random.Range(-1.2f , 1.2f);
						}else if(ran >= 50){
							dropPos.x -= Random.Range(1.2f , 1.5f);
							dropPos.y += Random.Range(-1.2f , 1.2f);
						}else if(ran >= 25){
							dropPos.x += Random.Range(-1.2f , 1.2f);
							dropPos.y += Random.Range(1.2f , 1.5f);
						}else{
							dropPos.x += Random.Range(-1.2f , 1.2f);
							dropPos.y -= Random.Range(1.2f , 1.5f);
						}
					}

					Transform drop = itemDB.usableItem[pickupShortcutId].dropPrefab.transform;
					if(dropItemPrefab){
						drop = dropItemPrefab;
					}

					Transform i = Instantiate(drop , dropPos , Quaternion.identity) as Transform;
					/*i.GetComponent<AddItem>().itemID = pickupShortcutId;
					i.GetComponent<AddItem>().itemType = ItType.Usable;
					i.GetComponent<AddItem>().itemQuantity = qty;*/

					i.GetComponentInChildren<AddItem>().itemID = pickupShortcutId;
					i.GetComponentInChildren<AddItem>().itemType = ItType.Usable;
					i.GetComponentInChildren<AddItem>().itemQuantity = qty;

					if(rb.gravityScale == 0 && i.GetComponent<Rigidbody2D>()) {
						i.GetComponent<Rigidbody2D>().gravityScale = 0;
					}

					if(i.GetComponent<SpriteRenderer>()){
						i.GetComponent<SpriteRenderer>().sprite = itemDB.usableItem[pickupShortcutId].icon;
					}
				}
			}
		}else if(pickupShortcutType == 2){
			GetComponent<Inventory>().RemoveEquipment(pickupShortcutId);
			if(itemDB.equipment[pickupShortcutId].dropPrefab){
				Vector3 dropPos = transform.position;
				int ran = Random.Range(0 , 100);
				if(rb.gravityScale > 0){
					if(ran >= 50){
						dropPos.x += Random.Range(1 , 1.8f);
					}else{
						dropPos.x -= Random.Range(1 , 1.8f);
					}
					dropPos.y += Random.Range(1.1f , 1.4f);
				}else{
					if(ran >= 75){
						dropPos.x += Random.Range(1.2f , 1.5f);
						dropPos.y += Random.Range(-1.2f , 1.2f);
					}else if(ran >= 50){
						dropPos.x -= Random.Range(1.2f , 1.5f);
						dropPos.y += Random.Range(-1.2f , 1.2f);
					}else if(ran >= 25){
						dropPos.x += Random.Range(-1.2f , 1.2f);
						dropPos.y += Random.Range(1.2f , 1.5f);
					}else{
						dropPos.x += Random.Range(-1.2f , 1.2f);
						dropPos.y -= Random.Range(1.2f , 1.5f);
					}
				}

				Transform drop = itemDB.equipment[pickupShortcutId].dropPrefab.transform;
				if(dropItemPrefab){
					drop = dropItemPrefab;
				}

				Transform i = Instantiate(drop , dropPos , Quaternion.identity) as Transform;
				//i.GetComponent<AddItem>().itemID = pickupShortcutId;
				//i.GetComponent<AddItem>().itemType = ItType.Equipment;

				i.GetComponentInChildren<AddItem>().itemID = pickupShortcutId;
				i.GetComponentInChildren<AddItem>().itemType = ItType.Equipment;

				if(rb.gravityScale == 0 && i.GetComponent<Rigidbody2D>()){
					i.GetComponent<Rigidbody2D>().gravityScale = 0;
				}

				if(i.GetComponent<SpriteRenderer>()){
					i.GetComponent<SpriteRenderer>().sprite = itemDB.equipment[pickupShortcutId].icon;
				}
			}
		}
	}

	public void DiscardShortcut(){
		pickupShortcutId = 0;
		pickupShortcutType = 0;
	}

	public void TriggerAttack(){
		if(Time.timeScale == 0.0f || GetComponent<Status>().freeze || GlobalStatus.freezePlayer){
			return;
		}
		if(Time.time > nextFire && !onAttacking && !onShortCutArea && !GlobalStatus.menuOn && !charging && !onButtonMenu) {
			if(Time.time > (nextFire + 0.5f)){
				c = 0;
			}
			//Attack Combo
			if(attackAnimationTrigger.Length >= 1 && !charging){
				StartCoroutine(AttackCombo());
			}
			//Charging Weapon if the Weapon can charge and player hold the Attack Button
			if(charge.Length > 0 && !charging && Time.time > nextFire /2){
				charging = true;
				int b = charge.Length -1;
				while(b >= 0){
					charge[b].currentChargeTime = Time.time + charge[b].chargeTime;
					b--;
				}
			}
		}
	}

	public void ReleaseCharge(){
		if(charging){
			charging = false;
			if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || stat.block || stat.flinch){
				if(chargingEffect){
					Destroy(chargingEffect.gameObject);
				}
				c = 0;
				return;
			}
			int b = charge.Length -1;
			if(chargingEffect){
				Destroy(chargingEffect.gameObject);
			}
			while(b >= 0){
				if(Time.time > charge[b].currentChargeTime){
					//Charge Attack!!
					if(Time.time > (nextFire + 0.5f)){
						c = 0;
					}
					StartCoroutine(ChargeAttack());
					b = -1;
				}else{
					b--;
				}
			}
		}
	}

	public void LookAtMouse(){
		Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

		if(GetComponent<TopDownFourDirection>()){
			if(delta.y > 1 && Mathf.Abs(delta.x) < 2){
				GetComponent<TopDownFourDirection>().SetDirection(2);
			}else if(delta.y < -1 && Mathf.Abs(delta.x) < 2) {
				GetComponent<TopDownFourDirection>().SetDirection(3);
			}else if(delta.x >= 0){
				GetComponent<TopDownFourDirection>().SetDirection(0);
			}else if(delta.x < 0){
				GetComponent<TopDownFourDirection>().SetDirection(1);
			}
			return;
		}

		if(delta.x >= 0 && !facingRight){
			Vector3 rot = transform.eulerAngles;
			rot.y = 0;
			transform.eulerAngles = rot;
			facingRight = true;
		}else if (delta.x < 0 && facingRight){
			Vector3 rot = transform.eulerAngles;
			rot.y = 180;
			transform.eulerAngles = rot;
			facingRight = false;
		}
	}

	IEnumerator ShowPrintingText(string txt){
		if(!textPrinter){
			yield break;
		}
		textPrinter.text = txt;
		textPrinter.gameObject.SetActive(true);
		yield return new WaitForSeconds(3.9f);
		textPrinter.gameObject.SetActive(false);
	}

	public void PrintingText(string txt){
		StartCoroutine(ShowPrintingText(txt));
	}
	
	IEnumerator AttackCombo(){
		int atkPref = c;
		if(c >= attackAnimationTrigger.Length){
			c = 0;
		}
		if(atkPref >= attackPrefab.Length){
			atkPref = 0;
		}
		if(attackPrefab.Length > 0 && !attackPrefab[atkPref]){
			print("You didn't assign Attack Prefab yet");
			yield break;
		}
		if(stat.dodge){
			yield break;
		}
		if(requireItemId > 0){
			bool have = GetComponent<Inventory>().RemoveItem(requireItemId , 1);
			if(!have){
				print("Require " + requireItemName);
				StartCoroutine(ShowPrintingText("Require " + requireItemName));
				yield break;
			}
		}
		int str = GetComponent<Status>().totalStat.atk;
		int matk = GetComponent<Status>().totalStat.matk;
		onAttacking = true;
		// If Melee Dash
		if(aimAtMouse) {
			LookAtMouse();
		}
		if(whileAttack == WhileAtk.MeleeFwd){
			GetComponent<Status>().canControl = false;
			StartCoroutine(MeleeDash());
		}
		// If Immobile
		if(whileAttack == WhileAtk.Immobile){
			GetComponent<Status>().canControl = false;
		}
		if(sound.attackComboVoice.Length > c && sound.attackComboVoice[c]){
			GetComponent<AudioSource>().PlayOneShot(sound.attackComboVoice[c]);
		}
		if(attackSoundEffect){
			GetComponent<AudioSource>().PlayOneShot(attackSoundEffect);
		}
		if(attackAnimationTrigger[c] != ""){
			stat.mainSprite.SetTrigger(attackAnimationTrigger[c]);
		}
		
		yield return new WaitForSeconds(attackCast);
		c++;

		nextFire = Time.time + attackDelay;
        if (attackPrefab.Length > 0){
			Transform bulletShootout = Instantiate(attackPrefab[atkPref].transform, attackPoint.transform.position, attackPoint.transform.rotation) as Transform;
			bulletShootout.gameObject.SetActive(true);
			bulletShootout.GetComponent<BulletStatus>().Setting(str, matk, "Player", this.gameObject);
			if (GetComponent<Status>().hiddenStatus.drainTouch > 0){
				bulletShootout.GetComponent<BulletStatus>().drainHp += GetComponent<Status>().hiddenStatus.drainTouch;
			}
		}
		
		if(c >= attackAnimationTrigger.Length){
			c = 0;
		}
		yield return new WaitForSeconds(attackDelay);
		
		onAttacking = false;
		GetComponent<Status>().canControl = true;
	}

	IEnumerator ChargeAttack(){
		charging = false;
		if(!charge[ch].chargeAttackPrefab){
			print("You didn't assign Attack Prefab yet");
			yield break;
		}
		if(stat.dodge){
			yield break;
		}
		if(requireItemId > 0){
			bool have = GetComponent<Inventory>().RemoveItem(requireItemId , 1);
			if(!have){
				print("Require " + requireItemName);
				StartCoroutine(ShowPrintingText("Require " + requireItemName));
				yield break;
			}
		}
		
		int str = GetComponent<Status>().totalStat.atk;
		int matk = GetComponent<Status>().totalStat.matk;
		onAttacking = true;
		
		// If Melee Dash
		if(whileAttack == WhileAtk.MeleeFwd){
			GetComponent<Status>().canControl = false;
			StartCoroutine(MeleeDash());
		}
		// If Immobile
		if(whileAttack == WhileAtk.Immobile){
			GetComponent<Status>().canControl = false;
		}
		
		if(charge[ch].soundEffect){
			GetComponent<AudioSource>().PlayOneShot(charge[ch].soundEffect);
		}
		if(charge[ch].soundEffect2){
			GetComponent<AudioSource>().PlayOneShot(charge[ch].soundEffect2);
		}
		if(aimAtMouse){
			LookAtMouse();
		}
		if(charge[ch].chargeAnimationTrigger != ""){
			stat.mainSprite.SetTrigger(charge[ch].chargeAnimationTrigger);
		}
		
		yield return new WaitForSeconds(charge[ch].attackCast);
		c++;
		
		nextFire = Time.time + charge[ch].attackDelay;
		Transform bulletShootout = Instantiate(charge[ch].chargeAttackPrefab.transform, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
		bulletShootout.gameObject.SetActive(true);
		bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
		if(GetComponent<Status>().hiddenStatus.drainTouch > 0){
			bulletShootout.GetComponent<BulletStatus>().drainHp += GetComponent<Status>().hiddenStatus.drainTouch;
		}
		yield return new WaitForSeconds(charge[ch].attackDelay);
		ch = 0;
		onAttacking = false;
		GetComponent<Status>().canControl = true;
	}

	public void TriggerSkill(int sk){
		if(Time.timeScale == 0.0f || GetComponent<Status>().freeze || onAttacking || !shortcuts[sk].skill.skillPrefab || GlobalStatus.freezePlayer){
			return;
		}
		StartCoroutine(MagicSkill(sk));
	}

	private GameObject castEff;
	IEnumerator MagicSkill(int skillID){
		if(shortcuts[skillID].skill.requireWeapon && weaponType != shortcuts[skillID].skill.requireWeaponType){
			//Check Weapon Type for Use Skill
			print("Cannot Use Skill with this Weapon");
			StartCoroutine(ShowPrintingText("Cannot Use Skill with this Weapon"));
			yield break;
		}
		if(shortcuts[skillID].onCoolDown > 0 || GetComponent<Status>().silence){
			yield break;
		}
		c = 0;
		int cost = shortcuts[skillID].skill.manaCost;
		if(GetComponent<Status>().hiddenStatus.mpReduce > 0){
			//Calculate MP Reduce
			int per = 100 - GetComponent<Status>().hiddenStatus.mpReduce;
			if(per < 0){
				per = 0;
			}
			cost *= per;
			cost /= 100;
		}
		if(GetComponent<Status>().mana >= cost){
			if(shortcuts[skillID].skill.sendMsg != ""){
				SendMessage(shortcuts[skillID].skill.sendMsg , SendMessageOptions.DontRequireReceiver);
			}
			GetComponent<Status>().mana -= cost;

			int str = GetComponent<Status>().totalStat.atk;
			int matk = GetComponent<Status>().totalStat.matk;
			
			if(sound.magicCastVoice){
				GetComponent<AudioSource>().clip = sound.magicCastVoice;
				GetComponent<AudioSource>().Play();
			}
			onAttacking = true;
			// If Melee Dash
			if(shortcuts[skillID].skill.whileAttack == WhileAtk.MeleeFwd){
				GetComponent<Status>().canControl = false;
				meleefwd = true;
			}
			// If Immobile
			if(shortcuts[skillID].skill.whileAttack == WhileAtk.Immobile){
				GetComponent<Status>().canControl = false;
			}
			if(aimAtMouse){
				LookAtMouse();
			}
			if(shortcuts[skillID].skill.skillAnimationTrigger != ""){
				stat.mainSprite.SetTrigger(shortcuts[skillID].skill.skillAnimationTrigger);
			}
			if(shortcuts[skillID].skill.castEffect){
				castEff = Instantiate(shortcuts[skillID].skill.castEffect , transform.position , transform.rotation) as GameObject;
				castEff.transform.parent = this.transform;
			}
			nextFire = Time.time + shortcuts[skillID].skill.skillDelay;

			yield return new WaitForSeconds(shortcuts[skillID].skill.castTime);
			if(castEff){
				Destroy(castEff);
			}
			//onAttacking = true;
			if(shortcuts[skillID].skill.skillSpawn == BSpawnType.FromPlayer){
				Transform bulletShootout = Instantiate(shortcuts[skillID].skill.skillPrefab.transform, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
				bulletShootout.gameObject.SetActive(true);
				bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
			}else{
				Vector2 skillPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				Transform bulletShootout = Instantiate(shortcuts[skillID].skill.skillPrefab.transform, skillPos , attackPoint.transform.rotation) as Transform;
				bulletShootout.gameObject.SetActive(true);
				bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
			}
			if(shortcuts[skillID].skill.soundEffect){
				GetComponent<AudioSource>().PlayOneShot(shortcuts[skillID].skill.soundEffect);
			}
			yield return new WaitForSeconds(shortcuts[skillID].skill.skillDelay);
			
			//Addition Hit
			for(int m = 0; m < shortcuts[skillID].skill.multipleHit.Length; m++){
				if(shortcuts[skillID].skill.multipleHit[m].skillAnimationTrigger != ""){
					stat.mainSprite.SetTrigger(shortcuts[skillID].skill.multipleHit[m].skillAnimationTrigger);
				}
				yield return new WaitForSeconds(shortcuts[skillID].skill.multipleHit[m].castTime);
				
				if(shortcuts[skillID].skill.skillSpawn == BSpawnType.FromPlayer){
					Transform bulletShootout = Instantiate(shortcuts[skillID].skill.multipleHit[m].skillPrefab.transform, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
					bulletShootout.gameObject.SetActive(true);
					bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
				}else{
					/*Transform bulletShootout = Instantiate(shortcuts[skillID].skill.multipleHit[m].skillPrefab.transform, skillSpawnPos , transform.rotation) as Transform;
					bulletShootout.gameObject.SetActive(true);
					bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);*/
	}
				if(shortcuts[skillID].skill.multipleHit[m].soundEffect){
					GetComponent<AudioSource>().PlayOneShot(shortcuts[skillID].skill.multipleHit[m].soundEffect);
				}
				yield return new WaitForSeconds(shortcuts[skillID].skill.multipleHit[m].skillDelay);
			}
			
			shortcuts[skillID].onCoolDown = shortcuts[skillID].skill.coolDown;
			onAttacking = false;
			//onAttacking = false;
			meleefwd = false;
			GetComponent<Status>().canControl = true;
		}else{
			StartCoroutine(ShowPrintingText("Not Enough MP!!"));
		}
	}

	IEnumerator MeleeDash(){
		meleefwd = true;
		yield return new WaitForSeconds(0.2f);
		meleefwd = false;
	}

	public void GetActivator(GameObject obj , string msg , string btn){
		actvateObj = obj;
		actvateMsg = msg;
		buttonText = btn;
		showButton = true;
		if(canvasElement.activatorText){
			canvasElement.activatorText.text = btn;
		}
	}
	
	public void RemoveActivator(GameObject obj){
		if(obj == actvateObj){
			actvateObj = null;
			actvateMsg = "";
			buttonText = "";
			showButton = false;
			if(canvasElement.activatorText){
				canvasElement.activatorText.text = "";
			}
		}
	}
	
	public void Activator(){
		if(!actvateObj || actvateMsg == "" || stat.freeze){
			return;
		}
		actvateObj.SendMessage(actvateMsg , SendMessageOptions.DontRequireReceiver);
	}

	public void TriggerGuard(){
		if(canBlock && !onAttacking && !stat.block){
			stat.mainSprite.ResetTrigger("cancelGuard");
			stat.GuardUp(blockingAnimationTrigger);
			rb.velocity = Vector2.zero;
		}
	}

	public void CancelGuard(){
		if(stat.block){
			stat.GuardBreak("cancelGuard");
		}
	}
}

[System.Serializable]
public class ChargeAtk{
	public GameObject chargeEffect;
	public BulletStatus chargeAttackPrefab;
	public float chargeTime = 1.0f;
	public string chargeAnimationTrigger;
	public float attackCast = 0.18f;
	public float attackDelay = 0.12f;

	public AudioClip soundEffect;
	public AudioClip soundEffect2;
	[HideInInspector]
	public float currentChargeTime = 1.0f;
}
