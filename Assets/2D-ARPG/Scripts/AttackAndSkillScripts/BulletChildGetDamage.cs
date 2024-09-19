using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletChildGetDamage : MonoBehaviour {
	public Transform master;

	void Start(){
		if(!master){
			master = transform.root;
		}
		int dmg = master.GetComponent<BulletStatus>().playerAttack;
		string tag = master.GetComponent<BulletStatus>().shooterTag;
		GameObject shooter = master.GetComponent<BulletStatus>().shooter;
		GetComponent<BulletStatus>().Setting(dmg , dmg , tag , shooter);
	}
}
