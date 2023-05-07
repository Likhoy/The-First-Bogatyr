using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowsColl : MonoBehaviour
{
    private AudioSource audioEffects;
    [SerializeField] private AudioClip[] CSnake;

    void Start()
    {
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            System.Random rand = new System.Random();
            audioEffects.PlayOneShot(CSnake[rand.Next(CSnake.Length)]);
        }
    }
}
