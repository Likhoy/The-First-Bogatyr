using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSlashRotation : MonoBehaviour {
	public BulletStatus bulletStat;

	void Start(){
		if(bulletStat && bulletStat.shooter){
			Vector3 rot = transform.localEulerAngles;
			rot.x += bulletStat.shooter.transform.eulerAngles.y;
			transform.localEulerAngles = rot;
		}
	}
}
