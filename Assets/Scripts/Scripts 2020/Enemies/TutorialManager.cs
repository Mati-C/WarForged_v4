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
    public bool canMove;

    public int hitCounts;
    public int hitCountsMax;
    public int defendCounts;
    public int defendCountsMax;
    public int quickDefendCounts;
    public int quickDefendCountsMax;
    public int heavyAttackCounts;
    public int heavyAttackCountsMax;
    public float moveDefenceAmount;
    public float moveDefenceAmountMax;


    TextMeshPro _tutorialHitText;
    TextMeshPro _tutorialDefenceText;
    TextMeshPro _tutorialQuickDefenceText;
    TextMeshPro _tutorialMoveDefenceText;
    TextMeshPro _tutorialHeavyAttackText;
    TextMeshPro _tutorialFireSwordText;
    public Transform objective1;
    Transform _objective;
    GameObject _arrow;

    public Color colorSucsess;
    public Color colorSucsessOutline;
    public Color colorIncomplite;
    public Color colorIncompliteOutline;

    private void Awake()
    {
        _player = FindObjectOfType<Model_Player>();
        _te = FindObjectOfType<Model_TE_Melee>();

        _player.MakeDamageTutorialEvent += PlusHitCounts;
        _player.DefendTutorialEvent += PlusDefendCounts;
        _player.KickTutorialEvent += PlusKicks;
        _player.ChargeAttackEvent += PlusHeavyAttack;

        StartCoroutine(HeavyAttack());

        _tutorialHitText = GameObject.Find("Text Tutorial Hit").GetComponent<TextMeshPro>();
        _tutorialDefenceText = GameObject.Find("Text Tutorial Defend").GetComponent<TextMeshPro>();
        _tutorialQuickDefenceText = GameObject.Find("Text Tutorial Quick Defend").GetComponent<TextMeshPro>();
        _tutorialMoveDefenceText = GameObject.Find("Text Tutorial Move Defend ").GetComponent<TextMeshPro>();
        _tutorialHeavyAttackText = GameObject.Find("Text Tutorial Heavy Attack").GetComponent<TextMeshPro>();
        _tutorialFireSwordText = GameObject.Find("Text Tutorial Fire Sword").GetComponent<TextMeshPro>();
        _arrow = GameObject.Find("PointerPrefab");
        _tutorialDefenceText.gameObject.SetActive(false);
        _tutorialQuickDefenceText.gameObject.SetActive(false);
        _tutorialMoveDefenceText.gameObject.SetActive(false);
        _tutorialHeavyAttackText.gameObject.SetActive(false);
        _tutorialFireSwordText.gameObject.SetActive(false);
        _tutorialHitText.text = "Use Left Click to Hit enemies " + hitCounts +"/" + hitCountsMax;
        _tutorialDefenceText.text = "Use Right Click to Defend " + defendCounts + "/"+ defendCountsMax;
        _tutorialQuickDefenceText.text = "Use Left Click quick when enemies attack you to kick them " + quickDefendCounts + "/"+ quickDefendCountsMax;
        _tutorialHeavyAttackText.text = "Keep Left Click to make an area attack " + heavyAttackCounts + "/"+ heavyAttackCountsMax;
        _tutorialMoveDefenceText.text = "You can move while you are defending";
        _tutorialFireSwordText.text = "Press Q to use the Fire Sword when the bar is full and kill the enemy";


    }

    void Start()
    {
        _objective = objective1;
        _te.life = 99999;
        _te.maxLife = 99999;
    }
    
    void Update()
    {
        _arrow.transform.position = _player.transform.position + Vector3.up/2;
        var dir = (_objective.position - _arrow.transform.position).normalized;
        _arrow.transform.forward = -dir;
    }

    void PlusFireSword()
    {

    }

    IEnumerator FlamesOn()
    {
        _player.fireEnergy = 100;
        while(!_player.flamesOn)
        { 
            yield return new WaitForEndOfFrame();
        }
        _te.life = 50;
        _te.maxLife = 50;

        while (_te.life>0)
        {
            yield return new WaitForEndOfFrame();
        }

        _tutorialFireSwordText.faceColor = colorSucsess;
        _tutorialFireSwordText.outlineColor = colorSucsessOutline;

    }

    void PlusHeavyAttack()
    {
        if (heavyAttackCounts < heavyAttackCountsMax)
        {
            heavyAttackCounts++;
            _tutorialHeavyAttackText.text = "Keep Left Click to make an area attack " + heavyAttackCounts + "/" + heavyAttackCountsMax;
            if (heavyAttackCounts >= heavyAttackCountsMax)
            {
                _tutorialHeavyAttackText.faceColor = colorSucsess;
                _tutorialHeavyAttackText.outlineColor = colorSucsessOutline;
                StartCoroutine(FlamesOn());
                StartCoroutine(DesactivateText(_tutorialHeavyAttackText.gameObject, _tutorialFireSwordText.gameObject));
            }
        }
    }

    void PlusDefenceMove()
    {
        if(moveDefenceAmount < moveDefenceAmountMax && _player.onDefence)
        {
            moveDefenceAmount += Time.deltaTime * 2;
            if(moveDefenceAmount >= moveDefenceAmountMax)
            {
                _tutorialMoveDefenceText.faceColor = colorSucsess;
                _tutorialMoveDefenceText.outlineColor = colorSucsessOutline;
                StartCoroutine(DesactivateText(_tutorialMoveDefenceText.gameObject, _tutorialHeavyAttackText.gameObject));
            }
        }
    }

    void PlusKicks()
    {
        if (quickDefendCounts < defendCountsMax)
        {
            quickDefendCounts++;
            _tutorialQuickDefenceText.text = "Use Left Click quick when enemies attack you to kick them " + quickDefendCounts + "/" + quickDefendCountsMax;
            if (quickDefendCounts >= defendCountsMax)
            {
                canMove = true;
                _tutorialQuickDefenceText.faceColor = colorSucsess;
                _tutorialQuickDefenceText.outlineColor = colorSucsessOutline;
                _player.WalkEvent += PlusDefenceMove;
                StartCoroutine(DesactivateText(_tutorialQuickDefenceText.gameObject, _tutorialMoveDefenceText.gameObject));
            }
        }
    }

    void PlusDefendCounts()
    {
        if (defendCounts < defendCountsMax)
        {
            defendCounts++;
            _tutorialDefenceText.text = "Use Right Click to Defend " + defendCounts + "/" + defendCountsMax;
            if (defendCounts >= defendCountsMax)
            {
                defendTutorialFinish = true;
                _tutorialDefenceText.faceColor = colorSucsess;
                _tutorialDefenceText.outlineColor = colorSucsessOutline;
                StartCoroutine(DesactivateText(_tutorialDefenceText.gameObject, _tutorialQuickDefenceText.gameObject));
            }
        }
    }

    void PlusHitCounts()
    {
        if (hitCounts < hitCountsMax)
        {
            hitCounts++;
            _tutorialHitText.text = "Use Left Click to Hit enemies " + hitCounts + "/9";
            if (hitCounts >= hitCountsMax)
            {
                _tutorialHitText.faceColor = colorSucsess;
                _tutorialHitText.outlineColor = colorSucsessOutline;
                attacksTutorialFinish = true;
                StartCoroutine(DefendTutorial());
                StartCoroutine(DesactivateText(_tutorialHitText.gameObject, _tutorialDefenceText.gameObject));
            }
        }
    }

    IEnumerator HeavyAttack()
    {
        while (!canMove)
        {
            _player.chargeAttackAmount = 0;
            _player.fireEnergy = 0;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DefendTutorial()
    {
        _tutorialDefenceText.faceColor = colorIncomplite;
        _tutorialDefenceText.outlineColor = colorIncompliteOutline;
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
