using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour {
	public ItemData itemDatabase;
	[System.Serializable]
	public class ItemDrop{
		public GameObject itemPrefab;
		[Range (0, 100)]
		public int dropChance = 20;
		[Tooltip("Set to 0 if you don't want to change the Item ID.")]
		public int setId = 0; //Set to 0 if you don't want to change the Item ID.
	}
	public ItemDrop[] itemDropSetting = new ItemDrop[1];
	public float randomPosition = 1.0f;

	void Start(){
		for(int n = 0; n < itemDropSetting.Length ; n++){
			int ran = Random.Range(0 , 100);
			if(ran <= itemDropSetting[n].dropChance){
				Vector3 ranPos = transform.position; //Slightly Random x z position.
				ranPos.x += Random.Range(-randomPosition , randomPosition);
				ranPos.y += Random.Range(0.0f , randomPosition);
				//Drop Item
				GameObject dr = Instantiate(itemDropSetting[n].itemPrefab , ranPos , itemDropSetting[n].itemPrefab.transform.rotation) as GameObject;
				if(itemDropSetting[n].setId > 0){
					dr.GetComponentInChildren<AddItem>().itemID = itemDropSetting[n].setId;
				}
				if(itemDatabase && dr.GetComponent<SpriteRenderer>()){
					if(dr.GetComponentInChildren<AddItem>().itemType == ItType.Usable){
						dr.GetComponent<SpriteRenderer>().sprite = itemDatabase.usableItem[dr.GetComponentInChildren<AddItem>().itemID].icon;
					}else{
						dr.GetComponent<SpriteRenderer>().sprite = itemDatabase.equipment[dr.GetComponentInChildren<AddItem>().itemID].icon;
					}
				}
			}
		}
	}
}
