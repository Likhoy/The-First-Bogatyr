using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTooltips : MonoBehaviour {
	public GameObject[] tooltips = new GameObject[2];

	public static WeaponTooltips showWeaponTooltips;

	void Start(){
		showWeaponTooltips = GetComponent<WeaponTooltips>();
		if(GlobalStatus.mainPlayer){
			SetTooltip(GlobalStatus.mainPlayer.GetComponent<AttackTrigger>().weaponType);
		}
	}

	public void SetTooltip(int weaponType){
		for(int a = 0; a < tooltips.Length; a++){
			tooltips[a].SetActive(false);
		}
		tooltips[weaponType].SetActive(true);
	}
}
