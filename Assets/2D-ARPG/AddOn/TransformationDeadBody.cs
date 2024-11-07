using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationDeadBody : MonoBehaviour{
    public GameObject[] deadSprite = new GameObject[4];
    public string deathAnimationName = "Death";

    void Start(){
        for(int a = 0; a < deadSprite.Length; a++) {
            if(deadSprite[a]) {
                deadSprite[a].SetActive(false);
            }
        }
        deadSprite[PlatformerTransformation.currentForm].SetActive(true);
        if(deadSprite[PlatformerTransformation.currentForm].GetComponent<Animator>()) {
            deadSprite[PlatformerTransformation.currentForm].GetComponent<Animator>().Play(deathAnimationName);
        }
    }

}
