using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private Player player;
    [SerializeField] float maxVolume;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameManager.Instance.GetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= 15)
                audioSource.volume = maxVolume;
            else
            {
                float pos = Mathf.Max(0f, 22 - dist);
                audioSource.volume = maxVolume * (100f * pos / 6f) / 100f;
            }
        }
        else
            audioSource.volume = 0f;
    }
}
