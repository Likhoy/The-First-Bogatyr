using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiMaster : MonoBehaviour {
	public EventSystem eventSystem;
	public HealthBar healthBar;
	public StatusWindow statusWindow;
	public SkillTreeUi skillTree;
	public InventoryUi inventoryWindow;
	public QuestUi questWindow;
    public Bestiary bestiary;
    public GameObject map;

    public GameObject lvUpWarningStatus;
	public GameObject lvUpWarningSkill;
	public GameObject newQuestWarning;

	private QuestLogWindow questLogWindow;

    private void Awake()
    {
		questLogWindow = DialogueManager.Instance.GetComponentInChildren<QuestLogWindow>();
    }

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
		if(inventoryWindow && Input.GetKeyDown(Settings.commandButtons[Command.OpenInventory])){
			OnOffInventoryMenu();
		}
		if(skillTree && Input.GetKeyDown(Settings.commandButtons[Command.OpenSkillTree])){
			OnOffSkillMenu();
		}
		if(bestiary && Input.GetKeyDown(Settings.commandButtons[Command.OpenBestiary])){
			OnOffBestiary();
		}

		if (map && Input.GetKeyDown(Settings.commandButtons[Command.OpenMap])){
			OnOffMap();
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
		if (bestiary)
			bestiary.gameObject.SetActive(false);
		if (map)
			map.SetActive(false);

		if (questLogWindow)
			questLogWindow.Close(); // пока это бессмысленно, потому что quest log не дает открывать что-либо еще
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

    public void OnOffMap()
    {
        if (map.activeSelf == false)
        {
            //Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CloseAllMenu();
            map.SetActive(true);
            GlobalStatus.menuOn = true;
        }
        else
        {
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
	
	public void OnOffBestiary(){
		if(bestiary.gameObject.activeSelf == false){
			//Time.timeScale = 0.0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			bestiary.gameObject.SetActive(true);
			GlobalStatus.menuOn = true;

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
