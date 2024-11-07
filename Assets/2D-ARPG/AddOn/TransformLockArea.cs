using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TransformLockArea : MonoBehaviour{
    
    void Start(){
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D x) {
        if(x.tag == "Player" && x.GetComponent<PlatformerTransformation>()) {
            x.GetComponent<PlatformerTransformation>().transformLock = true;
        }
    }

    void OnTriggerExit2D(Collider2D x) {
        if(x.tag == "Player" && x.GetComponent<PlatformerTransformation>()) {
            x.GetComponent<PlatformerTransformation>().transformLock = false;
        }
    }
}
