using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpMpRegen : MonoBehaviour {
	public int hpRegen = 0;
	public int mpRegen = 3;
	public float hpRegenDelay = 3.0f;
	public float mpRegenDelay = 3.0f;
	
	private float hpTime = 0.0f;
	private float mpTime = 0.0f;
	private Status stat;
	
	void Start(){
		stat= GetComponent<Status>();
	}
	
	void Update(){
		if(hpRegen > 0 && stat.health < stat.totalStat.health){
			if(hpTime >= hpRegenDelay){
				HPRecovery();
			}else{
				hpTime += Time.deltaTime;
			}
		}
		//----------------------------------------------------
		if(mpRegen > 0 && stat.mana < stat.totalStat.mana){
			if(mpTime >= mpRegenDelay){
				MPRecovery();
			}else{
				mpTime += Time.deltaTime;
			}
		}
	}
	
	void HPRecovery(){
		int amount = stat.totalStat.health * hpRegen / 100;
		if(amount <= 1){
			amount = 1;
		}
		stat.health += amount;
		hpTime = 0.0f;
		if(stat.health >= stat.totalStat.health){
			stat.health = stat.totalStat.health;
		}
	}
	
	void MPRecovery(){
		int amount = stat.totalStat.mana * mpRegen / 100;
		if(amount <= 1){
			amount = 1;
		}
		stat.mana += amount;
		mpTime = 0.0f;
		if(stat.mana >= stat.totalStat.mana){
			stat.mana = stat.totalStat.mana;
		}
	}
}
