using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour {
	public float duration = 1.5f;
	
	void Start(){
		Destroy(gameObject, duration);
	}
}
