using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButton : MonoBehaviour {
	public int buttonId = 0;
	public Image iconImageObj;
	private Sprite icon;
	public Sprite iconLocked;
	
	private string skillName = "";
	private string description = "";

	public SkillTreeUi skillTree;

	private SkillData db;
	private int skillId = 0;
	
	void Start(){
		SettingUp();
	}

	public void SettingUp(){
		if(!skillTree){
			skillTree = transform.root.GetComponent<SkillTreeUi>();
		}
		db = skillTree.database;
		if(db){
			skillId = skillTree.skillSlots[buttonId].skillId;
			skillName = db.skill[skillId].skillName;
			description = db.skill[skillId].description;
			icon = db.skill[skillId].icon;
			UpdateIcon();
		}
	}
	
	void Update(){
		if(skillTree.tooltip && skillTree.tooltip.activeSelf == true){
			Vector2 tooltipPos = Input.mousePosition;
			tooltipPos.x += 7;
			skillTree.tooltip.transform.position = tooltipPos;
		}
	}

	public void ButtonClick(){
		if(!skillTree){
			skillTree = transform.root.GetComponent<SkillTreeUi>();
		}
		skillTree.ButtonSkillClick(buttonId);
	}
	
	public void UpdateIcon(){
		iconImageObj.color = Color.white;
		if(skillTree.skillSlots[buttonId].locked){
			iconImageObj.sprite = iconLocked;
			return;
		}else{
			iconImageObj.sprite = icon;
		}
		
		if(!skillTree.skillSlots[buttonId].learned){
			iconImageObj.color = Color.gray;
		}
	}
	
	public void ShowSkillTooltip(){
		if(!skillTree.tooltip){
			return;
		}
		skillTree.tooltipIcon.sprite = icon;
		skillTree.tooltipName.text = skillName;
		
		skillTree.tooltipText.text = description;
		skillTree.mpCostTooltip.text = "MP : " + db.skill[skillId].manaCost.ToString();
		
		skillTree.tooltip.SetActive(true);
	}
	
	public void HideTooltip(){
		if(!skillTree.tooltip){
			return;
		}
		skillTree.tooltip.SetActive(false);
	}

	public void OnDragSkill(){
		if(!GlobalStatus.mainPlayer || !skillTree.skillSlots[buttonId].learned){
			return;
		}
		AttackTrigger atk = GlobalStatus.mainPlayer.GetComponent<AttackTrigger>();
		atk.draggingItemIcon.gameObject.SetActive(true);
		atk.draggingItemIcon.sprite = db.skill[skillId].icon;

		GlobalStatus.mainPlayer.GetComponent<AttackTrigger>().PickupForShortcut(skillId , 3);
	}
	
}
