using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUi : MonoBehaviour {
	[System.Serializable]
	public class SkillSlot{
		public string skillName = "";
		public int skillId = 0;
		public SkillTreeButton skillButton;
		public int[] unlockConditionId;
		public int unlockLevel = 1;
		public int skPointUse = 1;

		[HideInInspector]
		public bool learned = false;
		public bool locked = false;
	}

	public SkillSlot[] skillSlots = new SkillSlot[5];
	public Text skillPointText;
	public GameObject player;
	private int buttonSelect = 0;

	public GameObject tooltip;
	public Image tooltipIcon;
	public Text tooltipName;
	public Text tooltipText;
	public Text mpCostTooltip;
	public SkillData database;

	public void Start(){
		SetButtonID();
		CheckUnlockSkill();
		UpdateSkillButton();
	}

	void SetButtonID(){
		for(int a = 0; a < skillSlots.Length; a++){
			if(skillSlots[a].skillButton){
				skillSlots[a].skillButton.buttonId = a;
				skillSlots[a].skillButton.skillTree = GetComponent<SkillTreeUi>();
				skillSlots[a].skillButton.SettingUp();
			}
		}
	}
	
	void Update(){
		if(!player){
			return;
		}
		if(skillPointText){
			skillPointText.text = player.GetComponent<Status>().skillPoint.ToString();
		}
	}
	
	public void CheckLearnedSkill(){
		if(!player){
			return;
		}
		SkillStatus sk = player.GetComponent<SkillStatus>();
		for(int a = 0; a < skillSlots.Length; a++){
			if(skillSlots[a].skillId > 0){
				skillSlots[a].learned = sk.HaveSkill(skillSlots[a].skillId);
			}
		}
	}
	
	public void CheckUnlockSkill(){
		if(!player){
			return;
		}
		CheckLearnedSkill();
		SkillStatus sk = player.GetComponent<SkillStatus>();
		int lv = player.GetComponent<Status>().level;
		
		for(int a = 0; a < skillSlots.Length; a++){
			//Check Player Level
			bool  lvPass = false;
			int allUnlock = 0;
			if(lv >= skillSlots[a].unlockLevel){
				lvPass = true;
			}
			//Check unlockConditionId
			if(skillSlots[a].unlockConditionId.Length > 0){
				allUnlock = 0;
				for(int b = 0; b < skillSlots[a].unlockConditionId.Length; b++){
					if(sk.HaveSkill(skillSlots[a].unlockConditionId[b])){
						allUnlock++;
					}
				}
			}
			//If Overall Pass
			if(lvPass && allUnlock >= skillSlots[a].unlockConditionId.Length){
				skillSlots[a].locked = false;
			}
		}
	}
	
	public void ButtonSkillClick(int buttonId){
		if(!player || skillSlots[buttonId].locked){
			return;
		}
		if(!skillSlots[buttonId].learned){
			LearnSkill(buttonId);
		}else{
			buttonSelect = buttonId;
			//ShowShortcutSetting();
		}
	}
	
	public void SetSlot(int slot){
		SkillStatus sk = player.GetComponent<SkillStatus>();
		sk.AssignSkillByID(slot , skillSlots[buttonSelect].skillId);
	}
	
	public void LearnSkill(int buttonId){
		if(player.GetComponent<Status>().skillPoint < skillSlots[buttonId].skPointUse){
			player.GetComponent<AttackTrigger>().PrintingText("Not enough Skill Point");
			print("Not enough Skill Point");
			return;
		}
		player.GetComponent<Status>().skillPoint -= skillSlots[buttonId].skPointUse;
		SkillStatus sk = player.GetComponent<SkillStatus>();
		
		sk.AddSkill(skillSlots[buttonId].skillId);
		CheckUnlockSkill();
		UpdateSkillButton();
	}
	
	public void UpdateSkillButton(){
		SkillTreeButton[] skillButton;
		skillButton = GetComponentsInChildren<SkillTreeButton>();
		if(skillButton != null) {
			foreach(SkillTreeButton button in skillButton){
				button.UpdateIcon();
			}
		}
	}
	
	public void CloseMenu(){
		Time.timeScale = 1.0f;
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		GlobalStatus.menuOn = false;
		gameObject.SetActive(false);
	}
}
