using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundCue : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!audioSource.isPlaying && Time.timeScale != 0)
            Destroy(gameObject);
    }
}
