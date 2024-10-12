using UnityEngine;
using System.Collections;

public class Unparent : MonoBehaviour {

	void Start(){
		//transform.parent = null;
		transform.SetParent(null , true);
	}
}
