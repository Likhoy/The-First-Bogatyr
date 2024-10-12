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
		if(slot > GetComponent<AttackTrigger>().shortcuts.Length){
			return;
		}
		if(GetComponent<AttackTrigger>().shortcuts[slot].onCoolDown > 0 || GetComponent<AttackTrigger>().onAttacking){
			print("This Skill is not Ready");
			return;
		}
		//GetComponent<AttackTrigger>().SetShortcut(slot);
		GetComponent<AttackTrigger>().shortcuts[slot].skill.manaCost = database.skill[skillId].manaCost;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.skillPrefab = database.skill[skillId].skillPrefab;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.skillAnimationTrigger = database.skill[skillId].skillAnimationTrigger;

		GetComponent<AttackTrigger>().shortcuts[slot].skill.icon = database.skill[skillId].icon;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.sendMsg = database.skill[skillId].sendMsg;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.castEffect = database.skill[skillId].castEffect;
		
		GetComponent<AttackTrigger>().shortcuts[slot].skill.castTime = database.skill[skillId].castTime;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.skillDelay = database.skill[skillId].skillDelay;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.whileAttack = database.skill[skillId].whileAttack;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.coolDown = database.skill[skillId].coolDown;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.skillSpawn = database.skill[skillId].skillSpawn;
		
		GetComponent<AttackTrigger>().shortcuts[slot].skill.requireWeapon = database.skill[skillId].requireWeapon;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.requireWeaponType = database.skill[skillId].requireWeaponType;
		
		GetComponent<AttackTrigger>().shortcuts[slot].skill.soundEffect = database.skill[skillId].soundEffect;
		
		int mh = database.skill[skillId].multipleHit.Length;
		GetComponent<AttackTrigger>().shortcuts[slot].skill.multipleHit = new SkillAdditionHit[mh];
		for(int m = 0; m < mh; m++){
			GetComponent<AttackTrigger>().shortcuts[slot].skill.multipleHit[m] = new SkillAdditionHit();
			
			GetComponent<AttackTrigger>().shortcuts[slot].skill.multipleHit[m].skillPrefab = database.skill[skillId].multipleHit[m].skillPrefab;
			GetComponent<AttackTrigger>().shortcuts[slot].skill.multipleHit[m].skillAnimationTrigger = database.skill[skillId].multipleHit[m].skillAnimationTrigger;

			GetComponent<AttackTrigger>().shortcuts[slot].skill.multipleHit[m].castTime = database.skill[skillId].multipleHit[m].castTime;
			GetComponent<AttackTrigger>().shortcuts[slot].skill.multipleHit[m].skillDelay = database.skill[skillId].multipleHit[m].skillDelay;
			
			GetComponent<AttackTrigger>().shortcuts[slot].skill.multipleHit[m].soundEffect = database.skill[skillId].multipleHit[m].soundEffect;
		}
		
		CheckSameSkill(GetComponent<AttackTrigger>().shortcuts[slot].id , slot);
	}
	
	public void AssignAllSkill(){
		AttackTrigger atk = GetComponent<AttackTrigger>();
		int n = 0;
		while(n < GetComponent<AttackTrigger>().shortcuts.Length){
			if(GetComponent<AttackTrigger>().shortcuts[n].type == AttackTrigger.ShortcutType.Skill){
				GetComponent<AttackTrigger>().shortcuts[n].skill.manaCost = database.skill[atk.shortcuts[n].id].manaCost;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillPrefab = database.skill[atk.shortcuts[n].id].skillPrefab;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillAnimationTrigger = database.skill[atk.shortcuts[n].id].skillAnimationTrigger;
				
				GetComponent<AttackTrigger>().shortcuts[n].skill.icon = database.skill[atk.shortcuts[n].id].icon;
				GetComponent<AttackTrigger>().shortcuts[n].skill.sendMsg = database.skill[atk.shortcuts[n].id].sendMsg;
				GetComponent<AttackTrigger>().shortcuts[n].skill.castEffect = database.skill[atk.shortcuts[n].id].castEffect;
				
				GetComponent<AttackTrigger>().shortcuts[n].skill.castTime = database.skill[atk.shortcuts[n].id].castTime;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillDelay = database.skill[atk.shortcuts[n].id].skillDelay;
				GetComponent<AttackTrigger>().shortcuts[n].skill.whileAttack = database.skill[atk.shortcuts[n].id].whileAttack;
				GetComponent<AttackTrigger>().shortcuts[n].skill.coolDown = database.skill[atk.shortcuts[n].id].coolDown;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillSpawn = database.skill[atk.shortcuts[n].id].skillSpawn;
				
				GetComponent<AttackTrigger>().shortcuts[n].skill.requireWeapon = database.skill[atk.shortcuts[n].id].requireWeapon;
				GetComponent<AttackTrigger>().shortcuts[n].skill.requireWeaponType = database.skill[atk.shortcuts[n].id].requireWeaponType;
				
				GetComponent<AttackTrigger>().shortcuts[n].skill.soundEffect = database.skill[atk.shortcuts[n].id].soundEffect;
				
				int mh = database.skill[atk.shortcuts[n].id].multipleHit.Length;
				GetComponent<AttackTrigger>().shortcuts[n].skill.multipleHit = new SkillAdditionHit[mh];
				for(int m = 0; m < mh; m++){
					GetComponent<AttackTrigger>().shortcuts[n].skill.multipleHit[m] = new SkillAdditionHit();
					
					GetComponent<AttackTrigger>().shortcuts[n].skill.multipleHit[m].skillPrefab = database.skill[atk.shortcuts[n].id].multipleHit[m].skillPrefab;
					GetComponent<AttackTrigger>().shortcuts[n].skill.multipleHit[m].skillAnimationTrigger = database.skill[atk.shortcuts[n].id].multipleHit[m].skillAnimationTrigger;
					
					GetComponent<AttackTrigger>().shortcuts[n].skill.multipleHit[m].castTime = database.skill[atk.shortcuts[n].id].multipleHit[m].castTime;
					GetComponent<AttackTrigger>().shortcuts[n].skill.multipleHit[m].skillDelay = database.skill[atk.shortcuts[n].id].multipleHit[m].skillDelay;
					
					GetComponent<AttackTrigger>().shortcuts[n].skill.multipleHit[m].soundEffect = database.skill[atk.shortcuts[n].id].multipleHit[m].soundEffect;
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
		while(n < GetComponent<AttackTrigger>().shortcuts.Length){
			if(GetComponent<AttackTrigger>().shortcuts[n].id == id && n != slot){
				GetComponent<AttackTrigger>().shortcuts[n].skill.manaCost = 0;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillPrefab = null;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillAnimationTrigger = database.skill[0].skillAnimationTrigger;

				GetComponent<AttackTrigger>().shortcuts[n].skill.icon = null;
				GetComponent<AttackTrigger>().shortcuts[n].skill.sendMsg = "";
				GetComponent<AttackTrigger>().shortcuts[n].skill.castEffect = null;
				
				GetComponent<AttackTrigger>().shortcuts[n].skill.castTime = 0;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillDelay = 0;
				GetComponent<AttackTrigger>().shortcuts[n].skill.whileAttack = database.skill[0].whileAttack;
				GetComponent<AttackTrigger>().shortcuts[n].skill.coolDown = 0;
				GetComponent<AttackTrigger>().shortcuts[n].skill.skillSpawn = database.skill[0].skillSpawn;
				
				GetComponent<AttackTrigger>().shortcuts[n].skill.requireWeapon = false;
				GetComponent<AttackTrigger>().shortcuts[n].skill.requireWeaponType = 0;
				
				GetComponent<AttackTrigger>().shortcuts[n].skill.soundEffect = null;
				
				if(GetComponent<AttackTrigger>().shortcuts[n].onCoolDown > 0){
					GetComponent<AttackTrigger>().shortcuts[slot].onCoolDown = GetComponent<AttackTrigger>().shortcuts[n].onCoolDown;
				}
				GetComponent<AttackTrigger>().shortcuts[n].onCoolDown = 0;
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
