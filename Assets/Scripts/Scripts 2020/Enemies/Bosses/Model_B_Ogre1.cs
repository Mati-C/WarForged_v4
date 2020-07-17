using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public class Model_B_Ogre1 : ClassEnemy
{
    Viewerl_B_Ogre1 _view;
    public CinematicController CC;

    [Header("Boss Faces:")]
    public bool fase1;
    public bool fase2;
    public bool fase3;
    [Range(0, 100)]
    public float lightAttackCoef;
    [Range(0, 100)]
    public float HeavyAttackCoef;
    [Range(0, 100)]
    public float ComboAttackCoef;

    [Header("Boss Attack Variables:")]
    public float heavyAttackDamage;
    public float lightAttackDamage;
    public float combo1AttackDamage;
    public float combo2AttackDamage;
    public float combo3AttackDamage;
    public float angleToAttack;
    public float viewDistanceAttack;
    public bool canAttack;
    public bool onAttackAnimation;
    public bool enemyWavesStrategy;
    public bool attackFinish;
    public int attackID;
    public int enemyWavesCount;
    int _comboAmount;
    public List<ClassEnemy> wave1 = new List<ClassEnemy>();
    public List<ClassEnemy> wave2 = new List<ClassEnemy>();
    public List<Transform> phWave1 = new List<Transform>();
    public List<Transform> phWave2 = new List<Transform>();

    [Header("Boss Surround Variables:")]

    public float surroundSpeed;
    public float surroundTimer;
    public float surroundTimerMin;
    public float surroundTimerMax;

    [Header("Boss Portal Variables:")]
    public Transform phPortal;
    public bool canScape;
    bool _heavyEnd;
    public Vector3 phScape;
    bool _playerDead;

    public Action IdleEvent;
    public Action WalkEvent;
    public Action WalkRightEvent;
    public Action WalkLeftEvent;
    public Action TauntEvent;
    public Action LightAttackEvent;
    public Action ComboAttackEvent;
    public Action HeavyAttackEvent;

    bool _onTaunt;

    IEnumerator TauntCorrutine()
    {
        _onTaunt = true;
        yield return new WaitForSeconds(2f);
        _onTaunt = false;
    }

    IEnumerator OnAttackAnimationCorrutine(float time)
    {
        onAttackAnimation = true;
        yield return new WaitForSeconds(time);
        onAttackAnimation = false;
        _comboAmount = 0;
        attackFinish = true;
    }

    IEnumerator HeavyAttackScape()
    {
        yield return new WaitForSeconds(2.3f);
        HeavyAttackEvent();
        yield return new WaitForSeconds(1);
        _heavyEnd = true;
    }

    IEnumerator WavesStart()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < wave1.Count; i++)
        {
            wave1[i].dontMove = true;
            wave1[i].gameObject.SetActive(false);
            wave1[i].transform.position = phWave1[i].position;
        }

        for (int i = 0; i < wave2.Count; i++)
        {
            wave2[i].dontMove = true;
            wave2[i].gameObject.SetActive(false);
            wave2[i].transform.position = phWave2[i].position;
        }
    }


    void Start()
    {
        if (enemyWavesStrategy) StartCoroutine(WavesStart());
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
        _view = GetComponent<Viewerl_B_Ogre1>();
        playerFireSowrd = FindObjectOfType<FireSword>();
        exp = playerFireSowrd.BossOgre1Exp;
        phScape = transform.position;
        boss = true;

        var attack = new N_FSM_State("ATTACK");
        var surround = new N_FSM_State("SURROUND");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var scape = new N_FSM_State("SCAPE");
        var die = new N_FSM_State("DIE");

        patrolState = patrol;
        IdleEvent += _view.AnimIdle;
        WalkEvent += _view.AnimWalk;
        WalkLeftEvent += _view.animWalkLeft;
        WalkRightEvent += _view.animWalkRight;
        TauntEvent += _view.AnimTaunt;
        GetHitEvent += _view.AnimGetHit;
        LightAttackEvent += _view.AnimLightAttack;
        HeavyAttackEvent += _view.AnimHeavyAttack;
        ComboAttackEvent += _view.AnimComboAttack;
        DieEvent += _view.AnimDie;


        scape.OnEnter += () =>
        {
            _view.anim.SetBool("HeavyAttack", false);
            _view.anim.SetBool("LightAttack", false);
            _view.anim.SetBool("ComboAttack", false);
            TauntEvent();
            _view.healthBar.SetActive(false);
            StartCoroutine(TauntCorrutine());
            StartCoroutine(HeavyAttackScape());
        };

        scape.OnUpdate += () =>
        {
            if(!_onTaunt && _heavyEnd)
            {
                WalkEvent();
                MoveToTarget(phScape);
            }
            viewDistancePersuit = 0;
        };

        patrol.OnEnter += () =>
        {

        };

        patrol.OnUpdate += () =>
        {
            if (portalOrder && !dontMove)
            {
                float d = Vector3.Distance(transform.position, phPortal.position);

                if (d > 1.2f)
                {
                    WalkEvent();
                    MoveToTarget(phPortal.position);
                }

                else
                {
                    TauntEvent();
                    viewDistancePersuit = 100;
                    StartCoroutine(TauntCorrutine());
                    portalOrder= false;
                }
            }

            if(canPersuit && !portalOrder && !_onTaunt && player.life>0 && !dontMove) myFSM_EventMachine.ChangeState(persuit);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        patrol.OnExit += () =>
        {
            var dir = (player.transform.position - transform.position).normalized;
            dir.y = 0;
            transform.forward = dir;
        };

        persuit.OnEnter += () =>
        {
            
        };

        persuit.OnUpdate += () =>
        {
            var d = Vector3.Distance(player.transform.position, transform.position);

            if (d < 5 && !_view.onSmashAttack) playerCamera.CameraShake(0.5f, 0.5f);
            if (d > 5 && !_view.onSmashAttack) playerCamera.CameraShake(0, 0);

            player.CombatStateUp(false);
            if (!_onTaunt)
            {
                WalkEvent();
                MoveToTarget(player.transform.position);
            }

            if(canSurround) myFSM_EventMachine.ChangeState(surround);

            if(canScape) myFSM_EventMachine.ChangeState(scape);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        persuit.OnExit += () =>
        {

        };

        surround.OnEnter += () =>
        {
            viewDistanceSurround += 1;

            if (timeToAttack <= 0) timeToAttack = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);

        };

        surround.OnUpdate += () =>
        {
            var d = Vector3.Distance(player.transform.position, transform.position);
           
            player.CombatStateUp(false);

            surroundTimer -= Time.deltaTime;
           
            var obs = Physics.OverlapSphere(transform.position, 1, layersObstacles);

            timeToAttack -= Time.deltaTime;

            if (surroundBehaviourID == 0 && !_onTaunt)
            {
                IdleEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
            }

            if (surroundBehaviourID == 1 && !_onTaunt)
            {
                if (NearEnemy() && !cantAvoid) StartCoroutine(AvoidNearEntity());
                WalkRightEvent();
                if (d < 5 && !_view.onSmashAttack) playerCamera.CameraShake(0.5f, 0.5f);
                if (d > 5 && !_view.onSmashAttack) playerCamera.CameraShake(0, 0);
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
                rb.MovePosition(rb.position + transform.right * surroundSpeed * Time.deltaTime);
                if (obs.Count() > 0) rb.MovePosition(transform.position + transform.forward * 2 * Time.deltaTime);
            }

            if (surroundBehaviourID == 2 && !_onTaunt)
            {
                if (NearEnemy() && !cantAvoid) StartCoroutine(AvoidNearEntity());
                WalkLeftEvent();
                if (d < 5 && !_view.onSmashAttack) playerCamera.CameraShake(0.5f, 0.5f);
                if (d > 5 && !_view.onSmashAttack) playerCamera.CameraShake(0, 0);
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
                rb.MovePosition(rb.position - transform.right * surroundSpeed * Time.deltaTime);
                if (obs.Count() > 0) rb.MovePosition(transform.position + transform.forward * 2 * Time.deltaTime);

            }

            if (surroundTimer <= 0 && !_onTaunt)
            {
                surroundBehaviourID = UnityEngine.Random.Range(0, 3);

                surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);
            }

            if(timeToAttack <=0 && !_onTaunt) myFSM_EventMachine.ChangeState(attack);

            if(canPersuit && !canSurround && !_onTaunt) myFSM_EventMachine.ChangeState(persuit);

            if (canScape) myFSM_EventMachine.ChangeState(scape);

            if (_playerDead) myFSM_EventMachine.ChangeState(patrol);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        surround.OnExit += () =>
        {
            surroundTimer = 1f;
            viewDistanceSurround -= 1;
            if (!_view.onSmashAttack) playerCamera.CameraShake(0, 0);
        };

        attack.OnEnter += () =>
        {
            attackID = StartAttackStrategy();
        };

        attack.OnUpdate += () =>
        {

            player.CombatStateUp(false);

            var d = Vector3.Distance(player.transform.position, transform.position);

            

            if (!canAttack && !onAttackAnimation && !_onTaunt && !attackFinish)
            {
                if (d > 0.5f) WalkEvent();

                if (d < 5 && !_view.onSmashAttack) playerCamera.CameraShake(0.5f, 0.5f);
                if (d > 5 && !_view.onSmashAttack) playerCamera.CameraShake(0, 0);

                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

                MoveToTarget(player.transform.position);
            }

            if (canAttack && !onAttackAnimation && !attackFinish && !_onTaunt)
            {
                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

                if (attackID == 0)
                {
                    LightAttackEvent();
                    StartCoroutine(OnAttackAnimationCorrutine(1.2f));
                }

                if (attackID == 1)
                {
                    HeavyAttackEvent();
                    StartCoroutine(OnAttackAnimationCorrutine(1.3f));
                }

                if (attackID == 2)
                {
                    ComboAttackEvent();
                    StartCoroutine(MoveOnAttack());
                    StartCoroutine(OnAttackAnimationCorrutine(2.3f));
                }
            }

            if(canPersuit && !canSurround && attackFinish) myFSM_EventMachine.ChangeState(persuit);

            if(canSurround && attackFinish) myFSM_EventMachine.ChangeState(surround);

            if (canScape) myFSM_EventMachine.ChangeState(scape);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        attack.OnExit += () =>
        {
            attackFinish = false;
            if (!_view.onSmashAttack) playerCamera.CameraShake(0, 0);
        };

        myFSM_EventMachine = new N_FSM_EventMachine(patrol);

        
    }

    
    void Update()
    {
        canPersuit = CanSee(player.transform, viewDistancePersuit, angleToPersuit, layersCanSee);

        canSurround = CanSee(player.transform, viewDistanceSurround, angleToSurround, layersCanSee);

        canAttack = CanSee(player.transform, viewDistanceAttack, angleToAttack, layersCanSee);

        if (!_playerDead && player.life <= 0 && life >0) StartCoroutine(StartRespawn());

        myFSM_EventMachine.Update();
    }

    IEnumerator StartRespawn()
    {
        _playerDead = true;
        yield return new WaitForSeconds(5f);
        transform.position = phScape;
        dontMove = true;
        portalOrder = true;
        PatrolState();
        _playerDead = false;
        life = maxLife;
        _view.healthBar.gameObject.SetActive(false);
        fase2 = false;
        fase3 = false;
        lightAttackCoef = 60;
        HeavyAttackCoef = 0;
        ComboAttackCoef = 0;
        if (!CC.cinematicLevel2)
        {
            CC.barsAnimator.SetBool("Activate", false);
            CC.cinematicLevel2 = true;

            foreach (var item in wave2)
            {
                item.gameObject.SetActive(false);
                item.ReturnToLife();
                item.Resume();
                item.dontMove = true;
                item.onPatrol = true;
                item.PatrolState();
                item.RestartDistances_Angles();
                item.enemyLayer = layersCanSee;
            }

            foreach (var item in wave1)
            {
                item.gameObject.SetActive(false);
                item.ReturnToLife();
                item.Resume();
                item.dontMove = true;
                item.onPatrol = true;
                item.PatrolState();
                item.RestartDistances_Angles();
                item.enemyLayer = layersCanSee;
            }

            StartCoroutine(WavesStart());
        }

        if(!CC.cinematicLevel1)
        {
            CC.barsAnimator.SetBool("Activate", false);
            CC.cinematicLevel1 = true;
        }
        SoundManager.instance.BossMusic(false);
    }

    int StartAttackStrategy()
    {
        var values = new List<float>();

        values.Add(lightAttackCoef);
        values.Add(HeavyAttackCoef);
        values.Add(ComboAttackCoef);

        var coefList = new List<float>();

        var sum = values.Sum();

        foreach (var item in values)
        {
            coefList.Add(item / sum);
        }

        int rndPercent = UnityEngine.Random.Range(0, 100);
        float r = rndPercent / 100f;

        float sumCoef = 0;

        for (int i = 0; i < values.Count; i++)
        {
            sumCoef += coefList[i];

            if (sumCoef > r)
            {
                return i;
            }
        }

        return -1;
    }

    IEnumerator MoveOnAttack()
    {
        SoundManager.instance.Play(Boss.MONSTER_ATTACK1, transform.position, true, 1.3f);
        yield return new WaitForSeconds(1);
        SoundManager.instance.Play(Boss.MONSTER_ATTACK2, transform.position, true, 1.3f);
        rb.AddForce(transform.forward * 200, ForceMode.Impulse);
        yield return new WaitForSeconds(0.8f);       
        rb.AddForce(transform.forward * 200, ForceMode.Impulse);
        yield return new WaitForSeconds(0.8f);
        rb.AddForce(transform.forward * 200, ForceMode.Impulse);
    }

    public void MakeHeavyAttack()
    {
        var p = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<Model_Player>());

        if (p.Count() > 0) player.GetDamage(heavyAttackDamage, transform, Model_Player.DamageType.Heavy);
    }

    public void MakeLightDamage()
    {
        if(canAttack) player.GetDamage(lightAttackDamage, transform, Model_Player.DamageType.Heavy);
    }

    public void MakeComboDamage()
    {
        if (canAttack)
        {
            switch (_comboAmount)
            {
                case 0:
                    _comboAmount++;
                    player.GetDamage(combo1AttackDamage, transform, Model_Player.DamageType.Heavy);                    
                    break;

                case 1:
                    _comboAmount++;
                    player.GetDamage(combo2AttackDamage, transform, Model_Player.DamageType.Heavy);
                    break;

                case 2:
                    _comboAmount = 0;
                    player.GetDamage(combo3AttackDamage, transform, Model_Player.DamageType.Heavy);                   
                    break;
            }
        }
    }

    public override void GetDamage(float d, Model_Player.DamageType t)
    {

        if (!_onTaunt && life >0)
        {
            GetHitEvent();
            life -= d;
            _view.CreatePopText(d);
            if (player.flamesOn) StartBurning();

            if (life <= 0)
            {
                _view.CreateExpPopText(exp);
                playerFireSowrd.SwordExp(exp);
                DieEvent();
                if(!CC.cinematicLevel2) CC.barsAnimator.SetBool("Activate", false);
                SoundManager.instance.Play(Objects.IRON_BARS, CC.barsAnimator.bodyPosition, true, 1);
                _view.healthBar.gameObject.SetActive(false);
            }
        }

        else player.FailAttack("");

        if(fase1 && life<=130 && !fase2)
        {
            TauntEvent();
            StartCoroutine(TauntCorrutine());
            fase2 = true;
            lightAttackCoef = 30;
            HeavyAttackCoef = 45;
            foreach (var item in wave1)
            {
                item.gameObject.SetActive(true);
                item.Resume();
                item.dontMove = false;
            }
        }

        if(fase2 && life <=80 && !fase3)
        {
            TauntEvent();
            StartCoroutine(TauntCorrutine());
            fase3 = true;
            lightAttackCoef = 20;
            HeavyAttackCoef = 45;
            ComboAttackCoef = 55;
            foreach (var item in wave2)
            {
                item.gameObject.SetActive(true);
                item.Resume();
                item.dontMove = false;
            }
        }

        
    }

    public override void Resume()
    {

    }

    public override void PatrolState()
    {
        myFSM_EventMachine.ChangeState(patrolState);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistancePersuit);

        Vector3 rightLimit = Quaternion.AngleAxis(angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistancePersuit));

        Vector3 leftLimit = Quaternion.AngleAxis(-angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistancePersuit));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewDistanceSurround);

        Vector3 rightLimit2 = Quaternion.AngleAxis(angleToSurround, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceSurround));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-angleToSurround, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceSurround));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit3 = Quaternion.AngleAxis(angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit3 * viewDistanceAttack));

        Vector3 leftLimit3 = Quaternion.AngleAxis(-angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit3 * viewDistanceAttack));

    }
}
