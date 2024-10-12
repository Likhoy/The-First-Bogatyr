using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoSetting : MonoBehaviour {
	public Text[] buttonTxt = new Text[4];

	void Start(){
		if(!GlobalStatus.mainPlayer){
			return;
		}
		AttackTrigger atk = GlobalStatus.mainPlayer.GetComponent<AttackTrigger>();
		if(buttonTxt[0]){
			if(atk.aimAtMouse){
				buttonTxt[0].text = "Aim at Mouse [On]";
			}else{
				buttonTxt[0].text = "Aim at Mouse [Off]";
			}
		}

		if(GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>()){
			PlatformerController2D con = GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>();
			if(buttonTxt[1]){
				if(con.canDash){
					buttonTxt[1].text = "Dash [On]";
				}else{
					buttonTxt[1].text = "Dash [Off]";
				}
			}
			if(buttonTxt[2]){
				if(con.canDoubleJump){
					buttonTxt[2].text = "Double Jump [On]";
				}else{
					buttonTxt[2].text = "Double Jump [Off]";
				}
			}
			if(buttonTxt[3]){
				if(con.canAirDash){
					buttonTxt[3].text = "Air Dash [On]";
				}else{
					buttonTxt[3].text = "Air Dash [Off]";
				}
			}
		}
		if(GlobalStatus.mainPlayer.GetComponent<TopdownInputController2D>()){
			TopdownInputController2D con = GlobalStatus.mainPlayer.GetComponent<TopdownInputController2D>();
			if(buttonTxt[1]){
				if(con.canDash){
					buttonTxt[1].text = "Dash [On]";
				}else{
					buttonTxt[1].text = "Dash [Off]";
				}
			}
		}
	}

	public void AimAtMouseOnOff(){
		if(!GlobalStatus.mainPlayer){
			return;
		}
		AttackTrigger atk = GlobalStatus.mainPlayer.GetComponent<AttackTrigger>();
		if(atk.aimAtMouse){
			atk.aimAtMouse = false;
			atk.attackPoint.rotation = GlobalStatus.mainPlayer.transform.rotation;
			atk.attackPoint.GetComponentInChildren<SpriteRenderer>().enabled = false;
			if(buttonTxt[0]){
				buttonTxt[0].text = "Aim at Mouse [Off]";
			}
		}else{
			atk.aimAtMouse = true;
			atk.attackPoint.GetComponentInChildren<SpriteRenderer>().enabled = true;
			if(buttonTxt[0]){
				buttonTxt[0].text = "Aim at Mouse [On]";
			}
		}
	}

	public void DashOnOff(){
		if(!GlobalStatus.mainPlayer){
			return;
		}
		if(GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>()){
			PlatformerController2D con = GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>();
			con.canDash = !con.canDash;
			if(buttonTxt[1]){
				if(con.canDash){
					buttonTxt[1].text = "Dash [On]";
				}else{
					buttonTxt[1].text = "Dash [Off]";
				}
			}
		}
		if(GlobalStatus.mainPlayer.GetComponent<TopdownInputController2D>()){
			TopdownInputController2D con = GlobalStatus.mainPlayer.GetComponent<TopdownInputController2D>();
			con.canDash = !con.canDash;
			if(buttonTxt[1]){
				if(con.canDash){
					buttonTxt[1].text = "Dash [On]";
				}else{
					buttonTxt[1].text = "Dash [Off]";
				}
			}
		}
	}

	public void DoubleJumpOnOff(){
		if(!GlobalStatus.mainPlayer){
			return;
		}
		if(GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>()){
			PlatformerController2D con = GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>();
			con.canDoubleJump = !con.canDoubleJump;
			if(buttonTxt[2]){
				if(con.canDoubleJump){
					buttonTxt[2].text = "Double Jump [On]";
				}else{
					buttonTxt[2].text = "Double Jump [Off]";
				}
			}
		}
	}

	public void AirDashOnOff(){
		if(!GlobalStatus.mainPlayer){
			return;
		}
		if(GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>()){
			PlatformerController2D con = GlobalStatus.mainPlayer.GetComponent<PlatformerController2D>();
			con.canAirDash = !con.canAirDash;
			if(buttonTxt[3]){
				if(con.canAirDash){
					buttonTxt[3].text = "Air Dash [On]";
				}else{
					buttonTxt[3].text = "Air Dash [Off]";
				}
			}
		}
	}
}
