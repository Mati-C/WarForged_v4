using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject title;
    public GameObject buttons;

    public Material titleMat;
    public float timeToUnveilMenu;
    public float menuFillAmount;
    public float fillSpeed;
    // Start is called before the first frame update
    void Start()
    {
        menuFillAmount = 0;
        buttons.SetActive(false);
        //StartCoroutine(TitleEffect());
        //StartCoroutine(TitleFlame());
        titleMat = title.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        timeToUnveilMenu -= Time.deltaTime;
        if (timeToUnveilMenu <= 0)
        {
            timeToUnveilMenu = 0;
            if (menuFillAmount < 1) menuFillAmount += Time.deltaTime * fillSpeed;
            if (menuFillAmount >= 0.85) buttons.SetActive(true);
            titleMat.SetFloat("_DissolveAmount", menuFillAmount);
        }
    }

    //IEnumerator TitleEffect()
    //{
    //    Material titleMat = title.GetComponent<Renderer>().material;
    //    float timer = titleTime * 0.3f;
    //    while (timer < titleTime)
    //    {
    //        timer += Time.deltaTime;
    //        titleMat.SetFloat("_Life", timer / titleTime);
    //        yield return new WaitForEndOfFrame();
    //    }
    //}

    //IEnumerator TitleFlame()
    //{
    //    GameObject flame = title.transform.GetChild(0).gameObject;
    //    yield return new WaitForSeconds(titleTime * 0.7f);
    //    flame.SetActive(true);
    //    buttons.SetActive(true);
    //}
}
