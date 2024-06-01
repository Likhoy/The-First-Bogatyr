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
        audioSource.Stop();
        audioSource.clip = CFight;
        audioSource.loop = true;
        audioSource.Play();
    }
}
