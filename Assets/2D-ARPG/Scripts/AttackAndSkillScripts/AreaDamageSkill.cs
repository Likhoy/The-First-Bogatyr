using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BulletStatus))]
public class AreaDamageSkill : MonoBehaviour {
	public float radius = 5.0f;
	public float delayPerHit = 1.0f;
	public float duration = 3.1f;
	public bool applyDamageWhenSpawn = true;
	
	private float wait = 0;
	private BulletStatus bl;

	void Start(){
		bl = GetComponent<BulletStatus>();
		if(applyDamageWhenSpawn){
			ApplyDamage();
		}
		if(duration > 0){
			Destroy(gameObject, duration);
		}
	}
	
	void Update (){
		if(wait >= delayPerHit){
			ApplyDamage();
			wait = 0;
		}else{
			wait += Time.deltaTime;
		}
	}
	
	void ApplyDamage(){
		//Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position , radius);
		
		for(int i = 0; i < hitColliders.Length; i++) {
			if(bl.shooterTag == "Enemy" && hitColliders[i].tag == "Player" && hitColliders[i].gameObject != bl.shooter){
				bl.DealDamageOnly(hitColliders[i].transform , true);
			}else if(bl.shooterTag == "Player" && hitColliders[i].tag == "Enemy" && hitColliders[i].gameObject != bl.shooter){  	
				bl.DealDamageOnly(hitColliders[i].transform , false);
			}else if(bl.shooterTag == "Enemy" && hitColliders[i].tag == "Ally" && hitColliders[i].gameObject != bl.shooter){
				bl.DealDamageOnly(hitColliders[i].transform , true);
			}
		}
	}
}
