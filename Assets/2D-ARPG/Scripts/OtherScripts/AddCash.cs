using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider2D))]
[AddComponentMenu("2D Action-RPG Kit/Create Pickup Cash")]

public class AddCash : MonoBehaviour {
	public int cashMin = 10;
	public int cashMax = 50;
	public float duration = 30.0f;
	
	private Transform master;
	
	public Transform popup;
	
	void Start(){
		master = transform.root;
		GetComponent<Collider2D>().isTrigger = true;
		if(duration > 0){
			Destroy (gameObject, duration);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other){
		//Pick up Item
		if(other.gameObject.tag == "Player"){
			AddCashToPlayer(other.gameObject);
		}
	}
	
	void AddCashToPlayer(GameObject other){
		int gotCash = Random.Range(cashMin , cashMax);
		other.GetComponent<Inventory>().cash += gotCash;
		master = transform.root;
		
		if(popup){
			Transform pop = Instantiate(popup, transform.position , transform.rotation) as Transform;
			pop.GetComponent<DamagePopup>().damage = "Money " + gotCash.ToString();
		}
		Destroy(master.gameObject);
	}
}
