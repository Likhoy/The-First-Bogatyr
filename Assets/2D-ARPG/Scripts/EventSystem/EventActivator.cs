using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(EventSetting))]

public class EventActivator : MonoBehaviour{
	public Transform mainModel;
	[HideInInspector]
	public int runEvent = 0;
	public enum StartConditions{
		KeyTrigger,
		Collide,
		TriggerEnter,
		TriggerExit,
		AutoStart,
		None //Call from another script
	}
	public StartConditions startCondition = StartConditions.KeyTrigger;
	public bool lookNpc = false;
	public bool lookAtPlayer = false;
	private bool onLooking = false;
	private bool playerLooking = false;
	public bool freezePlayerDuringEvent = false;
	public bool lockCam = false;
	public bool closePlayerUi = false;
	public string buttonTxt = "Check";

	[HideInInspector]
	public bool eventRunning = false;
	private Transform mainPlayer;

	public static bool onInteracting = false;
	// Use this for initialization
	void Start(){
		if(startCondition == StartConditions.AutoStart){
			if(freezePlayerDuringEvent){
				FreezePlayer();
			}
			GetComponents<EventSetting>()[0].Activate();
		}
		/*if(startCondition == StartConditions.KeyTrigger){
			gameObject.tag = "TriggerEvent";
		}*/
		if(!mainModel){
			mainModel = this.transform;
		}
	}

	void FreezePlayer(){
		//globalFreeze = true;
		GlobalStatus.freezeAll = true;
		onInteracting = true;
		if(lockCam){
			GlobalStatus.freezeCam = true;
		}
		//If you have other Player Controller scripts and want to freeze character
		//or Disable controller during the event you can do your stuffs here.
		//For Example
		//FindPlayer();
		//mainPlayer.GetComponent<YourController>().enabled = false;
	}

	void Update(){
		/*if(onLooking){
			if(!mainPlayer){
				return;
			}
			Vector3 lookPos = mainPlayer.position - transform.position;
			lookPos.y = 0;
			if(lookPos != Vector3.zero){
				Quaternion rot = Quaternion.LookRotation(lookPos);
				mainModel.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 20);
			}
		}*/

		if(lookAtPlayer && onLooking && player){
			Vector3 destinyb = player.transform.position;
			destinyb.y = mainModel.position.y;
			
			Quaternion targetRotation = Quaternion.LookRotation(destinyb - mainModel.position);
			mainModel.transform.rotation = Quaternion.Slerp(mainModel.rotation, targetRotation, 20 * Time.unscaledDeltaTime);
		}

		if(lookNpc && playerLooking && player){
			Vector3 destinya = mainModel.position;
			destinya.y = player.transform.position.y;
			
			Quaternion targetRotation = Quaternion.LookRotation(destinya - player.transform.position);
			player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 20 * Time.unscaledDeltaTime);
		}
	}

	public void ActivateTrigger(){
		if(startCondition == StartConditions.KeyTrigger && !GlobalStatus.freezeAll && Time.timeScale != 0.0f && !GlobalStatus.freezePlayer){
			ActivateEvent();
		}
	}

	IEnumerator LookPlayer(){
		FindPlayer();
		if(mainPlayer){
			onLooking = true;
			yield return new WaitForSeconds(1);
			onLooking = false;

			Vector3 destiny = mainPlayer.transform.position;
			destiny.y = mainModel.position.y;
			mainModel.transform.LookAt(destiny);

		}else{
			yield return new WaitForSeconds(0.01f);
		}
	}

	IEnumerator LookAtMe(){
		playerLooking = true;
		yield return new WaitForSeconds(1);
		playerLooking = false;

		Vector3 destiny = mainModel.position;
		destiny.y = player.transform.position.y;
		player.transform.LookAt(destiny);
	}

	public void ActivateEvent(){
		if(runEvent > 0 || eventRunning){
			return;
		}

		if(lookAtPlayer){
			//StartCoroutine(LookPlayer());
			FindPlayer();
			if(mainPlayer){
				Vector3 delta = mainPlayer.position - mainModel.position; 
				
				if(delta.x >= 0){
					Vector3 rot = mainModel.eulerAngles;
					rot.y = 0;
					mainModel.eulerAngles = rot;
				}else if (delta.x < 0){
					Vector3 rot = mainModel.eulerAngles;
					rot.y = 180;
					mainModel.eulerAngles = rot;
				}
			}
		}
		if(lookNpc){
			//StartCoroutine(LookAtMe());
			FindPlayer();
			if(mainPlayer){
				AttackTrigger atk = mainPlayer.GetComponent<AttackTrigger>();
				Vector3 delta = mainModel.position - mainPlayer.position; 
				
				if(delta.x >= 0 && !atk.facingRight){
					Vector3 rot = mainPlayer.eulerAngles;
					rot.y = 0;
					mainPlayer.eulerAngles = rot;
					atk.facingRight = true;
				}else if (delta.x < 0 && atk.facingRight){
					Vector3 rot = transform.eulerAngles;
					rot.y = 180;
					mainPlayer.eulerAngles = rot;
					atk.facingRight = false;
				}
			}
		}
		if(freezePlayerDuringEvent){
			FreezePlayer();
		}
		eventRunning = true;
		GetComponents<EventSetting>()[0].Activate();

		if(closePlayerUi && GlobalStatus.mainPlayer){
			GlobalStatus.mainPlayer.GetComponent<UiMaster>().CloseAllMenu();
		}
	}

	public void ActivateEventId(int id){
		runEvent = id;
		eventRunning = true;
		if(freezePlayerDuringEvent){
			FreezePlayer();
		}
		GetComponents<EventSetting>()[runEvent].Activate();
	}

	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public bool enter = false;

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player" && startCondition == StartConditions.TriggerEnter){
			ActivateEvent();
		}

		if(other.tag == "Player" && startCondition == StartConditions.KeyTrigger){
			player = other.gameObject;
			enter = true;
			if(player.GetComponent<AttackTrigger>())
				player.GetComponent<AttackTrigger>().GetActivator(this.gameObject , "ActivateTrigger" , buttonTxt);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if(other.tag == "Player" && other.GetComponent<AttackTrigger>() && startCondition == StartConditions.KeyTrigger){
			enter = false;
			if(player.GetComponent<AttackTrigger>())
				player.GetComponent<AttackTrigger>().RemoveActivator(this.gameObject);
		}

		if(other.tag == "Player" && startCondition == StartConditions.TriggerExit){
			ActivateEvent();
		}
	}

	void OnCollisionEnter2D(Collision2D other){
		if(startCondition == StartConditions.Collide){
			ActivateEvent();
		}
	}

	void FindPlayer(){
		if(!mainPlayer){
			mainPlayer = GameObject.FindWithTag("Player").transform;
		}
	}

	public void EndEvent(){
		if(freezePlayerDuringEvent){
			//globalFreeze = false;
			GlobalStatus.freezeAll = false;
			onInteracting = false;
			if(lockCam){
				GlobalStatus.freezeCam = false;
			}
			//If you have other Player Controller scripts and want to unfreeze character
			//or Enable controller during the event you can do your stuffs here.
			//For Example
			//FindPlayer();
			//mainPlayer.GetComponent<YourController>().enabled = true;
		}
		runEvent = 0;
		eventRunning = false;
	}
}
