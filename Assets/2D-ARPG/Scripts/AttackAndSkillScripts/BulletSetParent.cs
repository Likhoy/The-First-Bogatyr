using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletStatus))]
public class BulletSetParent : MonoBehaviour {
	public float duration = 1.0f;
	public bool penetrate = false;
	private GameObject hitEffect;
	
	public bool setRotation = false;
	public Vector3 rotationSetTo = Vector3.zero;
	private Transform shooter;
	// Use this for initialization
	void Start(){
		hitEffect = GetComponent<BulletStatus>().hitEffect;
		//Set this object parent of the Shooter GameObject from BulletStatus
		if(setRotation){
			transform.eulerAngles = rotationSetTo;
		}
		shooter = GetComponent<BulletStatus>().shooter.transform;
		//transform.parent = shooter;
		//transform.position = shooter.position;
		Destroy(gameObject, duration);
	}

	void Update(){
		if(shooter){
			transform.position = shooter.position;
		}
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Wall"){
			if(hitEffect && !penetrate){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(!penetrate){
				//Destroy this object if it not Penetrate
				Destroy (gameObject);
			}
		}
	}
}
