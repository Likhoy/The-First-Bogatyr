using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestClientEV : MonoBehaviour{
	public int questId = 1;
	public GameObject questData;
	[HideInInspector]
	public bool enter = false;
	[HideInInspector]
	public int s = 0;
	
	private GameObject player;

	public EventActivator talkingEvent;
	public EventActivator ongoingQuestEvent;
	public EventActivator finishQuestEvent;
	public EventActivator alreadyFinishQuestEvent;
	public EventActivator questFullEvent;

	private bool acceptQuest = false;
	public bool trigger = false;
	public string showText = "";
	private bool thisActive = false;
	private bool questFinish = false;
	public string sendMsgWhenTakeQuest = "";
	public string sendMsgWhenQuestComplete = "";
	public bool repeatable = false;
	
	void Update(){
		if(questFullEvent && questFullEvent.eventRunning){
			return;
		}
		if(Input.GetKeyDown("e") && enter && thisActive){
			SetDialogue();
		}
	}
	
	public void SetDialogue(){
		if(!player){
			player = GameObject.FindWithTag("Player");
		}

		int ongoing = player.GetComponent<QuestStat>().CheckQuestProgress(questId);
		int finish = questData.GetComponent<QuestData>().questData[questId].finishProgress;
		int qprogress = player.GetComponent<QuestStat>().questProgress[questId];
		if(qprogress >= finish + 9){
			if(finishQuestEvent.runEvent > 0 || finishQuestEvent.eventRunning){
				return;
			}
			alreadyFinishQuestEvent.player = player;
			alreadyFinishQuestEvent.ActivateEvent();
			print("Already Clear");
			return;
		}
		if(acceptQuest){
			if(ongoing >= finish){ //Quest Complete
				finishQuestEvent.player = player;
				finishQuestEvent.ActivateEvent();
				FinishQuest();
			}else{
				//Ongoing
				if(talkingEvent.runEvent > 0 || talkingEvent.eventRunning){
					questFullEvent.player = player;
					questFullEvent.ActivateEvent();
					return;
				}
				ongoingQuestEvent.player = player;
				ongoingQuestEvent.ActivateEvent();
			}
		}else{
			int ll = player.GetComponent<QuestStat>().questSlot.Length;
			if(questFullEvent && player.GetComponent<QuestStat>().questSlot[ll - 1] > 0){
				questFullEvent.player = player;
				questFullEvent.ActivateEvent();
				return;
			}
			//Before Take the quest
			talkingEvent.player = player;
			talkingEvent.ActivateEvent();
			TakeQuest();
		}
	}
	
	public void TakeQuest(){
		//StartCoroutine(AcceptQuest());
		AcceptQuest();
		CloseTalk();	
	}
	
	public void FinishQuest(){
		questData.GetComponent<QuestData>().QuestClear(questId , player);
		player.GetComponent<QuestStat>().Clear(questId);
		print("Clear");
		questFinish = true;
		if(sendMsgWhenQuestComplete != ""){
			SendMessage(sendMsgWhenQuestComplete);
		}
		CloseTalk();
		if(repeatable){
			player.GetComponent<QuestStat>().questProgress[questId] = 0;
			questFinish = false;
		}
	}
	
	public void AcceptQuest(){
		bool full = player.GetComponent<QuestStat>().AddQuest(questId);
		if(full){
			//Quest Full
			/*if(questFullEvent){
				questFullEvent.player = player;
				questFullEvent.ActivateEvent();
			}*/
		}else{
			acceptQuest = player.GetComponent<QuestStat>().CheckQuestSlot(questId);
			if(sendMsgWhenTakeQuest != ""){
				SendMessage(sendMsgWhenTakeQuest);
			}
		}
	}
	
	public void CheckQuestCondition(){
		QuestData quest = questData.GetComponent<QuestData>();
		int progress = player.GetComponent<QuestStat>().CheckQuestProgress(questId);
		
		if(progress >= quest.questData[questId].finishProgress){
			//Quest Clear
			quest.QuestClear(questId , player);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if(!trigger){
			return;
		}
		if(other.tag == "Player"){
			s = 0;
			player = other.gameObject;
			acceptQuest = player.GetComponent<QuestStat>().CheckQuestSlot(questId);
			enter = true;
			thisActive = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other){
		if(!trigger){
			return;
		}
		if(other.tag == "Player"){
			s = 0;
			enter = false;
			CloseTalk();
		}
		thisActive = false;
	}
	
	void CloseTalk(){
		//Time.timeScale = 1.0f;
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		s = 0;
	}
	
	public bool ActivateQuest(GameObject p){
		player = p;
		acceptQuest = player.GetComponent<QuestStat>().CheckQuestSlot(questId);
		thisActive = false;
		trigger = false;
		SetDialogue();
		return questFinish;
	}
}
