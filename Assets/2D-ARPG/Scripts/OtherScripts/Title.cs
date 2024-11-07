using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
	public GameObject[] playerPrefab = new GameObject[1];
	public int playerSelect = 0;
	public string goToScene = "Camp";
	public string spawnPointName = "PlayerSpawnPoint";

	private int saveSlot = 0;
	private string charName = "Irene";
	private int mode = 0;

	public GameObject[] destroyObjWhenStart;
	public Text[] saveSlotText = new Text[3];

	public GameObject menuPanel;
	public GameObject loadGamePanel;
	public GameObject overwritePanel;
	[Tooltip("You can set it to null, if you don't want player to edit character' name")]
	public GameObject inputNamePanel;

	// Use this for initialization
	void Start(){
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		//Reset All Static variable in Evene Maker System
		for(int a = 0; a < EventSetting.globalBoolean.Length; a++){
			EventSetting.globalBoolean[a] = false;
		}
		for(int a = 0; a < EventSetting.globalInt.Length; a++){
			EventSetting.globalInt[a] = 0;
		}
		if(inputNamePanel){
			inputNamePanel.SetActive(false);
		}
	}
	
	void UpdateSaveData(){
		for(int a = 0; a < saveSlotText.Length; a++){
			if(PlayerPrefs.GetInt("PreviousSave" + a.ToString()) > 0) {
				saveSlotText[a].text = PlayerPrefs.GetString("Name" + a.ToString ()) + "\n" + "Level " + PlayerPrefs.GetInt ("PlayerLevel" + a.ToString ()).ToString ();
			}
		}
	}

	public void StartGameButton(){
		mode = 0;
		menuPanel.SetActive(false);
		loadGamePanel.SetActive(true);
		UpdateSaveData();
	}

	public void QuitGame(){
		Application.Quit();
	}
	
	public void SetPlayerName(string val){
		charName = val;
	}

	public void LoadGameButton(){
		mode = 1;
		menuPanel.SetActive(false);
		loadGamePanel.SetActive(true);
		UpdateSaveData();
	}
	
	public void LoadGame(int id){
		UpdateSaveData();
		if(mode == 0){
			if(PlayerPrefs.GetInt("PreviousSave" + id.ToString()) > 0){
				saveSlot = id;
				menuPanel.SetActive(false);
				loadGamePanel.SetActive(false);
				overwritePanel.SetActive(true);
			}else{
				saveSlot = id;
				menuPanel.SetActive(false);
				loadGamePanel.SetActive(false);
				if(inputNamePanel){
					inputNamePanel.SetActive(true);
				}else{
					NewGame();
				}
			}
		}
		if(mode == 1){
			if(PlayerPrefs.GetInt("PreviousSave" + id.ToString()) > 0){
				saveSlot = id;
				LoadData();
			}
		}
	}
	
	public void ConfirmOverwrite(){
		menuPanel.SetActive(false);
		loadGamePanel.SetActive(false);
		overwritePanel.SetActive(false);
		if(inputNamePanel){
			inputNamePanel.SetActive(true);
		}else{
			NewGame();
		}
	}
	
	public void NewGame(){
		GlobalStatus.saveSlot = saveSlot;
		PlayerPrefs.SetInt("Loadgame", 0);
		for(int a = 0; a < destroyObjWhenStart.Length; a++){
			Destroy(destroyObjWhenStart[a]);
		}
		GameObject pl = Instantiate(playerPrefab[playerSelect] , transform.position , transform.rotation) as GameObject;
		pl.GetComponent<Status>().spawnPointName = spawnPointName;
		pl.GetComponent<Status>().characterName = charName;
		GlobalStatus.characterId = pl.GetComponent<Status>().characterId;
		SceneManager.LoadScene(goToScene, LoadSceneMode.Single);
	}
	
	void LoadData(){
		GlobalStatus.saveSlot = saveSlot;
		SpawnPlayer.onLoadGame = true;
		for(int a = 0; a < destroyObjWhenStart.Length; a++){
			Destroy(destroyObjWhenStart[a]);
		}
		PlayerPrefs.SetInt("Loadgame", 10);
		int playerId = PlayerPrefs.GetInt("PlayerID" +saveSlot.ToString());
		GlobalStatus.characterId = playerId;
		GameObject pl = Instantiate(playerPrefab[playerId] , transform.position , transform.rotation) as GameObject;

		pl.GetComponent<SaveLoad>().LoadDataOnly();
		GlobalStatus.LoadPlayerStatus(pl);
		pl.transform.position = GlobalStatus.savePosition;
		SceneManager.LoadScene(GlobalStatus.savePointMap , LoadSceneMode.Single);
	}

	public void RemoveFromSaveData(int slot){
		PlayerPrefs.SetInt("PreviousSave" +slot.ToString(), 0);
	}

}
