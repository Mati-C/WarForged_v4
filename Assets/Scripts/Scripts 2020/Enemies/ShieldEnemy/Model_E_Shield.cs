﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Model_E_Shield : ClassEnemy
{

    public Action IdleEvent;
    public Action WalkEvent;
    public Action RunEvent;
    public Action RetreatEvent;
    public Action WalkLeftEvent;
    public Action WalkRightEvent;
    public Action AttackEvent;
    public Action ParryEvent;

    Viewer_E_Shield _view;

    [Header("Enemy Surround Variables:")]

    public float surroundSpeed;
    public float surroundTimer;
    public float surroundTimerMin;
    public float surroundTimerMax;

    [Header("EnemyShield Attack Variables:")]

    public float attackDamage;
    public float angleToAttack;
    public float viewDistanceAttack;
    public bool canAttack;
    public bool onAttackAnimation;
    public bool attackFinish;
    float _timeToMoveOnAttack;

    [Header("Enemy Retreat Variables:")]

    public float timeToRetreat;
    public float maxTimeToRetreat;
    public float retreatSpeed;
    public bool waitingForRetreat;

    [Header("Enemy Defend Variables:")]

    public bool defenceBroken;
    public bool onParry;
    public float timeOnParry;
    public float timeOnParryMax;

    [Header("Enemy FSM States:")]
    public bool Spersuit;
    public bool Ssurround;
    public bool Sattack;
    public bool Sretraet;
    public bool Stakedamage;
    public bool Sblocked;
    public bool Sparry;

    IEnumerator DefenceBrokenTimer()
    {
        defenceBroken = true;
        yield return new WaitForSeconds(5f);
        defenceBroken = false;
    }

    public IEnumerator OnParryTimer()
    {
        while (true)
        {
            if (timeOnParry > 0)
            {
                timeOnParry -= Time.deltaTime;
                onParry = true;
            }

            if (timeOnParry <= 0) onParry = false;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator OnAttackAnimationCorrutine(float time)
    {
        onAttackAnimation = true;
        yield return new WaitForSeconds(time);
        onAttackAnimation = false;
        if (onDamageTime <= 0) attackFinish = true;
    }

    void Start()
    {
        melee = true;
        _view = GetComponent<Viewer_E_Shield>();
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
        ia_Manager = FindObjectOfType<IA_CombatManager>();
        playerFireSowrd = FindObjectOfType<FireSword>();
        exp = playerFireSowrd.shieldExp;
        enemyLayer = layersCanSee;

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var takeDamage = new N_FSM_State("TAKE_DAMAGE");
        var die = new N_FSM_State("DIE");
        var blocked = new N_FSM_State("BLOCKED");
        var parry = new N_FSM_State("Parry");

        patrolState = patrol;
        IdleEvent += _view.AnimIdleCombat;
        WalkEvent += _view.AnimWalkCombat;
        RunEvent += _view.AnimRunCombat;
        RetreatEvent += _view.AnimRetreat;
        WalkRightEvent += _view.AnimWalkRight;
        WalkLeftEvent += _view.AnimWalkLeft;
        AttackEvent += _view.AnimComboAttack;
        GetHitEvent += _view.AnimGetHit;
        BlockedEvent += _view.BlockedAnim;
        KnockedEvent += _view.KnockedAnim;
        KnockedEvent += DefenceBrokenStart;
        DieEvent += _view.AnimDie;
        ParryEvent += _view.AnimParry;

        StartCoroutine(MoveOnAttack());
        StartCoroutine(OnDamageTimer());
        StartCoroutine(OnParryTimer());

        patrol.OnEnter += () =>
        {
            onPatrol = true;
            RestartDistances_Angles();
            enemyLayer = layersCanSee;
        };

        patrol.OnUpdate += () =>
        {
            var distancePH_patrol = Vector3.Distance(transform.position, patrolPosition);
            var distacePlayer = Vector3.Distance(transform.position, player.transform.position);

            if (!dontMove)
            {

                if (portal)
                {

                    if (distancePH_patrol > 0.6f && portalOrder)
                    {
                        WalkEvent();
                        MoveToTarget(patrolPosition);
                    }

                    if (distancePH_patrol <= 1.2f && portalOrder)
                    {
                        portalOrder = false;
                        IdleEvent();
                        Quaternion targetRotation = Quaternion.LookRotation(patrolForward, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
                    }
                }

                if (distancePH_patrol > 0.6f && !portalOrder)
                {
                    WalkEvent();
                    MoveToTarget(patrolPosition);
                }

                if (distancePH_patrol <= 1.2f && !portalOrder)
                {
                    IdleEvent();
                    Quaternion targetRotation = Quaternion.LookRotation(patrolForward, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
                }
            }

            if (canPersuit && !PlayerOnGrid() || distacePlayer<=1.5f && !PlayerOnGrid()) myFSM_EventMachine.ChangeState(persuit);
        };

        patrol.OnExit += () =>
        {
            viewDistancePersuit = 100;
            angleToPersuit = 360;
            angleToSurround = 360;

            foreach (var item in sameID_Enemies)
            {
                item.enemyLayer = layersPlayer;
                item.viewDistancePersuit = 100;
                item.angleToPersuit = 360;
                item.angleToSurround = 360;
            }
            onPatrol = false;
            enemyLayer = layersPlayer;
        };

        persuit.OnUpdate += () =>
        {
            Spersuit = true;
            WalkEvent();
            player.CombatStateUp();

            if (aggressiveLevel == 1) MoveToTarget(FindNearAggressiveNode().transform.position);

            else MoveToTarget(FindNearNon_AggressiveNode().transform.position);

            if(PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        persuit.OnExit += () =>
        {
            Spersuit = false;   
        };

        surround.OnEnter += () =>
        {
            Ssurround = true;
            surroundBehaviourID = UnityEngine.Random.Range(0, 2);

            surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);

            if (timeToAttack <= 0)
            {
                timeToAttack = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);
                if (aggressiveLevel == 2) timeToAttack += 1;
            }

        };

        surround.OnUpdate += () =>
        {
            if (aggressiveLevel == 1) viewDistanceSurround = 5f;

            if (aggressiveLevel == 2) viewDistanceSurround = 8f;

            player.CombatStateUp();

            surroundTimer -= Time.deltaTime;

            if (!ia_Manager.enemyMeleePermisionAttack && !cantAskAgain) timeToAttack -= Time.deltaTime;

            var obs = Physics.OverlapSphere(transform.position, 1, layersObstacles);
            
            if (surroundBehaviourID == 0)
            {
                IdleEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
            }

            if (surroundBehaviourID == 1)
            {
                if (NearEnemy() && !cantAvoid) StartCoroutine(AvoidNearEntity());
                WalkRightEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
                rb.MovePosition(rb.position + transform.right * surroundSpeed * Time.deltaTime);
                if (obs.Count() > 0) rb.MovePosition(transform.position + transform.forward * 2 * Time.deltaTime);
            }

            if (surroundBehaviourID == 2)
            {
                if (NearEnemy() && !cantAvoid) StartCoroutine(AvoidNearEntity());
                WalkLeftEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
                rb.MovePosition(rb.position - transform.right * surroundSpeed * Time.deltaTime);
                if (obs.Count() > 0) rb.MovePosition(transform.position + transform.forward * 2 * Time.deltaTime);

            }

            if (surroundTimer <= 0)
            {
                surroundBehaviourID = UnityEngine.Random.Range(0, 3);

                surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);
            }

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (!canSurround && canPersuit && timeToAttack > 0 && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack <= 0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        surround.OnExit += () =>
        {
            if (aggressiveLevel == 1) viewDistanceSurround = 4.5f;

            if (aggressiveLevel == 2) viewDistanceSurround = 7f;

            Ssurround = false;
        };

        attack.OnEnter += () =>
        {
            Sattack = true;
            if (!permissionToAttack && !ia_Manager.enemyMeleePermisionAttack && !ia_Manager.decisionOnAttackMelee) 
            {
                ia_Manager.PermissionsMelee(true);
                permissionToAttack = true;
            }

            if (!ia_Manager.decisionOnAttackMelee)
            {
                ia_Manager.SetOrderAttack(this);
                ia_Manager.DecisionTakeMelee(true);
            }
        };

        attack.OnUpdate += () =>
        {
            player.CombatStateUp();

            if (!onAttackAnimation && !canAttack && !waitingForRetreat)
            {                
                RunEvent();
                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
                MoveToTarget(player.transform.position);
            }

            if (canAttack && !onAttackAnimation && !waitingForRetreat)
            {

                waitingForRetreat = true;

                AttackEvent();

                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

                StartCoroutine(OnAttackAnimationCorrutine(0.9f));    
            }

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (attackFinish && life > 0) myFSM_EventMachine.ChangeState(retreat);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        attack.OnExit += () =>
        {
            Sattack = false;
            if (timeToAttack <= 0)
            {
                timeToAttack = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);
                if (aggressiveLevel == 2) timeToAttack += 1;
            }

            surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);

            if (attackFinish) StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission + 1.2f, true));

            else StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission, true));
        };

        retreat.OnEnter += () =>
        {
            Sretraet = true;
            attackFinish = false;
            timeToRetreat = maxTimeToRetreat;
            waitingForRetreat = false;
            if (aggressiveLevel == 2) timeToRetreat += 1;
        };

        retreat.OnUpdate += () =>
        {
            timeToRetreat -= Time.deltaTime;

            var d = Vector3.Distance(transform.position, player.transform.position);

            if (d > 2) timeToRetreat = 0;

            if (timeToRetreat > 0)
            {
                RetreatEvent();
                player.CombatStateUp();
                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
                rb.MovePosition(rb.position - _dir * retreatSpeed * Time.deltaTime);
            }

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (timeToRetreat <= 0 && canPersuit && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToRetreat <= 0 && canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        retreat.OnExit += () =>
        {
            timeToRetreat = 0;
            Sretraet = false;
        };

        takeDamage.OnEnter += () =>                           
        {
            timeToRetreat = 0;
            Stakedamage = true;
        };

        takeDamage.OnUpdate += () =>
        {
            waitingForRetreat = false;
            attackFinish = false;

            if (PlayerOnGrid() && timeToAttack >0) myFSM_EventMachine.ChangeState(patrol);

            if (canPersuit && !canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        takeDamage.OnExit += () =>
        {
            timeToRetreat = 0;
            Stakedamage = false;
        };

        blocked.OnEnter += () =>
        {
            timeToRetreat = 0;
            Sblocked = true;
        };

        blocked.OnUpdate += () =>
        {
            waitingForRetreat = false;
            attackFinish = false;

            if(PlayerOnGrid() && !blockedAttack) myFSM_EventMachine.ChangeState(patrol);

            if (canPersuit && !canSurround && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        blocked.OnExit += () =>
        {
            timeToRetreat = 0;
            Sblocked = false;
        };

        parry.OnEnter += () =>
        {
            timeToRetreat = 0;
        };

        parry.OnUpdate += () =>
        {
            Sparry = true;
            waitingForRetreat = false;
            attackFinish = false;

            player.CombatStateUp();
            Vector3 _dir = Vector3.zero;
            Quaternion targetRotation;
            _dir = (player.transform.position - transform.position).normalized;
            _dir.y = 0;
            targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

            if (PlayerOnGrid() && !onParry) myFSM_EventMachine.ChangeState(patrol);

            if (canPersuit && !canSurround && !onParry && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && !onParry && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && !onParry && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        parry.OnExit += () =>
        {
            Sparry = false;
            timeToRetreat = 0;
        };

        die.OnEnter += () =>
        {
            playerFireSowrd.SwordExp(exp);
        };

        myFSM_EventMachine = new N_FSM_EventMachine(patrol);
    }


    void Update()
    {

        canPersuit = CanSee(player.transform, viewDistancePersuit, angleToPersuit, enemyLayer);

        canSurround = CanSee(player.transform, viewDistanceSurround, angleToSurround, enemyLayer);

        canAttack = CanSee(player.transform, viewDistanceAttack, 360, enemyLayer);

        //notOnGrid = PlayerOnGrid();

        myFSM_EventMachine.Update();
    }

    public override void Resume()
    {
        StartCoroutine(OnDamageTimer());
        StartCoroutine(MoveOnAttack());
        _view.StartCoroutine(_view.DamageTimerAnim());
        StartCoroutine(OnParryTimer());
    }

    public override void PatrolState()
    {
        myFSM_EventMachine.ChangeState(patrolState);
    }

    public void PlusAttackMove(float t) { _timeToMoveOnAttack = t; }

    public void DefenceBrokenStart() { StartCoroutine(DefenceBrokenTimer()); }

    public override void GetDamage(float d, Model_Player.DamageType t)
    {
        Vector3 toTarget = (player.transform.position - transform.position).normalized;

        Vector3 fowardPlayer = (transform.position - player.transform.position).normalized;

        if (onAttackAnimation || t == Model_Player.DamageType.Heavy)
        {
            DefenceBrokenStart();
            defenceBroken = true;
        }

        if (defenceBroken || knocked)
        {
            life -= d;
            onDamageTime = damageDelayTime;
            _view.UpdateLifeBar(life / maxLife);

            if (player.flamesOn) StartBurning();

            if (life <= 0 && !isDead)
            {
                isDead = true;
                if (portal) portal.PortalRemove();
                RuturnIA_ManagerInstant(false);
                _view.CreateExpPopText(exp);
                playerFireSowrd.SwordExp(exp);               
                DieEvent();
            }

            else
            {
                _view.CreatePopText(d);
                GetHitEvent();
            }

            if (Vector3.Dot(toTarget, transform.forward) > 0)
            {
                if (t == Model_Player.DamageType.Light) rb.AddForce(-transform.forward * 2, ForceMode.Impulse);

                else rb.AddForce(-transform.forward * 5, ForceMode.Impulse);
            }

            else
            {
                if (t == Model_Player.DamageType.Light) rb.AddForce(transform.forward * 2, ForceMode.Impulse);

                else rb.AddForce(transform.forward * 5, ForceMode.Impulse);
            }
        }

        if(!defenceBroken && !knocked && player.CanSee(transform,player.viewDistanceAttack, 60, player.playerCanSee))
        {
            ParryEvent();
            player.FailAttack("Enemy");
            timeOnParry = timeOnParryMax;
            rb.AddForce(-transform.forward * 2, ForceMode.Impulse);
        }
         
    }

    IEnumerator MoveOnAttack()
    {
        while (true)
        {
            if (_timeToMoveOnAttack > 0)
            {
                rb.MovePosition(transform.position + transform.forward * 2 * Time.deltaTime);
            }

            _timeToMoveOnAttack -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

    public void MakeDamage()
    {
        if (life > 0)
        {
            var p = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<Model_Player>()).Select(x => x.GetComponent<Model_Player>());

            if (p.Count() > 0)
            {
                if (CanSee(p.First().transform, viewDistanceAttack, angleToAttack, layersCanSee)) p.First().GetDamage(attackDamage, transform, Model_Player.DamageType.Light);
            }
        }
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
