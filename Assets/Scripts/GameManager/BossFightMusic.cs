using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightMusic : MonoBehaviour
{
    

    private AudioSource audioSource;
    [SerializeField] private AudioClip CFight;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetBossFightMusic()
    {
        Debug.Log("LJDFHULDSFHUDSFHLAD");
        audioSource.Stop();
        audioSource.clip = CFight;
        audioSource.loop = true;
        audioSource.Play();
    }
}
