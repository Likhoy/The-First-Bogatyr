using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
	public GameObject respawnPrefab;
	public Animator animator;
	public string deathAnimationName = "Death";
	public bool reloadScene = true;
	public GameObject eventSystemToDelete;

	void Start(){
		if(animator){
			animator.Play(deathAnimationName);
		}
	}
	
	public void QuitGame(){
		Destroy(Camera.main.gameObject); //Destroy Main Camera
		SceneManager.LoadScene("Title", LoadSceneMode.Single);
	}

	public void RespawnPlayer(){
		Destroy(Camera.main.gameObject); //Destroy Main Camera
        if(eventSystemToDelete) {
			Destroy(eventSystemToDelete);
        }

		GameObject respawn = Instantiate(respawnPrefab, GlobalStatus.savePosition , transform.rotation) as GameObject;
		GlobalStatus.mainPlayer = respawn;
		GlobalStatus.LoadPlayerStatus(respawn);

		respawn.GetComponent<Status>().health = respawn.GetComponent<Status>().maxHealth;
		respawn.GetComponent<Status>().mana = respawn.GetComponent<Status>().maxMana;

		Destroy(gameObject);
	}

}
