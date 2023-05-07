using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FielderDamage : MonoBehaviour
{
    private AudioSource audioEffects;
    [SerializeField] private AudioClip CDamage;

    void Start()
    {
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
            audioEffects.PlayOneShot(CDamage);
    }
}
