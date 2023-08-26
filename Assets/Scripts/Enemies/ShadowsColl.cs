using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowsColl : MonoBehaviour
{
    private float timer;

    private AudioSource audioEffects;
    [SerializeField] private AudioClip[] CSnake;

    void Start()
    {
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
        timer = 1000;
    }

    void Update()
    {
        if (timer < 1000)
            timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && timer  >= 1000)
        {
            System.Random rand = new System.Random();
            audioEffects.PlayOneShot(CSnake[rand.Next(CSnake.Length)]);
            timer = 0;
        }
    }
}
