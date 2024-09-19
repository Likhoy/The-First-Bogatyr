using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	public Image hpBar;
	public Image mpBar;
	public Image expBar;
	public Text hpText;
	public Text mpText;
	public Text lvText;
	public GameObject player;
	
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
		
		int maxHp = stat.totalStat.health;
		float hp = stat.health;
		int maxMp = stat.totalStat.mana;
		float mp = stat.mana;
		int exp = stat.exp;
		float maxExp = stat.maxExp;
		float curHp = hp/maxHp;
		float curMp = mp/maxMp;
		float curExp = exp/maxExp;

		//HP Gauge
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
		
		//MP Gauge
		if(curMp > mpBar.fillAmount){
			mpBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
			if(mpBar.fillAmount > curMp){
				mpBar.fillAmount = curMp;
			}
		}	
		if(curMp < mpBar.fillAmount){
			mpBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
			if(mpBar.fillAmount < curMp){
				mpBar.fillAmount = curMp;
			}
		}
		
		//EXP Gauge
		if(expBar){
			expBar.fillAmount = curExp;
		}
		if(lvText){
			lvText.text = stat.level.ToString();
		}
		if(hpText){
			hpText.text = hp.ToString() + "/" + maxHp.ToString();
		}
		if(mpText){
			mpText.text = mp.ToString() + "/" + maxMp.ToString();
		}
	}
}
