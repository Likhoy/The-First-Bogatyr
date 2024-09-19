using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour {
	public Image hpBar;
	public Status stat;

	void Start(){
		if(!stat){
			stat = transform.root.GetComponent<Status>();
		}
	}
	
	void Update(){
		transform.rotation = Quaternion.identity;
		int maxHp = stat.maxHealth;
		float hp = stat.health;
		
		float curHp = hp/maxHp;
		if(curHp > hpBar.fillAmount){
			hpBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
			if(hpBar.fillAmount > curHp){
				hpBar.fillAmount = curHp;
			}
		}	
		if(curHp < hpBar.fillAmount){
			hpBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
			if(hpBar.fillAmount < curHp){
				hpBar.fillAmount = curHp;
			}
		}
	}
}
