using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletStatus))]
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("2D Action-RPG Kit/Create Bullet")]

public class BulletMove : MonoBehaviour {
	public float speed = 20;
	public Vector3 relativeDirection = Vector3.right;
	public float duration = 1.0f;
	public string shooterTag = "Player";
	public GameObject hitEffect;
	public bool passthroughWall = false;
	//public float fwdPlusAfterSpawn = 0;
	
	void Start(){
		hitEffect = GetComponent<BulletStatus>().hitEffect;
		GetComponent<Rigidbody2D>().gravityScale = 0;
		if(GetComponent<Collider2D>()){
			GetComponent<Collider2D>().isTrigger = true;
		}
		Destroy(gameObject, duration);
		/*if(fwdPlusAfterSpawn != 0 ){
			Vector3 absoluteDirection = transform.rotation * relativeDirection;
			transform.position += absoluteDirection * fwdPlusAfterSpawn;
		}*/
	}
	
	void Update(){
		//Vector3 absoluteDirection = transform.rotation * relativeDirection;
		//transform.position += absoluteDirection * speed* Time.deltaTime;

		Vector3 dir = transform.TransformDirection(Vector3.right);
		GetComponent<Rigidbody2D>().velocity = dir * speed;
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Wall" && !passthroughWall){
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(GetComponent<BulletStatus>().bombHitSetting.enable){
				GetComponent<BulletStatus>().ExplosionDamage();
			}
			Destroy(gameObject);
		}
	}
}
