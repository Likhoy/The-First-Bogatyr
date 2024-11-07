using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider2D))]
[AddComponentMenu("2D Action-RPG Kit/Create Pickup Item")]

public class AddItem : MonoBehaviour {
	public int itemID = 1;
	public int itemQuantity = 1;
	private string textPopup = "";

	public ItType itemType = ItType.Usable; 
	
	public float duration = 30.0f;
	private Transform master;
	
	public Transform popup;
	
	void Start(){
		master = transform.root;
		GetComponent<Collider2D>().isTrigger = true;
		if(duration > 0){
			Destroy(master.gameObject, duration);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other){
		//Pick up Item
		if(other.gameObject.tag == "Player"){
			AddItemToPlayer(other.gameObject);
		}
	}
	
	void AddItemToPlayer(GameObject other){
		bool full = false;
		ItemData db = other.GetComponent<Inventory>().database;
		if(itemType == ItType.Usable){
			full = other.GetComponent<Inventory>().AddItem(itemID , itemQuantity);
			textPopup = db.usableItem[itemID].itemName + " x" + itemQuantity.ToString();
		}else{
			full = other.GetComponent<Inventory>().AddEquipment(itemID);
			textPopup = db.equipment[itemID].itemName;
		}
		
		if(!full){
			master = transform.root;
			
			if(popup && textPopup != ""){
				Transform pop = Instantiate(popup, transform.position , transform.rotation) as Transform;
				pop.GetComponent<DamagePopup>().damage = textPopup;
			}
			Destroy(master.gameObject);
		}else{
			if(popup){
				Transform pop = Instantiate(popup, transform.position , transform.rotation) as Transform;
				pop.GetComponent<DamagePopup>().damage = "Inventory Full";
			}
		}
	}
}

public enum ItType {
	Usable = 0,
	Equipment = 1,
}
