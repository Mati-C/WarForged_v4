using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sound;

public class ChangeLevelManager : MonoBehaviour
{
    bool _sceneChange;
    public int sceneID;
    FadeLevel _fade;
    public bool changeSceneInstant;
    public bool endGame;
    public GameObject winMenu;
    public Model_Player _player;
    public Viewer_Player _viewer;
    public AudioSource winMusic;

    private void Awake()
    {
        
        _fade = FindObjectOfType<FadeLevel>();
        _player = FindObjectOfType<Model_Player>();
        _viewer = FindObjectOfType<Viewer_Player>();
    }

    public void ChangeScene()
    {
        if (!_sceneChange && !changeSceneInstant)
        {
            _fade.FadeIn(true);
            StartCoroutine(WaitingForChange());
        }

        if (changeSceneInstant) LoadingScreen.instance.LoadLevel(sceneID);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Model_Player>() && !endGame) ChangeScene();

        if (c.GetComponent<Model_Player>() && endGame)  StartCoroutine(WinGame());
    }

    IEnumerator WinGame()
    {
        _fade.FadeIn(false);
        _player.onCinematic = true;
        _player.idleEvent();
        winMusic.volume = 0;
        winMusic.Play();
        
        while (_fade.fadeIn)
        {
            winMusic.volume += Time.deltaTime/3;
            if (winMusic.volume > 1) winMusic.volume = 1;
            SoundManager.instance.ambienceAudio.volume -= Time.deltaTime/3;
            if (SoundManager.instance.ambienceAudio.volume < 0) SoundManager.instance.ambienceAudio.volume = 0;
            yield return new WaitForEndOfFrame();
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        winMenu.SetActive(true);
        

        if (_viewer.pauseMenu.activeSelf) _viewer.pauseMenu.SetActive(false);

    }

    IEnumerator WaitingForChange()
    {
        _sceneChange = true;

        while(_fade.fadeIn == true)
        {               
            yield return new WaitForEndOfFrame();
        }

        LoadingScreen.instance.LoadLevel(sceneID);
    }
}
