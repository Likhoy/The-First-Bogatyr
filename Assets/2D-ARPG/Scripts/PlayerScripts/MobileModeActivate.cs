using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileModeActivate : MonoBehaviour {
	public AttackTrigger player;
	public bool aimAtMouse = false;

	[System.Serializable]
	public class CanvasObj{
		public GameObject activatorButton;
		public Text activatorText;
	}
	public CanvasObj canvasElement;

	void Start(){
		if(!player){
			player = GlobalStatus.mainPlayer.GetComponent<AttackTrigger>();
		}
		player.mobileMode = true;
		player.aimAtMouse = aimAtMouse;

		if(canvasElement.activatorButton){
			player.GetComponent<AttackTrigger>().canvasElement.activatorButton = canvasElement.activatorButton;
		}
		if(canvasElement.activatorText){
			player.GetComponent<AttackTrigger>().canvasElement.activatorText = canvasElement.activatorText;
		}
	}

}
