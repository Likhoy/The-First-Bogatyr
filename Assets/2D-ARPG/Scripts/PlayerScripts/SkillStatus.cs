using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStatus : MonoBehaviour {
	public SkillData database;
	
	//public int[] skill = new int[8];
	public int[] skillListSlot = new int[30];
	
	//private string showSkillName = "";
	//public bool autoAssignSkill = false;

	/*void Start(){
		if(autoAssignSkill){
			AssignAllSkill();
		}
	}*/
	//-----------------------

	public void AssignSkillByID(int slot , int skillId){
		//Use With Canvas UI
		if(slot > GetComponent<AttackTrigger>().skillShortcuts.Length){
			return;
		}
		if(GetComponent<AttackTrigger>().skillShortcuts[slot].onCoolDown > 0 || GetComponent<AttackTrigger>().onAttacking){
			print("This Skill is not Ready");
			return;
		}
		//GetComponent<AttackTrigger>().SetShortcut(slot);
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.manaCost = database.skill[skillId].manaCost;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.skillPrefab = database.skill[skillId].skillPrefab;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.skillAnimationTrigger = database.skill[skillId].skillAnimationTrigger;

		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.icon = database.skill[skillId].icon;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.sendMsg = database.skill[skillId].sendMsg;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.castEffect = database.skill[skillId].castEffect;
		
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.castTime = database.skill[skillId].castTime;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.skillDelay = database.skill[skillId].skillDelay;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.whileAttack = database.skill[skillId].whileAttack;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.coolDown = database.skill[skillId].coolDown;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.skillSpawn = database.skill[skillId].skillSpawn;
		
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.requireWeapon = database.skill[skillId].requireWeapon;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.requireWeaponType = database.skill[skillId].requireWeaponType;
		
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.soundEffect = database.skill[skillId].soundEffect;
		
		int mh = database.skill[skillId].multipleHit.Length;
		GetComponent<AttackTrigger>().skillShortcuts[slot].skill.multipleHit = new SkillAdditionHit[mh];
		for(int m = 0; m < mh; m++){
			GetComponent<AttackTrigger>().skillShortcuts[slot].skill.multipleHit[m] = new SkillAdditionHit();
			
			GetComponent<AttackTrigger>().skillShortcuts[slot].skill.multipleHit[m].skillPrefab = database.skill[skillId].multipleHit[m].skillPrefab;
			GetComponent<AttackTrigger>().skillShortcuts[slot].skill.multipleHit[m].skillAnimationTrigger = database.skill[skillId].multipleHit[m].skillAnimationTrigger;

			GetComponent<AttackTrigger>().skillShortcuts[slot].skill.multipleHit[m].castTime = database.skill[skillId].multipleHit[m].castTime;
			GetComponent<AttackTrigger>().skillShortcuts[slot].skill.multipleHit[m].skillDelay = database.skill[skillId].multipleHit[m].skillDelay;
			
			GetComponent<AttackTrigger>().skillShortcuts[slot].skill.multipleHit[m].soundEffect = database.skill[skillId].multipleHit[m].soundEffect;
		}
		
		CheckSameSkill(GetComponent<AttackTrigger>().skillShortcuts[slot].id , slot);
	}
	
	public void AssignAllSkill(){
		AttackTrigger atk = GetComponent<AttackTrigger>();
		int n = 0;
		while(n < GetComponent<AttackTrigger>().skillShortcuts.Length){
			if(GetComponent<AttackTrigger>().skillShortcuts[n].type == AttackTrigger.ShortcutType.Skill){
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.manaCost = database.skill[atk.skillShortcuts[n].id].manaCost;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillPrefab = database.skill[atk.skillShortcuts[n].id].skillPrefab;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillAnimationTrigger = database.skill[atk.skillShortcuts[n].id].skillAnimationTrigger;
				
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.icon = database.skill[atk.skillShortcuts[n].id].icon;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.sendMsg = database.skill[atk.skillShortcuts[n].id].sendMsg;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.castEffect = database.skill[atk.skillShortcuts[n].id].castEffect;
				
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.castTime = database.skill[atk.skillShortcuts[n].id].castTime;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillDelay = database.skill[atk.skillShortcuts[n].id].skillDelay;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.whileAttack = database.skill[atk.skillShortcuts[n].id].whileAttack;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.coolDown = database.skill[atk.skillShortcuts[n].id].coolDown;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillSpawn = database.skill[atk.skillShortcuts[n].id].skillSpawn;
				
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.requireWeapon = database.skill[atk.skillShortcuts[n].id].requireWeapon;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.requireWeaponType = database.skill[atk.skillShortcuts[n].id].requireWeaponType;
				
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.soundEffect = database.skill[atk.skillShortcuts[n].id].soundEffect;
				
				int mh = database.skill[atk.skillShortcuts[n].id].multipleHit.Length;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.multipleHit = new SkillAdditionHit[mh];
				for(int m = 0; m < mh; m++){
					GetComponent<AttackTrigger>().skillShortcuts[n].skill.multipleHit[m] = new SkillAdditionHit();
					
					GetComponent<AttackTrigger>().skillShortcuts[n].skill.multipleHit[m].skillPrefab = database.skill[atk.skillShortcuts[n].id].multipleHit[m].skillPrefab;
					GetComponent<AttackTrigger>().skillShortcuts[n].skill.multipleHit[m].skillAnimationTrigger = database.skill[atk.skillShortcuts[n].id].multipleHit[m].skillAnimationTrigger;
					
					GetComponent<AttackTrigger>().skillShortcuts[n].skill.multipleHit[m].castTime = database.skill[atk.skillShortcuts[n].id].multipleHit[m].castTime;
					GetComponent<AttackTrigger>().skillShortcuts[n].skill.multipleHit[m].skillDelay = database.skill[atk.skillShortcuts[n].id].multipleHit[m].skillDelay;
					
					GetComponent<AttackTrigger>().skillShortcuts[n].skill.multipleHit[m].soundEffect = database.skill[atk.skillShortcuts[n].id].multipleHit[m].soundEffect;
				}
				n++;
			}
		}
		/*if(GetComponent<UiMasterC>()){
			GetComponent<UiMasterC>().SetSkillShortCutIcons();
		}*/
	}
	
	void CheckSameSkill(int id , int slot){
		//print (id + " + " + slot);
		int n = 0;
		while(n < GetComponent<AttackTrigger>().skillShortcuts.Length){
			if(GetComponent<AttackTrigger>().skillShortcuts[n].id == id && n != slot){
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.manaCost = 0;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillPrefab = null;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillAnimationTrigger = database.skill[0].skillAnimationTrigger;

				GetComponent<AttackTrigger>().skillShortcuts[n].skill.icon = null;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.sendMsg = "";
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.castEffect = null;
				
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.castTime = 0;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillDelay = 0;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.whileAttack = database.skill[0].whileAttack;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.coolDown = 0;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.skillSpawn = database.skill[0].skillSpawn;
				
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.requireWeapon = false;
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.requireWeaponType = 0;
				
				GetComponent<AttackTrigger>().skillShortcuts[n].skill.soundEffect = null;
				
				if(GetComponent<AttackTrigger>().skillShortcuts[n].onCoolDown > 0){
					GetComponent<AttackTrigger>().skillShortcuts[slot].onCoolDown = GetComponent<AttackTrigger>().skillShortcuts[n].onCoolDown;
				}
				GetComponent<AttackTrigger>().skillShortcuts[n].onCoolDown = 0;
			}
			n++;
		}
		/*if(GetComponent<UiMasterC>()){
			GetComponent<UiMasterC>().SetSkillShortCutIcons();
		}*/
	}
	
	public void AddSkill(int id){
		bool geta= false;
		int pt = 0;
		while(pt < skillListSlot.Length && !geta){
			if(skillListSlot[pt] == id){
				// Check if you already have this skill.
				geta = true;
			}else if(skillListSlot[pt] == 0){
				// Add Skill to empty slot.
				skillListSlot[pt] = id;
				//StartCoroutine(ShowLearnedSkill(id));
				geta = true;
			}else{
				pt++;
			}
		}
	}
	
	/*IEnumerator ShowLearnedSkill(int id){
		showSkillName = database.skill[id].skillName;
		yield return new WaitForSeconds(10.5f);
	}*/
	
	public bool HaveSkill(int id){
		bool have = false;
		for(int a = 0; a < skillListSlot.Length; a++){
			if(skillListSlot[a] == id){
				have = true;
			}
		}
		return have;
	}
}
