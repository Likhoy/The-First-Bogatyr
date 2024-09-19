using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletMove))]
public class HomingBullet : MonoBehaviour {
	public float firstDelay = 0.3f;
	public float radius = 20.0f;
	public float rotateSpeed = 7.5f;
	private Transform lockTarget;
	public bool lock1Target = true;
	private bool onLooking = false;
	private string shooterTag = "Player";
	private bool chasing = false;
	
	void Start(){
		shooterTag = GetComponent<BulletStatus>().shooterTag;
		if(firstDelay > 0){
			StartCoroutine(DelayChase());
		}else{
			chasing = true;
		}
	}
	
	IEnumerator DelayChase(){
		yield return new WaitForSeconds(firstDelay);
		chasing = true;
	}
	
	void Update(){
		if(!chasing){
			return;
		}
		if(lockTarget){
			Vector3 vectorToTarget = lockTarget.position - transform.position;
			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
		}
		if(onLooking){
			return;
		}
		FindClosestEnemy();
	}

	// Find the closest enemy 
	void FindClosestEnemy(){
		// Find all game objects with tag Enemy
		float distance = Mathf.Infinity; 
		
		Collider2D[] objectsAroundMe = Physics2D.OverlapCircleAll(transform.position , radius);
		foreach(Collider2D obj in objectsAroundMe){
			if(shooterTag == "Player" && obj.CompareTag("Enemy")){
				Vector3 diff = (obj.transform.position - transform.position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					//------------
					lockTarget = obj.transform;
					if(lock1Target){
						onLooking = true;
					}
					distance = curDistance;
				} 
			}
			if(shooterTag == "Enemy" && obj.CompareTag("Player") || shooterTag == "Enemy" && obj.CompareTag("Ally") || shooterTag == "Enemy2" && obj.CompareTag("Player")){
				Vector3 diff = (obj.transform.position - transform.position); 
				float curDistance = diff.sqrMagnitude; 
				if(curDistance < distance) { 
					//------------
					lockTarget = obj.transform;
					if(lock1Target){
						onLooking = true;
					}
					distance = curDistance;
				} 
			}
		}
		
	}
}
