using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiMaster : MonoBehaviour {
	public EventSystem eventSystem;
	public HealthBar healthBar;
	public StatusWindow statusWindow;
	public SkillTreeUi skillTree;
	public InventoryUi inventoryWindow;
	public QuestUi questWindow;

	public GameObject lvUpWarningStatus;
	public GameObject lvUpWarningSkill;
	public GameObject newQuestWarning;

	void Start(){
		if(healthBar){
			healthBar.player = this.gameObject;
		}
		if(statusWindow){
			statusWindow.GetComponent<StatusWindow>().player = this.gameObject;
			statusWindow.gameObject.SetActive(false);
		}
		if(inventoryWindow){
			inventoryWindow.GetComponent<InventoryUi>().player = this.gameObject;
			inventoryWindow.gameObject.SetActive(false);
		}
		if(skillTree){
			skillTree.GetComponent<SkillTreeUi>().player = this.gameObject;
			skillTree.gameObject.SetActive(false);
		}
		if(questWindow){
			questWindow.GetComponent<QuestUi>().player = this.gameObject;
			questWindow.gameObject.SetActive(false);
		}
		DeleteOtherEventSystem();
	}

	public void DeleteOtherEventSystem(){
		if(eventSystem){
			EventSystem[] sceneEventSystem = FindObjectsOfType<EventSystem>();
			if(sceneEventSystem.Length > 0){
				for(int a = 0; a < sceneEventSystem.Length; a++){
					if(sceneEventSystem[a] != eventSystem){
						Destroy(sceneEventSystem[a].gameObject);
					}
				}
			}
		}
	}
	
	void Update(){
		if(GlobalStatus.freezeAll || Time.timeScale == 0){
			return;
		}
		if(statusWindow && Input.GetKeyDown("c")){
			OnOffStatusMenu();
		}
		if(inventoryWindow && Input.GetKeyDown("i")){
			OnOffInventoryMenu();
		}
		if(skillTree && Input.GetKeyDown("k")){
			OnOffSkillMenu();
		}
		if(questWindow && Input.GetKeyDown("q")){
			OnOffQuestMenu();
		}

	}
	
	public void CloseAllMenu(){
		GlobalStatus.menuOn = false;
		if(statusWindow)
			statusWindow.gameObject.SetActive(false);
		if(inventoryWindow)
			inventoryWindow.gameObject.SetActive(false);
		if(skillTree)
			skillTree.gameObject.SetActive(false);
		if(questWindow)
			questWindow.gameObject.SetActive(false);
	}
	
	public void OnOffStatusMenu(){
		if(statusWindow.gameObject.activeSelf == false){
			//Time.timeScale = 0.0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			statusWindow.gameObject.SetActive(true);
			GlobalStatus.menuOn = true;
			if(lvUpWarningStatus){
				lvUpWarningStatus.SetActive(false);
			}
		}else{
			Time.timeScale = 1.0f;
			//Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
			CloseAllMenu();
		}
	}
	
	public void OnOffInventoryMenu(){
		if(inventoryWindow.gameObject.activeSelf == false){
			//Time.timeScale = 0.0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			inventoryWindow.gameObject.SetActive(true);
			GlobalStatus.menuOn = true;
		}else{
			Time.timeScale = 1.0f;
			//Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
			CloseAllMenu();
		}
	}
	
	public void OnOffSkillMenu(){
		if(skillTree.gameObject.activeSelf == false){
			//Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			skillTree.gameObject.SetActive(true);
			skillTree.gameObject.GetComponent<SkillTreeUi>().Start();
			GlobalStatus.menuOn = true;
			if(lvUpWarningSkill){
				lvUpWarningSkill.SetActive(false);
			}
		}else{
			Time.timeScale = 1.0f;
			//Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
			CloseAllMenu();
		}
	}
	
	public void OnOffQuestMenu(){
		if(questWindow.gameObject.activeSelf == false){
			//Time.timeScale = 0.0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			questWindow.gameObject.SetActive(true);
			questWindow.GetComponent<QuestUi>().ResetPage();
			GlobalStatus.menuOn = true;
			if(newQuestWarning){
				newQuestWarning.SetActive(false);
			}
		}else{
			Time.timeScale = 1.0f;
			//Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
			CloseAllMenu();
		}
	}

	public void ShowLevelUpWarning(){
		if(lvUpWarningStatus){
			lvUpWarningStatus.SetActive(true);
		}
		if(lvUpWarningSkill){
			lvUpWarningSkill.SetActive(true);
		}
	}

	public void ShowQuestWarning(){
		if(newQuestWarning){
			newQuestWarning.SetActive(true);
		}
	}
}
