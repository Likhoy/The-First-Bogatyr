using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainExp : MonoBehaviour {
	public int expGain = 20;
	private GameObject player;

	void Start(){
		player = GameObject.FindWithTag("Player");
		if(!player){
			return;
		}
		player.GetComponent<Status>().GainEXP(expGain);
	}
}
