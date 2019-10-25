using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public GameObject soundCuePrefab;
    public float musicTransitionTime;
    public float musicVolume;

    AudioSource ambienceAudio;
    AudioSource combatAudio;

    bool aux;

    [NamedArrayAttribute(new string[] { "CHECKPOINT", "DOOR_OPEN", "DOOR_CLOSE", "IRON_BARS_OPEN", "IRON_BARS_CLOSE" })]
    public AudioClip[] audioClips;

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
                ambienceAudio.volume = Mathf.Lerp(musicVolume * 0.75f, musicVolume, 1 - t) * musicVolume;
            }
            else
            {
                combatAudio.volume = (1 - t) * musicVolume;
                ambienceAudio.volume = t *musicVolume;
            }
            yield return new WaitForEndOfFrame();
        }

        if (!activate)
            combatAudio.Stop();
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