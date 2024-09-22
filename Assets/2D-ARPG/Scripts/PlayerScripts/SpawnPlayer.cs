using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPlayer : MonoBehaviour{
	public GameObject player;
	public static bool onLoadGame;
	public bool setCheckPoint = true;
	
	void Start(){
		if(GetComponent<SpriteRenderer>()){
			GetComponent<SpriteRenderer>().enabled = false;
		}
		//Check for Current Player in the scene
		GameObject currentPlayer = GameObject.FindWithTag("Player");
		if(currentPlayer){
			// If there are the player in the scene already. Check for the Spawn Point Name
			// If it match then Move Player to the SpawnpointPosition
			string spawnPointName = currentPlayer.GetComponent<Status>().spawnPointName;
			GameObject spawnPoint = GameObject.Find(spawnPointName);
			if(spawnPoint && !onLoadGame){
				currentPlayer.transform.root.position = spawnPoint.transform.position;
				//currentPlayer.transform.root.rotation = spawnPoint.transform.rotation;

				//Set Z Axis to 0
				Vector3 pos = currentPlayer.transform.position;
				pos.z = 0;
				currentPlayer.transform.position = pos;

				if(currentPlayer.GetComponent<AttackTrigger>().minion){
					currentPlayer.GetComponent<AttackTrigger>().minion.position = pos;
				}
			}
			
			if(setCheckPoint){
				GlobalStatus.SavePlayerPosition(currentPlayer);
			}
			
			onLoadGame = false;
			GameObject oldCam = AttackTrigger.mainCam.gameObject;
			if(!oldCam){
				return;
			}
			GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera"); 
			foreach(GameObject cam2 in cam) { 
				if(cam2 != oldCam){
					Destroy(cam2.gameObject);
				}
			}
			// If there are the player in the scene already. We will not spawn the new player.
			return;
		}
		//Spawn Player
		GameObject spawnPlayer = Instantiate(player, transform.position , transform.rotation) as GameObject;

		GlobalStatus.SavePlayerPosition(spawnPlayer);
		
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
	}
}
