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
	public SkillSlotsTrigger skillSlotsTrigger;

	private SkillData db;
	private int skillId = 0;

    private bool isDragging = false;

    void Start(){
		SettingUp();
	}

	public void SettingUp(){
		if(!skillTree){
			skillTree = transform.root.GetComponent<SkillTreeUi>();
		}
		if (!skillSlotsTrigger)
		{
			skillSlotsTrigger = skillTree.skillSlotsTrigger;
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
        if (skillTree.tooltip && skillTree.tooltip.activeSelf == true)
        {
            Vector2 tooltipPos = Input.mousePosition;
            tooltipPos.x += 7;
            skillTree.tooltip.transform.position = tooltipPos;
        }
    }

    public void OnDrag()
    {
        if (!skillTree.skillSlots[buttonId].learned) return;
        isDragging = true;
        
		skillTree.skillSlotsTrigger.PickupForShortcut(skillId);
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
}
