using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevelManager : MonoBehaviour
{
    bool _sceneChange;
    public int sceneID;
    FadeLevel _fade;
    public bool changeSceneInstant;

    private void Awake()
    {
        _fade = FindObjectOfType<FadeLevel>();
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
        if (c.GetComponent<Model_Player>()) ChangeScene();
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
