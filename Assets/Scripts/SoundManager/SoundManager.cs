using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sound;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public GameObject soundCuePrefab;
    public float musicTransitionTime;
    public float musicVolume;

    AudioSource ambienceAudio;
    AudioSource combatAudio;

    bool aux;

    [NamedArrayAttribute(new string[] { "CHECKPOINT_PASS", "CHECKPOINT_START", "CHECKPOINT_IDLE", "DOOR_OPEN", "DOOR_CLOSE", "IRON_BARS", "KEY_COLLECTED", "JUGS_BREAK", "BARREL_BREAK" })]
    public AudioClip[] miscSFX;

    [NamedArrayAttribute(new string[] { "TAKE_SWORD", "SAVE_SWORD", "SWORD_1", "SWORD_2", "SWORD_3" })]
    public AudioClip[] playerSFX;

    void Awake()
    {
        instance = this;
        foreach (var item in GetComponents<AudioSource>())
        {
            if (item.playOnAwake)
                ambienceAudio = item;
            else
                combatAudio = item;
        }
        aux = false;
    }

    public void Play<T>(T soundType, Vector3 position = new Vector3(), bool randomPitch = false, float volume = 0.7f, bool loop = false)
    {
        int id = -1;
        id = (int)Convert.ChangeType(soundType, typeof(Int32));

        GameObject s = Instantiate(soundCuePrefab);
        AudioSource audioSource = s.GetComponent<AudioSource>();

        if (soundType.GetType() == typeof(MiscSound))
            audioSource.clip = miscSFX[id];
        else if (soundType.GetType() == typeof(PlayerSound))
            audioSource.clip = playerSFX[id];

        audioSource.loop = loop;
        s.transform.position = position;
        audioSource.spatialBlend = position != new Vector3() ? 1 : 0;
        audioSource.volume = volume;
        audioSource.pitch = randomPitch ? UnityEngine.Random.Range(0.9f, 1.1f) : 1;
        audioSource.Play();
    }

    public void PlayRandom<T>(List<T> sounds, Vector3 position = new Vector3(), bool randomPitch = false, float volume = 0.6f, bool loop = false)
    {
        T soundType = sounds[UnityEngine.Random.Range(0, sounds.Count)];

        int id = -1;
        id = (int)Convert.ChangeType(soundType, typeof(Int32));

        GameObject s = Instantiate(soundCuePrefab);
        AudioSource audioSource = s.GetComponent<AudioSource>();

        if (soundType.GetType() == typeof(MiscSound))
            audioSource.clip = miscSFX[id];
        else if (soundType.GetType() == typeof(PlayerSound))
            audioSource.clip = playerSFX[id];

        audioSource.loop = loop;
        s.transform.position = position;
        audioSource.spatialBlend = position != new Vector3() ? 1 : 0;
        audioSource.volume = volume;
        audioSource.pitch = randomPitch ? UnityEngine.Random.Range(0.9f, 1.1f) : 1;
        audioSource.Play();
    }

    public void CombatMusic(bool activate)
    {
        if (aux == activate) return;
        aux = activate;
        StartCoroutine(CombatMusicCorroutine(activate));
    }

    IEnumerator CombatMusicCorroutine(bool activate)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / musicTransitionTime;
            if (activate)
            {
                if (!combatAudio.isPlaying)
                    combatAudio.Play();
                combatAudio.volume = t * musicVolume;
                ambienceAudio.volume = Mathf.Lerp(musicVolume * 0.75f, musicVolume, 1 - t);
            }
            else
            {
                combatAudio.volume = (1 - t) * musicVolume;
                ambienceAudio.volume = t * musicVolume;
            }
            yield return new WaitForEndOfFrame();
        }

        if (!activate)
            combatAudio.Stop();
    }
}
 namespace Sound
{
    public enum MiscSound
    {
        CHECKPOINT_PASS,
        CHECKPOINT_START,
        CHECKPOINT_IDLE,
        DOOR_OPEN,
        DOOR_CLOSE,
        IRON_BARS,
        KEY_COLLECTED,
        JUGS_BREAK,
        BARREL_BREAK
    }

    public enum PlayerSound
    {
        TAKE_SWORD,
        SAVE_SWORD,
        SWORD_1,
        SWORD_2,
        SWORD_3
    }
}