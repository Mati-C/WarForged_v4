using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    Model_Player _player;
    Viewer_Player _view;
    public Model_TE_Melee _te;
    public Model_E_Mage mage1;
    public List<ClassEnemy> secondEnemies = new List<ClassEnemy>();
    public List<ClassEnemy> thirdEnemies = new List<ClassEnemy>();
    public List<Transform> phPos = new List<Transform>();

    public bool attacksTutorialFinish;
    public bool defendTutorialFinish;
    public bool canMove;
    public bool fireSword;
    public bool checkpoint;

    public int hitCounts;
    public int hitCountsMax;
    public int defendCounts;
    public int defendCountsMax;
    public int quickDefendCounts;
    public int quickDefendCountsMax;
    public int heavyAttackCounts;
    public int heavyAttackCountsMax;
    public int dodgeCounts;
    public int dodgeCountsMax;
    public float moveDefenceAmount;
    public float moveDefenceAmountMax;
    public float timerCheckpoint;
    public float timerCheckpointMax;
    public int secondEnemiesAlive;

    TextMeshPro _tutorialText;

    public Transform objective1;
    public Transform objective2;
    public Transform objective3;
    public Transform objective4;
    public Transform objective5;
    public Transform objective6;
    public Transform objective7;
    public Transform objective8;

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
        _view = FindObjectOfType<Viewer_Player>();

        _player.MakeDamageTutorialEvent += PlusHitCounts;
        _player.DefendTutorialEvent += PlusDefendCounts;
        _player.KickTutorialEvent += PlusKicks;
        _player.ChargeAttackEvent += PlusHeavyAttack;

        StartCoroutine(HeavyAttack());
        StartCoroutine(MageInmortal());

        _tutorialText = GameObject.Find("Text Tutorial").GetComponent<TextMeshPro>();
        _tutorialText.text = "Use Left Click to Hit enemies " + hitCounts + "/9";
        _arrow = GameObject.Find("PointerPrefab");
        _player.chargeAttackCasted = true;
      

    }

    void Start()
    {
        _objective = objective1;
        _te.life = 99999;
        _te.maxLife = 99999;

        for (int i = 0; i < thirdEnemies.Count; i++)
        {
            thirdEnemies[i].transform.position = phPos[i].position;
        }
    }
    
    void Update()
    {
        _arrow.transform.position = _player.transform.position + Vector3.up/2;
        var dir = (_objective.position - _arrow.transform.position).normalized;
        _arrow.transform.forward = -dir;

        PlusCheckpoint();
    }

    void PlusFireSword()
    {

    }

    IEnumerator FlamesOn()
    {
        yield return new WaitForSeconds(2);
        _player.fireEnergy = _player.fireSword.energyToUseFireSword;
        
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _tutorialText.text = "When the Ability Bar is full, press Q to use the Fire Sword and kill the enemy";
        while(!_player.flamesOn)
        {
            _view.powerBar.fillAmount = 1;
            yield return new WaitForEndOfFrame();
        }
        _te.life = 50;
        _te.maxLife = 50;

        while (_te.life>0)
        {
            yield return new WaitForEndOfFrame();
        }

        _tutorialText.faceColor = colorSucsess;
        _tutorialText.outlineColor = colorSucsessOutline;
        yield return new WaitForSeconds(2);
        _tutorialText.text = "When you Kill enemies, they give you experience to update the Fire Sword";
        yield return new WaitForSeconds(5);
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _objective = objective2;
        _tutorialText.GetComponent<TextTutorial>().target = objective2;
        _tutorialText.GetComponent<TextTutorial>().plusPos = new Vector3(0,2,0);
        _tutorialText.text = "These sources will serve as checkpoints, where you recover life and exchange the sword experience to level it up and improve it";
        fireSword = true;
    }

    IEnumerator SecondEnemies()
    {
        _objective = objective5;
        yield return new WaitForSeconds(2);
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _tutorialText.GetComponent<TextTutorial>().target = objective5;
        _tutorialText.GetComponent<TextTutorial>().plusPos = new Vector3(0, 1.5f, 0);
        _tutorialText.text = "Kill all the enemies " + secondEnemiesAlive + "/2";
        bool alive = false;
        while(!alive)
        {
            if (secondEnemiesAlive >= 2) alive = true;

            for (int i = 0; i < secondEnemies.Count; i++)
            {
                if (secondEnemies[i].life <= 0)
                {
                    secondEnemiesAlive++;
                    _tutorialText.text = "Kill all the enemies " + secondEnemiesAlive + "/2";
                    secondEnemies.Remove(secondEnemies[i]);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        _tutorialText.faceColor = colorSucsess;
        _tutorialText.outlineColor = colorSucsessOutline;
        yield return new WaitForSeconds(2);
        _player.DodgeTutorialEvent += PlusDodge;
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _tutorialText.text = "You can dodge the fire balls pressing SPACE to dodge " + dodgeCounts + "/" + dodgeCountsMax;
        _tutorialText.GetComponent<TextTutorial>().target = mage1.transform;
        mage1.portalOrder = true;

        while(dodgeCounts < dodgeCountsMax)
        {
            _tutorialText.text = "You can dodge the fire balls pressing SPACE to dodge " + dodgeCounts + "/" + dodgeCountsMax;
            yield return new WaitForEndOfFrame();
        }

        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _tutorialText.text = "Kill all enemies 0/3";

        foreach (var item in thirdEnemies) item.portalOrder= true;

        int allEnemiesDead =0;
        bool alive2 = false;
        while (!alive2)
        {
            if (allEnemiesDead >= 3) alive2 = true;

            for (int i = 0; i < thirdEnemies.Count; i++)
            {
                if (thirdEnemies[i].life <= 0)
                {
                    allEnemiesDead++;
                    _tutorialText.text = "Kill all the enemies " + allEnemiesDead + "/3";
                    thirdEnemies.Remove(thirdEnemies[i]);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(DestroyRune());
    }

    IEnumerator DestroyRune()
    {
        _tutorialText.faceColor = colorSucsess;
        _tutorialText.outlineColor = colorSucsessOutline;
        yield return new WaitForSeconds(2);
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _tutorialText.text = "Destroy the rune to close the portal and stop the enemies from appearing";
        _objective = objective6;
        _tutorialText.GetComponent<TextTutorial>().target = objective6;
        _tutorialText.GetComponent<TextTutorial>().plusPos = -objective6.transform.forward + new Vector3(0,1.5f,0);
        var rune = FindObjectOfType<Portal_Rune>();

        var root2 = objective6.GetComponent<Roots>();
        root2.tutoRoot = false;

        while(!root2.burned) yield return new WaitForEndOfFrame();

        _objective = objective7;
        _tutorialText.GetComponent<TextTutorial>().target = objective7;
        _tutorialText.GetComponent<TextTutorial>().plusPos = -objective7.transform.forward;

        while (!rune.portalOff) yield return new WaitForEndOfFrame();

        _tutorialText.faceColor = colorSucsess;
        _tutorialText.outlineColor = colorSucsessOutline;

        yield return new WaitForSeconds(2);

        _objective = objective8;
        _tutorialText.GetComponent<TextTutorial>().target = objective8;
        _tutorialText.GetComponent<TextTutorial>().plusPos = Vector3.zero;
        _tutorialText.text = "Go through the tunnel";

    }

    public void PlusDodge()
    {
        if(dodgeCounts < dodgeCountsMax)
        {
            dodgeCounts++;
            _tutorialText.faceColor = colorSucsess;
            _tutorialText.outlineColor = colorSucsessOutline;
        }
    }


    IEnumerator MageInmortal()
    {
        while(dodgeCounts < dodgeCountsMax)
        {
            mage1.life = mage1.maxLife;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Destructibles()
    {
        yield return new WaitForSeconds(2);
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _tutorialText.GetComponent<TextTutorial>().target = objective3;
        _tutorialText.GetComponent<TextTutorial>().plusPos = new Vector3(0, 2.5f, 0);
        _tutorialText.text = "You can destroy objects on the level to recover life and gain energy to use the Fire Sword";
        _objective = objective3;
        var destructible = objective3.GetComponent<DestructibleOBJ>();
        while(!destructible.destroyed)
        {
            yield return new WaitForEndOfFrame();
        }
        _tutorialText.faceColor = colorSucsess;
        _tutorialText.outlineColor = colorSucsessOutline;
        yield return new WaitForSeconds(2);
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        _tutorialText.text = "Charge Fire Energy and use the Fire Sword to burn the Roots";
        _objective = objective4;
        _tutorialText.GetComponent<TextTutorial>().target = objective4;
        _tutorialText.GetComponent<TextTutorial>().plusPos = new Vector3(1, 1, 0.5f);
        var root = objective4.GetComponent<Roots>();
        root.tutoRoot = false;
        while(!root.burned)
        {
            _player.fireSwordCurrentTime = 1;
            yield return new WaitForEndOfFrame();
        }
        _tutorialText.faceColor = colorSucsess;
        _tutorialText.outlineColor = colorSucsessOutline;
        StartCoroutine(SecondEnemies());
    }

    IEnumerator CheckPointTimer()
    {
        yield return new WaitForSeconds(7);
        StartCoroutine(Destructibles());
    }

    void PlusCheckpoint()
    {
        if (!checkpoint && fireSword)
        {
            var d = Vector3.Distance(_player.transform.position, objective2.position);
            if (d < 1.5f)
            {
                _tutorialText.faceColor = colorSucsess;
                _tutorialText.outlineColor = colorSucsessOutline;
                checkpoint = true;
                StartCoroutine(CheckPointTimer());
            }
        }
    }

    void PlusHeavyAttack()
    {
        if (heavyAttackCounts < heavyAttackCountsMax && moveDefenceAmount >= moveDefenceAmountMax)
        {
            heavyAttackCounts++;
            _tutorialText.text = "Hold Left Click to make an area attack " + heavyAttackCounts + "/" + heavyAttackCountsMax;
            if (heavyAttackCounts >= heavyAttackCountsMax)
            {
                _tutorialText.faceColor = colorSucsess;
                _tutorialText.outlineColor = colorSucsessOutline;
                StartCoroutine(FlamesOn());
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
                _tutorialText.faceColor = colorSucsess;
                _tutorialText.outlineColor = colorSucsessOutline;
                StartCoroutine(ChangeText(3));
                _player.chargeAttackCasted = false;
            }
        }
    }

    void PlusKicks()
    {
        if (quickDefendCounts < defendCountsMax)
        {
            quickDefendCounts++;
            _tutorialText.text = "Use Right Click quickly when enemies attack you to kick them " + quickDefendCounts + "/" + quickDefendCountsMax;
            if (quickDefendCounts >= defendCountsMax)
            {
                canMove = true;
                _tutorialText.faceColor = colorSucsess;
                _tutorialText.outlineColor = colorSucsessOutline;
                _player.WalkEvent += PlusDefenceMove;
                StartCoroutine(ChangeText(2));
            }
        }
    }

    void PlusDefendCounts()
    {
        if (defendCounts < defendCountsMax)
        {
            defendCounts++;
            _tutorialText.text = "Use Right Click to Defend " + defendCounts + "/" + defendCountsMax;
            if (defendCounts >= defendCountsMax)
            {
                defendTutorialFinish = true;
                _tutorialText.faceColor = colorSucsess;
                _tutorialText.outlineColor = colorSucsessOutline;
                StartCoroutine(ChangeText(1));
            }
        }
    }

    void PlusHitCounts()
    {
        if (hitCounts < hitCountsMax)
        {
            hitCounts++;
            _tutorialText.text = "Use Left Click to Hit enemies " + hitCounts + "/9";
            if (hitCounts >= hitCountsMax)
            {
                _tutorialText.faceColor = colorSucsess;
                _tutorialText.outlineColor = colorSucsessOutline;
                attacksTutorialFinish = true;
                StartCoroutine(DefendTutorial());            
            }
        }
    }

    IEnumerator HeavyAttack()
    {
        while (!canMove)
        {
            _player.chargeAttackAmount = 0;
            _player.fireEnergy = 0;
            _view.powerBar.fillAmount = 0;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ChangeText(int id)
    {
        _tutorialText.faceColor = colorSucsess;
        _tutorialText.outlineColor = colorSucsessOutline;
        yield return new WaitForSeconds(2);
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        if (id ==1) _tutorialText.text = "Use Right Click quick when enemies attack you to kick them " + quickDefendCounts + "/" + quickDefendCountsMax;
        if (id ==2) _tutorialText.text = "You can Defend while moving";
        if (id ==3) _tutorialText.text = "Hold Left Click to make an area attack " + heavyAttackCounts + "/" + heavyAttackCountsMax;
    }

    IEnumerator DefendTutorial()
    {
        yield return new WaitForSeconds(2f);
        _tutorialText.text = "Use Right Click to Defend " + defendCounts + "/" + defendCountsMax;
        _tutorialText.faceColor = colorIncomplite;
        _tutorialText.outlineColor = colorIncompliteOutline;
        while(!defendTutorialFinish)
        {
            _player.defenceTimer = 1;
            yield return new WaitForEndOfFrame();
        }
    }  
}
