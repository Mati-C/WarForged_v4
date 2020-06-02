﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Model_E_Melee : ClassEnemy
{
    [Header("EnemyRoom Distances and Angles:")]
    public float viewDistancePersuit;
    public float angleToPersuit;
    public float viewDistanceSurround;
    public float angleToSurround;

    public Action IdleEvent;
    public Action WalkEvent;
    public Action RunEvent;
    public Action RetreatEvent;
    public Action WalkLeftEvent;
    public Action WalkRightEvent;
    public Action ComboAttackEvent;
    public Action HeavyAttackEvent;

    Viewer_E_Melee _view;

    [Header("Enemy Surround Variables:")]

    public float surroundSpeed;
    public float surroundTimer;
    public float surroundTimerMin;
    public float surroundTimerMax;
    public int surroundBehaviourID;
    public bool avoidFriends;

    [Header("Enemy Attack Variables:")]

    public int ID_Attack;

    [Range(10, 100)]
    public int singleAttackProbability;

    public float timeToAttack;
    public float maxTimeToAttack;
    public float minTimeToAttack;
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

  

    IEnumerator OnAttackAnimationCorrutine(float time)
    {
        onAttackAnimation = true;
        yield return new WaitForSeconds(time);
        onAttackAnimation = false;
        if(onDamageTime <=0 )attackFinish = true;
    }

    void Start()
    {
        _view = GetComponent<Viewer_E_Melee>();
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var takeDamage = new N_FSM_State("TAKE_DAMAGE");

        IdleEvent += _view.AnimIdleCombat;
        WalkEvent += _view.AnimWalkCombat;
        RunEvent += _view.AnimRunCombat;
        RetreatEvent += _view.AnimRetreat;
        WalkRightEvent += _view.AnimWalkRight;
        WalkLeftEvent += _view.AnimWalkLeft;
        ComboAttackEvent += _view.AnimComboAttack;
        HeavyAttackEvent += _view.AnimHeavyAttack;
        GetHitEvent += _view.AnimGetHit;    

        StartCoroutine(MoveOnAttack());
        StartCoroutine(OnDamageTimer());

        patrol.OnUpdate += () =>
        {
            if (canPersuit) myFSM_EventMachine.ChangeState(persuit);
        };

        persuit.OnUpdate += () =>
        {

            WalkEvent();
            player.CombatStateUp();

            if (aggressiveLevel == 1) MoveToTarget(player.transform);

            else MoveToTarget(FindNearNon_AggressiveNode().transform);
           
            if (canSurround) myFSM_EventMachine.ChangeState(surround);

            if(onDamageTime >0) myFSM_EventMachine.ChangeState(takeDamage);
        };

        persuit.OnExit += () =>
        {
            if (aggressiveLevel == 1) viewDistanceSurround = 3.5f;

            if (aggressiveLevel == 2) viewDistanceSurround = 7f;
        };

        surround.OnEnter += () =>
        {
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

            timeToAttack -= Time.deltaTime;

            if(surroundBehaviourID == 0)
            {
                IdleEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
            }

            if (surroundBehaviourID == 1)
            {
                WalkRightEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
                rb.MovePosition(rb.position + transform.right * surroundSpeed * Time.deltaTime);
            }

            if (surroundBehaviourID == 2)
            {
                WalkLeftEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
                rb.MovePosition(rb.position - transform.right * surroundSpeed * Time.deltaTime);

            }

            if(surroundTimer <=0)
            {
                surroundBehaviourID = UnityEngine.Random.Range(0, 3);

                surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);
            }

            if (!canSurround && canPersuit && timeToAttack >0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack <=0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0) myFSM_EventMachine.ChangeState(takeDamage);
        };

        surround.OnExit += () =>
        {
            
        };

        attack.OnEnter += () =>
        {
            int r = UnityEngine.Random.Range(1, 101);

            if (r < singleAttackProbability) ID_Attack = 1;
            else ID_Attack = 2;
        };

        attack.OnUpdate += () =>
        {
            player.CombatStateUp();

            if (!onAttackAnimation && !canAttack)
            {
                RunEvent();
                MoveToTarget(player.transform);
            }

            if(canAttack && !onAttackAnimation && !waitingForRetreat)
            {

                waitingForRetreat = true;

                if (ID_Attack == 1) ComboAttackEvent();

                else HeavyAttackEvent();

                var dir = Vector3.zero;
                dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                transform.forward = dir;
                if (ID_Attack == 1) StartCoroutine(OnAttackAnimationCorrutine(2));

                else StartCoroutine(OnAttackAnimationCorrutine(1.2f));
            }

            if(attackFinish) myFSM_EventMachine.ChangeState(retreat);

            if (onDamageTime > 0) myFSM_EventMachine.ChangeState(takeDamage);
        };

        attack.OnExit += () =>
        {
            if (timeToAttack <= 0)
            {
                timeToAttack = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);
                if (aggressiveLevel == 2) timeToAttack += 1;
            }

            surroundBehaviourID = UnityEngine.Random.Range(0, 2);

            surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);

            attackFinish = false;
        };

        retreat.OnEnter += () =>
        {
            attackFinish = false;
            timeToRetreat = maxTimeToRetreat;
            waitingForRetreat = false;
            if (aggressiveLevel == 2) timeToRetreat += 1;
        };

        retreat.OnUpdate += () =>
        {
            timeToRetreat -= Time.deltaTime;

            if (timeToRetreat >0)
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

            if (timeToRetreat <=0 && canPersuit) myFSM_EventMachine.ChangeState(persuit);

            if (timeToRetreat <=0 && canSurround) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0) myFSM_EventMachine.ChangeState(takeDamage);
        };

        takeDamage.OnEnter += () =>
        {                        
            timeToRetreat = 0;
        };

        takeDamage.OnUpdate += () =>
        {
            waitingForRetreat = false;
            attackFinish = false;

            if (canPersuit && !canSurround && onDamageTime <= 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 &&  canSurround &&  onDamageTime <= 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && onDamageTime <=0) myFSM_EventMachine.ChangeState(attack);
        };

        takeDamage.OnExit += () =>
        {
            timeToRetreat = 0;
        };

        myFSM_EventMachine = new N_FSM_EventMachine(patrol);
    }

    
    void Update()
    {
       
        canPersuit = CanSee(player.transform, viewDistancePersuit, angleToPersuit, layersCanSee);

        canSurround = CanSee(player.transform, viewDistanceSurround, angleToSurround, layersCanSee);

        canAttack = CanSee(player.transform, viewDistanceAttack, angleToAttack, layersCanSee);

        myFSM_EventMachine.Update();
    }

    public void PlusAttackMove(float t) { _timeToMoveOnAttack = t; }


    IEnumerator MoveOnAttack()
    {
        while (true)
        {
            if(_timeToMoveOnAttack >0)
            {
                rb.MovePosition(transform.position + transform.forward * 2 * Time.deltaTime);
            }

            _timeToMoveOnAttack -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
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
