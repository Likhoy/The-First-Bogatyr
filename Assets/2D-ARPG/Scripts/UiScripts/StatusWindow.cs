using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour {
	public GameObject player;
	public TMP_Text charName;
	public TMP_Text lv;
	public TMP_Text atk;
	public TMP_Text def;
	public TMP_Text matk;
	public TMP_Text mdef;
	//public TMP_Text exp;
	//public TMP_Text nextLv;
	public TMP_Text stPoint;
	
	//public TMP_Text totalAtk;
	//public TMP_Text totalDef;
	//public TMP_Text totalMatk;
	//public TMP_Text totalMdef;
	
	public Button atkUpButton;
	public Button defUpButton;
	public Button matkUpButton;
	public Button mdefUpButton;
	
	void Start(){
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
	}
	
	void Update(){
		if(!player){
			Destroy(gameObject);
			return;
		}
		Status stat = player.GetComponent<Status>();
		if(charName){
			charName.text = stat.characterName.ToString();
		}
		if(lv){
			lv.text = stat.level.ToString();
		}
		if(atk){
			atk.text = stat.atk.ToString() + "(" + stat.totalStat.atk.ToString() + ")";
		}
		if(def){
			def.text = stat.def.ToString() + "(" + stat.totalStat.def.ToString() + ")";
		}
		if(matk){
			matk.text = stat.matk.ToString() + "(" + stat.totalStat.matk.ToString() + ")";
		}
		if(mdef){
			mdef.text = stat.mdef.ToString() + "(" + stat.totalStat.mdef.ToString() + ")";
		}
		
		//if(exp){
		//	exp.text = stat.exp.ToString() + "/" + stat.maxExp.ToString();
		//}
		//if(nextLv){
		//	nextLv.text = stat.maxExp.ToString();
		//}
		if(stPoint){
			stPoint.text = stat.statusPoint.ToString();
		}
		
		//if(totalAtk){
		//	totalAtk.text = "(" + stat.totalStat.atk.ToString() + ")";
		//}
		//if(totalDef){
		//	totalDef.text = "(" + stat.totalStat.def.ToString() + ")";
		//}
		//if(totalMatk){
		//	totalMatk.text = "(" + stat.totalStat.matk.ToString() + ")";
		//}
		//if(totalMdef){
		//	totalMdef.text = "(" + stat.totalStat.mdef.ToString() + ")";
		//}
		
		if(stat.statusPoint > 0){
			if(atkUpButton)
				atkUpButton.gameObject.SetActive(true);
			if(defUpButton)
				defUpButton.gameObject.SetActive(true);
			if(matkUpButton)
				matkUpButton.gameObject.SetActive(true);
			if(mdefUpButton)
				mdefUpButton.gameObject.SetActive(true);
		}else{
			if(atkUpButton)
				atkUpButton.gameObject.SetActive(false);
			if(defUpButton)
				defUpButton.gameObject.SetActive(false);
			if(matkUpButton)
				matkUpButton.gameObject.SetActive(false);
			if(mdefUpButton)
				mdefUpButton.gameObject.SetActive(false);
		}
		
	}
	
	public void UpgradeStatus(int statusId){
		//0 = Atk , 1 = Def , 2 = Matk , 3 = Mdef
		if(!player){
			return;
		}
		Status stat = player.GetComponent<Status>();
		if(statusId == 0 && stat.statusPoint > 0){
			stat.atk += 1;
			stat.statusPoint -= 1;
			stat.CalculateStatus();
		}
		if(statusId == 1 && stat.statusPoint > 0){
			stat.def += 1;
			stat.maxHealth += 5;
			stat.statusPoint -= 1;
			stat.CalculateStatus();
		}
		if(statusId == 2 && stat.statusPoint > 0){
			stat.matk += 1;
			stat.maxMana += 3;
			stat.statusPoint -= 1;
			stat.CalculateStatus();
		}
		if(statusId == 3 && stat.statusPoint > 0){
			stat.mdef += 1;
			stat.statusPoint -= 1;
			stat.CalculateStatus();
		}
	}
	
	public void CloseMenu(){
		Time.timeScale = 1.0f;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        GlobalStatus.menuOn = false;
        GlobalStatus.freezePlayer = false;
        gameObject.SetActive(false);
	}
}
