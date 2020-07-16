using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    PlayableDirector pd;

    public GameObject title;
    public GameObject pressAnyKey;
    public GameObject buttons;
    public GameObject swordLevels;
    public RawImage fade;
    public float titleTime;

    Material titleMat;
    GameObject flame;
    bool isInTitle;

    void Start()
    {
        buttons.SetActive(false);
        pressAnyKey.SetActive(false);
        StartCoroutine(TitleEffect());
        titleMat = title.GetComponent<Renderer>().material;
        pd = FindObjectOfType<PlayableDirector>();
        isInTitle = false;
        flame = title.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (Input.anyKey && isInTitle)
        {
            SoundManager.instance.Play2D(3, 0.7f, false);
            isInTitle = false;
            StartCoroutine(AdvanceCamera(false));
        }
    }

    IEnumerator TitleEffect()
    {
        yield return new WaitForEndOfFrame();
        pd.Pause();

        float timer = 0;
        while (timer < titleTime)
        {
            timer += Time.deltaTime;
            titleMat.SetFloat("_Life", Mathf.Lerp(0.6f, 0.9f, timer / titleTime));
            yield return new WaitForEndOfFrame();
        }
        isInTitle = true;
        pressAnyKey.SetActive(true);
        flame.SetActive(true);
    }

    public void Play()
    {
        StartCoroutine(AdvanceCamera(true));
    }

    IEnumerator AdvanceCamera(bool toPlay)
    {
        pressAnyKey.SetActive(false);
        pd.Play();
        if (toPlay)
        {
            float t = 1;
            flame.SetActive(false);
            while (t > 0)
            {
                t -= Time.deltaTime;
                titleMat.SetFloat("_Life", Mathf.Lerp(0.6f, 0.9f, t));
                Image[] sprites = buttons.GetComponentsInChildren<Image>();
                foreach (var i in sprites)
                {
                    var tempColor = i.color;
                    tempColor.a -= Time.deltaTime;
                    i.color = tempColor;
                }
                yield return new WaitForEndOfFrame();
            }
            buttons.SetActive(false);

            yield return new WaitForSeconds(2);

            float t2 = 0;
            fade.gameObject.SetActive(true);
            while (t2 < 1)
            {
                t2 += Time.deltaTime;
                var tempColor = fade.color;
                tempColor.a += Time.deltaTime;
                fade.color = tempColor;
                yield return new WaitForEndOfFrame();
            }
            LoadingScreen.instance.LoadLevel(1);
        }
        else
        {
            yield return new WaitForSeconds(4);
            pd.Pause();
            buttons.SetActive(true);
        }
    }

    public void OpenWindow(GameObject window)
    {
        window.SetActive(true);
    }

    public void CloseWindow(GameObject window)
    {
        window.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
