using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public GameObject soundCuePrefab;

    [NamedArrayAttribute(new string[] { "CHECKPOINT", "DOOR_OPEN", "DOOR_CLOSE", "IRON_BARS_OPEN", "IRON_BARS_CLOSE" })]
    public AudioClip[] audioClips;

    void Awake()
    {
        instance = this;
    }

    public void Play(int id, bool loop = false, Vector3 position = new Vector3(), float volume = 1, float pitch = 1)
    {
        GameObject s = Instantiate(soundCuePrefab);
        AudioSource audioSource = s.GetComponent<AudioSource>();
        audioSource.clip = audioClips[id];
        audioSource.loop = loop;
        s.transform.position = position;
        audioSource.spatialBlend = position != new Vector3() ? 1 : 0;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
    }
}
 namespace SoundTypes
{
    public enum SoundType
    {
        CHECKPOINT,
        DOOR_OPEN,
        DOOR_CLOSE,
        IRON_BARS_OPEN,
        IRON_BARS_CLOSE
    }
}