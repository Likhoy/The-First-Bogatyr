using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckQuestItem : MonoBehaviour {
	public int questId = 1;
	public int itemId = 1;
	public int quantityNeeded = 1;
	public ItType itemType = ItType.Usable;
	public bool removeItem = true;
	
	public EventActivator notEnoughItemEvent;
	public EventActivator clearEvent;
	
	void CheckItemQuest(){
		bool p = GlobalStatus.mainPlayer.GetComponent<Inventory>().CheckItem(itemId , (int)itemType , quantityNeeded);
		if(p){
			QuestStat q = GlobalStatus.mainPlayer.GetComponent<QuestStat>();
			q.Clear(questId);
			q.questDataBase.GetComponent<QuestData>().QuestClear(questId , GlobalStatus.mainPlayer);
			
			if(removeItem && itemType == ItType.Usable){
				GlobalStatus.mainPlayer.GetComponent<Inventory>().RemoveItem(itemId , quantityNeeded);
			}else if(removeItem && itemType == ItType.Usable){
				GlobalStatus.mainPlayer.GetComponent<Inventory>().RemoveEquipment(itemId);
			}
			
			if(clearEvent){
				clearEvent.ActivateEvent();
			}
		}else if(notEnoughItemEvent){
			notEnoughItemEvent.ActivateEvent();
		}
	}
}
