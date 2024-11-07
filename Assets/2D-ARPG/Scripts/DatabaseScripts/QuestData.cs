using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour {
	public GameObject itemData;
	[System.Serializable]
	public class Quest {
		public string questName = "";
		public string description;
		public int finishProgress = 5;
		public int rewardCash = 100;
		public int rewardExp = 100;
		public int[] rewardItemID;
		public int[] rewardEquipmentID;
		public bool showProgress = true;
		public bool cantCancel = false;
	}
	
	public Quest[] questData = new Quest[3];
	
	public void QuestClear(int id , GameObject player){
		//Get Rewards
		player.GetComponent<Inventory>().cash += questData[id].rewardCash; //Add Cash
		player.GetComponent<Status>().GainEXP(questData[id].rewardExp); //Get EXP
		int i = 0;
		if(questData[id].rewardItemID.Length > 0){	//Add Items
			i = 0;
			while(i < questData[id].rewardItemID.Length){
				player.GetComponent<Inventory>().AddItem(questData[id].rewardItemID[i] , 1);
				i++;
			}
		}
		
		if(questData[id].rewardEquipmentID.Length > 0){	//Add Equipments
			i = 0;
			while(i < questData[id].rewardEquipmentID.Length){
				player.GetComponent<Inventory>().AddEquipment(questData[id].rewardEquipmentID[i]);
				i++;
			}
		}
	}
}
