using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour {
	public Collider2D obj1;
	public Collider2D obj2;
	public bool condition = true;

	void Start(){
		Physics2D.IgnoreCollision(obj1 , obj2 , condition);
	}

}
