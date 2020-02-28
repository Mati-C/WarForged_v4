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
    public AudioSource combatAudio;

    bool aux;

    [NamedArrayAttribute(new string[] { "CHECKPOINT_PASS", "CHECKPOINT_START", "CHECKPOINT_IDLE", "DOOR_OPEN", "DOOR_CLOSE", "IRON_BARS", "KEY_COLLECTED", "JUGS_BREAK", "BARREL_BREAK" })]
    public AudioClip[] miscSFX;

    [NamedArrayAttribute(new string[] { "TAKE_SWORD", "SAVE_SWORD", "SWING_1", "SWING_2", "SWING_3", "BODY_IMPACT_1", "BODY_IMPACT_2", "ROAR", "MONSTER_ATTACK1", "MONSTER_ATTACK2", "MONSTER_ATTACK3", "SMASH", "FIREBALL" })]
    public AudioClip[] playerSFX;

    [NamedArrayAttribute(new string[] { "DIE_1", "DIE_2", "DIE_3", "DAMAGE_1", "DAMAGE_2", "DAMAGE_3", "DAMAGE_4", "DAMAGE_5", "DAMAGE_6", "LAST_COMBO_HIT" })]
    public AudioClip[] voices;

    public AudioClip bossMusic;

    #region SFX Lists
    public List<Voice> deathVoice = new List<Voice>() { Voice.DIE_1, Voice.DIE_2, Voice.DIE_3 };
    public List<Voice> damageVoice = new List<Voice>() { Voice.DAMAGE_1, Voice.DAMAGE_2, Voice.DAMAGE_3, Voice.DAMAGE_4, Voice.DAMAGE_5, Voice.DAMAGE_6 };
    public List<EntitySound> swing = new List<EntitySound>() { EntitySound.SWING_1, EntitySound.SWING_2, EntitySound.SWING_3 };
    public List<EntitySound> bodyImpact = new List<EntitySound>() { EntitySound.BODY_IMPACT_1, EntitySound.BODY_IMPACT_2 };
    #endregion

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
        else if (soundType.GetType() == typeof(EntitySound))
            audioSource.clip = playerSFX[id];
        else if (soundType.GetType() == typeof(Voice))
            audioSource.clip = voices[id];

        audioSource.loop = loop;
        s.transform.position = position;
        audioSource.spatialBlend = position != new Vector3() ? 1 : 0;
        audioSource.volume = volume;
        audioSource.pitch = randomPitch ? UnityEngine.Random.Range(0.9f, 1.1f) : 1;
        audioSource.Play();
    }

    public void PlayRandom<T>(List<T> sounds, Vector3 position = new Vector3(), bool randomPitch = false, float volume = 0.6f, float pitch = 1f, bool loop = false)
    {
        T soundType;
        if (sounds.Count == 1)
            soundType = sounds[0];
        else
            soundType = sounds[UnityEngine.Random.Range(0, sounds.Count)];

        int id = -1;
        id = (int)Convert.ChangeType(soundType, typeof(Int32));

        GameObject s = Instantiate(soundCuePrefab);
        AudioSource audioSource = s.GetComponent<AudioSource>();

        if (soundType.GetType() == typeof(MiscSound))
            audioSource.clip = miscSFX[id];
        else if (soundType.GetType() == typeof(EntitySound))
            audioSource.clip = playerSFX[id];
        else if (soundType.GetType() == typeof(Voice))
            audioSource.clip = voices[id];

        audioSource.loop = loop;
        s.transform.position = position;
        audioSource.spatialBlend = position != new Vector3() ? 1 : 0;
        audioSource.volume = volume;
        audioSource.pitch = randomPitch ? UnityEngine.Random.Range(pitch * 0.9f, pitch * 1.1f) : pitch;
        audioSource.Play();
    }

    public void CombatMusic(bool activate)
    {
        if (aux == activate) return;
        aux = activate;
        StartCoroutine(CombatMusicCorroutine(activate));
    }

    public void BossMusic(bool activate)
    {
        if (aux == activate) return;
        combatAudio.clip = bossMusic;
        aux = activate;
        musicVolume *= 1.2f;
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
                combatAudio.volume = t * (musicVolume * 0.75f);
                ambienceAudio.volume = Mathf.Lerp(musicVolume, musicVolume * 0.75f, t);
            }
            else
            {
                combatAudio.volume = Mathf.Lerp(musicVolume * 0.75f, 0, t);
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

    public enum EntitySound
    {
        TAKE_SWORD,
        SAVE_SWORD,
        SWING_1,
        SWING_2,
        SWING_3,
        BODY_IMPACT_1,
        BODY_IMPACT_2,
        ROAR,
        MONSTER_ATTACK1,
        MONSTER_ATTACK2,
        MONSTER_ATTACK3,
        SMASH,
        FIREBALL
    }

    public enum Voice
    {
        DIE_1,
        DIE_2,
        DIE_3,
        DAMAGE_1,
        DAMAGE_2,
        DAMAGE_3,
        DAMAGE_4,
        DAMAGE_5,
        DAMAGE_6,
        LAST_COMBO_HIT
    }
}