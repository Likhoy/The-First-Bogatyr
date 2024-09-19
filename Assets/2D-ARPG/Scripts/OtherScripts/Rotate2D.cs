using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate2D : MonoBehaviour {
	public float value = 30;

	void Update(){
		transform.Rotate(0 ,0 ,value * Time.deltaTime);
	}
}
