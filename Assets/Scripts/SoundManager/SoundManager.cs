﻿using System.Collections;
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

    [NamedArray(new string[] { "CHECKPOINT_PASS", "CHECKPOINT_IDLE", "DOOR_OPEN", "DOOR_CLOSE", "IRON_BARS", "KEY_COLLECTED", "JUGS_BREAK", "BARREL_BREAK" })]
    public AudioClip[] objects;

    [NamedArray(new string[] { "DIE_1", "DIE_2", "DIE_3", "DAMAGE_1", "DAMAGE_2", "DAMAGE_3", "DAMAGE_4", "DAMAGE_5", "DAMAGE_6", "FIREBALL"})]
    public AudioClip[] entity;

    [NamedArray(new string[] { "ROAR", "MONSTER_ATTACK1", "MONSTER_ATTACK2", "MONSTER_ATTACK3", "SMASH" })]
    public AudioClip[] boss;

    [NamedArray(new string[] { "TAKE_SWORD", "SAVE_SWORD", "SWING_1", "SWING_2", "SWING_3", "FIRE_SWORD", "SHORT_YELL" })]
    public AudioClip[] player;

    [NamedArray(new string[] { "LEVEL_1", "LEVEL_2", "BATTLE_MUSIC", "BOSS_MUSIC" })]
    public AudioClip[] music;

    [NamedArray(new string[] { "HARD", "SOFT", "WOOD" })]
    public AudioClip[] hit;

    [NamedArray(new string[] { "ROCKS_1", "ROCKS_2", "ROCKS_3", "ROCKS_4" })]
    public AudioClip[] rocks;

    #region SFX Lists
    [HideInInspector]
    public List<Entity> deathVoice = new List<Entity>() { Entity.DIE_1, Entity.DIE_2, Entity.DIE_3 };

    [HideInInspector]
    public List<Entity> damageVoice = new List<Entity>() { Entity.DAMAGE_1, Entity.DAMAGE_2, Entity.DAMAGE_3, Entity.DAMAGE_4, Entity.DAMAGE_5, Entity.DAMAGE_6 };

    [HideInInspector]
    public List<Player> swing = new List<Player>() { Player.SWING_1, Player.SWING_2, Player.SWING_3 };

    [HideInInspector]
    public List<Boss> bossAttack = new List<Boss>() { Boss.MONSTER_ATTACK1, Boss.MONSTER_ATTACK2, Boss.MONSTER_ATTACK3 };

    [HideInInspector]
    public List<Rocks> fallingRocks = new List<Rocks>() { Rocks.ROCKS_1, Rocks.ROCKS_2, Rocks.ROCKS_3, Rocks.ROCKS_4 };
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

        if (soundType.GetType() == typeof(Objects))
            audioSource.clip = objects[id];
        else if (soundType.GetType() == typeof(Entity))
            audioSource.clip = entity[id];
        else if (soundType.GetType() == typeof(Boss))
            audioSource.clip = boss[id];
        else if (soundType.GetType() == typeof(Player))
            audioSource.clip = player[id];
        else if (soundType.GetType() == typeof(Music))
            audioSource.clip = music[id];
        else if (soundType.GetType() == typeof(Hit))
            audioSource.clip = hit[id];
        else if (soundType.GetType() == typeof(Rocks))
            audioSource.clip = hit[id];

        audioSource.loop = loop;
        s.transform.position = position;
        audioSource.spatialBlend = position != new Vector3() ? 1 : 0;
        audioSource.volume = volume;
        audioSource.pitch = randomPitch ? UnityEngine.Random.Range(0.9f, 1.1f) : 1;
        audioSource.Play();
    }

    public void PlayRandom<T>(List<T> sounds, Vector3 position = new Vector3(), bool randomPitch = false, float pitch = 1f, float volume = 1f, bool loop = false)
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

        if (soundType.GetType() == typeof(Objects))
            audioSource.clip = objects[id];
        else if (soundType.GetType() == typeof(Entity))
            audioSource.clip = entity[id];
        else if (soundType.GetType() == typeof(Boss))
            audioSource.clip = boss[id];
        else if (soundType.GetType() == typeof(Player))
            audioSource.clip = player[id];
        else if (soundType.GetType() == typeof(Music))
            audioSource.clip = music[id];
        else if (soundType.GetType() == typeof(Hit))
            audioSource.clip = hit[id];
        else if (soundType.GetType() == typeof(Rocks))
            audioSource.clip = hit[id];

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
        combatAudio.clip = music[(int)Music.BOSS_MUSIC];
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
                combatAudio.volume = t * (musicVolume);
                ambienceAudio.volume = Mathf.Lerp(musicVolume, 0.75f, t);
            }
            else
            {
                combatAudio.volume = Mathf.Lerp(musicVolume, 0, t);
                ambienceAudio.volume = t;
            }
            yield return new WaitForEndOfFrame();
        }

        if (!activate)
            combatAudio.Stop();
    }
}
 namespace Sound
{
    public enum Objects
    {
        CHECKPOINT_PASS,
        CHECKPOINT_IDLE,
        DOOR_OPEN,
        DOOR_CLOSE,
        IRON_BARS,
        KEY_COLLECTED,
        JUGS_BREAK,
        BARREL_BREAK
    }

    public enum Entity
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
        FIREBALL
    }

    public enum Boss
    {
        ROAR,
        MONSTER_ATTACK1,
        MONSTER_ATTACK2,
        MONSTER_ATTACK3,
        SMASH
    }

    public enum Player
    {
        TAKE_SWORD,
        SAVE_SWORD,
        SWING_1,
        SWING_2,
        SWING_3,
        FIRE_SWORD,
        SHORT_YELL
    }

    public enum Music
    {
        LEVEL_1,
        LEVEL_2,
        BATTLE_MUSIC,
        BOSS_MUSIC
    }

    public enum Hit
    {
        HARD,
        SOFT,
        WOOD
    }

    public enum Rocks
    {
        ROCKS_1,
        ROCKS_2,
        ROCKS_3,
        ROCKS_4
    }
}