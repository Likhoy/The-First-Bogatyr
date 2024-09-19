using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour {
	public Transform prefab;

	void OnDestroy(){
		Instantiate(prefab , transform.position , transform.rotation);
	}

}
