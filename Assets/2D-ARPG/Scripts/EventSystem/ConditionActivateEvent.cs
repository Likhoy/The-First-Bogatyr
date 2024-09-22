using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionActivateEvent : MonoBehaviour {
	public int varId = 1;
	[System.Serializable]
	public class EventSetta{
		public EventActivator events;
		public int progress = 0;
	}
	public EventSetta[] eventStep = new EventSetta[2];
	public bool notAtvIfFreeze = false;
	// Use this for initialization
	void StartEvent(){
		if(GlobalStatus.freezeAll && notAtvIfFreeze){
			return;
		}
		bool done = false;
		int a = eventStep.Length - 1;

		int step = 0;
		while(!done){
			if(EventSetting.globalInt[varId] < eventStep[a].progress){
				a -= 1;
				if(a <= 0){
					done = true;
				}
			}else{
				step = a;
				done = true;
			}
		}
		//------------
		print(step + " : Progress " + EventSetting.globalInt[varId]);
		eventStep[step].events.ActivateEvent();
	}
}
