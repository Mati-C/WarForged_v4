using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeLevel : MonoBehaviour
{
    public bool fadeIn;
    public bool fadeOut;
    public bool playerCantMove;
    public RawImage fadeImage;

    private void Awake()
    {
        fadeImage = GameObject.Find("Start Fade").GetComponent<RawImage>();
        if (fadeIn) FadeIn(false);
        if (fadeOut) FadeOut(false);
    }

    void Update()
    {
        
    }

    public void FadeIn(bool playerMove)
    {
        fadeIn = true;
        if (!playerMove) playerCantMove = true;
        var tempColor = fadeImage.color;
        tempColor.a = 0f;
        fadeImage.color = tempColor;
        StartCoroutine(FadeInCorrutine());
    }


    public void FadeOut(bool playerMove)
    {
        fadeOut = false;
        if(!playerMove) playerCantMove = true;
        var tempColor = fadeImage.color;
        tempColor.a = 1f;
        fadeImage.color = tempColor;
        StartCoroutine(FadeOutCorrutine());
    }

    IEnumerator FadeOutCorrutine()
    {
        while(fadeImage.color.a >0)
        {
            var tempColor = fadeImage.color;
            tempColor.a -= Time.deltaTime /3.5f;
            fadeImage.color = tempColor;
            yield return new WaitForEndOfFrame();
        }
        fadeOut = false;
        playerCantMove = false;
    }

    IEnumerator FadeInCorrutine()
    {
        while (fadeImage.color.a < 1)
        {
            var tempColor = fadeImage.color;
            tempColor.a += Time.deltaTime / 3.5f;
            fadeImage.color = tempColor;
            yield return new WaitForEndOfFrame();
        }
        fadeIn = false;
        playerCantMove = false;
    }
}
