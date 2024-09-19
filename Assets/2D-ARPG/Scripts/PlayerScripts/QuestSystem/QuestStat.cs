using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStat : MonoBehaviour {
	public QuestData questDataBase;
	
	public int[] questProgress = new int[20];
	public int[] questSlot = new int[5];
	
	void Start(){
		// If Array Length of questProgress Variable < QuestData.Length
		if(questProgress .Length < questDataBase.questData.Length){
			questProgress = new int[questDataBase.questData.Length];
		}
	}

	public bool AddQuest(int id){
		bool full = false;
		bool geta = false;
		
		int pt= 0;
		while(pt < questSlot.Length && !geta){
			if(questSlot[pt] == id){
				print("You Already Accept this Quest");
				geta = true;
			}else if(questSlot[pt] == 0){
				questSlot[pt] = id;
				geta = true;
				if(GetComponent<UiMaster>()){
					GetComponent<UiMaster>().ShowQuestWarning();
				}
			}else{
				pt++;
				if(pt >= questSlot.Length){
					full = true;
					print("Full");
				}
			}
		}
		return full;
	}
	
	public void SortQuest(){
		int pt= 0;
		int nextp= 0;
		bool  clearr = false;
		while(pt < questSlot.Length){
			if(questSlot[pt] == 0){
				nextp = pt + 1;
				while(nextp < questSlot.Length && !clearr){
					if(questSlot[nextp] > 0){
						//Fine Next Slot and Set
						questSlot[pt] = questSlot[nextp];
						questSlot[nextp] = 0;
						clearr = true;
					}else{
						nextp++;
					}
				}
				//Continue New Loop
				clearr = false;
				pt++;
			}else{
				pt++;
			}
		}
	}
	
	public bool Progress(int id){
		bool haveQuest = false;
		//Check for You have a quest ID match to one of Quest Slot
		for(int n= 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				// Check If The Progress of this quest < Finish Progress then increase 1 of Quest Progress Variable
				if(questProgress[id] < questDataBase.questData[questSlot[n]].finishProgress){
					questProgress[id] += 1;
					haveQuest = true;
					if(GetComponent<UiMaster>() && questProgress[id] >= questDataBase.questData[questSlot[n]].finishProgress){
						GetComponent<UiMaster>().ShowQuestWarning();
					}
				}
				print("Quest Slot =" + n);
			}
		}
		return haveQuest;	
	}
	//-----------------------------------------------
	
	public bool CheckQuestSlot(int id){
		//Check for You have a quest ID match to one of Quest Slot
		bool exist = false;
		for(int n= 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				//You Have this quest in the slot
				exist = true;
			}
		}
		return exist;
	}
	
	public int CheckQuestProgress(int id){
		//Check for You have a quest ID match to one of Quest Slot
		int qProgress = 0;
		for(int n= 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				//You Have this quest in the slot
				qProgress = questProgress[id];
			}
		}
		return qProgress;
	}
	
	//---------------------------------------
	
	public void Clear(int id){
		//Check for You have a quest ID match to one of Quest Slot
		for(int n= 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				//QuestData data = questDataBase.GetComponent<QuestData>();
				questProgress[id] += 10;
				questSlot[n] = 0;
				SortQuest();
				print("Quest Slot =" + n);
			}
		}
	}
}
