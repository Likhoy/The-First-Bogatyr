using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentPlayerUI : MonoBehaviour {
	public AttackTrigger player;

	void Start(){
		if(!player){
			player = GlobalStatus.mainPlayer.GetComponent<AttackTrigger>();
		}
		transform.SetParent(null);
		DontDestroyOnLoad(transform.gameObject);
	}
	
	void Update(){
		if(!player){
			Destroy(gameObject);
		}
	}
}
