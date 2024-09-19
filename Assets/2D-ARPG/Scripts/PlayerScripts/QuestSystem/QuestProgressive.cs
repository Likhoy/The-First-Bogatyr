using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestProgressive : MonoBehaviour {
	public int questId = 1;
	private GameObject player;
	
	public enum progressType{
		Auto = 0,
		Trigger = 1,
		None = 2
	}
	
	public progressType type = progressType.Auto;
	
	void Start(){
		if(type == progressType.Auto){
			player = GameObject.FindWithTag("Player");
			if(!player){
				return;
			}
			//Increase the progress of the Quest ID
			//The Function will automatic check If player have this quest(ID) in the Quest Slot or not.
			QuestStat qstat = player.GetComponent<QuestStat>();
			if(qstat){
				player.GetComponent<QuestStat>().Progress(questId);
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player" && type == progressType.Trigger){
			//Increase the progress of the Quest ID
			//The Function will automatic check If player have this quest(ID) in the Quest Slot or not.
			QuestStat qstat = other.GetComponent<QuestStat>();
			if(qstat){
				bool c = other.GetComponent<QuestStat>().Progress(questId);
				if(c){
					Destroy(gameObject);
				}
			}
		}
	}

	public void AddProgress(){
		if(GlobalStatus.mainPlayer){
			GlobalStatus.mainPlayer.GetComponent<QuestStat>().Progress(questId);
		}
	}
}
