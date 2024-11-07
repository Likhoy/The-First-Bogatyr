using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (BulletStatus))]

public class HealingSkill : MonoBehaviour {

	public int hpRestore = 0;
	public int variance = 15;
	public DamagePopup popup;
	
	public enum buff{
		None = 0,
		Barrier = 1,
		MagicBarrier = 2,
		Brave = 3,
		Faith = 4
	}
	public buff buffs = buff.None;
	public int statusAmount = 0;
	public float statusDuration = 5.5f;
	
	public string shooterTag = "Player";
	public GameObject hitEffect;
	public float effectRadius = 0.0f;
	
	void Start(){
		if(effectRadius > 0){
			ApplyRadiusEffect();
		}else{
			ApplyEffect(GetComponent<BulletStatus>().shooter);
		}
	}
	
	void ApplyEffect(GameObject target){
		if(hpRestore > 0){
			if(variance >= 100){
				variance = 100;
			}
			if(variance <= 1){
				variance = 1;
			}
			int varMin = 100 - variance;
			int varMax = 100 + variance;
			if(GetComponent<BulletStatus>().shooter){
				hpRestore += GetComponent<BulletStatus>().shooter.GetComponent<Status>().totalStat.matk;
			}
			hpRestore = hpRestore * Random.Range(varMin ,varMax) / 100;
			
			target.GetComponent<Status>().Heal(hpRestore , 0);
			//Healing PopUp
			Transform popAmount = Instantiate(popup.transform, target.transform.position , transform.rotation) as Transform;
			popAmount.GetComponent<DamagePopup>().damage = hpRestore.ToString();
		}
		if(hitEffect){
			Instantiate(hitEffect, target.transform.position , hitEffect.transform.rotation);
		}
		//Call Function ApplyBuff in Status Script
		if(buffs != buff.None){
			target.GetComponent<Status>().ApplyBuff((int)buffs ,statusDuration , statusAmount);
		}
		Destroy(gameObject);
	}
	
	void ApplyRadiusEffect(){
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);
		
		for(var i = 0; i < hitColliders.Length; i++){
			if(shooterTag == "Player" && hitColliders[i].tag == "Player" || shooterTag == "Player" && hitColliders[i].tag == "Ally"){	  
				ApplyEffect(hitColliders[i].gameObject);
			}else if(shooterTag == "Enemy" && hitColliders[i].tag == "Enemy"){  	
				ApplyEffect(hitColliders[i].gameObject);
			}
		}
	}
}
