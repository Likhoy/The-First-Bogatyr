using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HireMercenary : MonoBehaviour {
	public AllyAi[] allyPrefab = new AllyAi[1];
	public Transform spawnPosition;

	public void SpawnAlly(int id){
		if(!GlobalStatus.mainPlayer){
			return;
		}
		Transform ally = Instantiate(allyPrefab[id].transform , transform.position , Quaternion.identity) as Transform;
		ally.GetComponent<AllyAi>().master = GlobalStatus.mainPlayer.transform;
		ally.GetComponent<AllyAi>().deadIfNoMaster = true;
	}
}
