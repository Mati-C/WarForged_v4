using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    Model_Player _player;
    Model_TE_Melee _te;

    public bool attacksTutorialFinish;
    public bool defendTutorialFinish;

    public int hitCounts;
    public int hitCountsMax;
    public int defendCounts;
    public int defendCountsMax;

    Action AttacksPlayerEvent;
    Action DefendPlayerEvent;

    public TextMeshPro tutorialHitText;
    public TextMeshPro tutorialDefenceText;


    private void Awake()
    {
        _player = FindObjectOfType<Model_Player>();
        _te = FindObjectOfType<Model_TE_Melee>();

        _player.MakeDamageTutorialEvent += PlusHitCounts;
        _player.DefendTutorialEvent += PlusDefendCounts;


        tutorialHitText = GameObject.Find("Text Tutorial Hit").GetComponent<TextMeshPro>();
        tutorialDefenceText = GameObject.Find("Text Tutorial Defend").GetComponent<TextMeshPro>();
        tutorialDefenceText.gameObject.SetActive(false);
        tutorialHitText.text = "Use Left Click to Hit enemies " + hitCounts +"/9";
        tutorialDefenceText.text = "Use Right Click to Defend " + defendCounts + "/4";
        
    }

    void Start()
    {
        
    }
    
    void Update()
    {
       
    }

    void PlusDefendCounts()
    {
        if (defendCounts < defendCountsMax)
        {
            defendCounts++;
            tutorialDefenceText.text = "Use Right Click to Defend " + defendCounts + "/4";
            if (defendCounts >= defendCountsMax)
            {
                defendTutorialFinish = true;
                //StartCoroutine(DesactivateText(tutorialHitText.gameObject, tutorialDefenceText.gameObject));
            }
        }
    }

    void PlusHitCounts()
    {
        if (hitCounts < hitCountsMax)
        {
            hitCounts++;
            tutorialHitText.text = "Use Left Click to Hit enemies " + hitCounts + "/9";
            if (hitCounts >= hitCountsMax)
            {
                attacksTutorialFinish = true;
                StartCoroutine(DefendTutorial());
                StartCoroutine(DesactivateText(tutorialHitText.gameObject, tutorialDefenceText.gameObject));
            }
        }
    }

    IEnumerator DefendTutorial()
    {
        while(!defendTutorialFinish)
        {
            _player.defenceTimer = 1;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DesactivateText(GameObject g, GameObject g2)
    {
        yield return new WaitForSeconds(2f);
        g.SetActive(false);
        g2.SetActive(true);
    }
}
