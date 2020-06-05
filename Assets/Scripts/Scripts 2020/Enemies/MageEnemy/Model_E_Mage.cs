using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Model_E_Mage : ClassEnemy
{
    Viewer_E_Mage _view;

    [Header("EnemyRoom Distances and Angles:")]
    public float viewDistancePersuit;
    public float angleToPersuit;
    public float viewDistanceSurround;
    public float angleToSurround;

    public Action IdleEvent;
    public Action WalkEvent;
    public Action RetreatEvent;
    public Action WalkLeftEvent;
    public Action WalkRightEvent;
    public Action MagicAttackEvent;

    [Header("Enemy Surround Variables:")]

    public float surroundSpeed;
    public float surroundTimer;
    public float surroundTimerMin;
    public float surroundTimerMax;
    public int surroundBehaviourID;

    [Header("Enemy Attack Variables:")]

    public float timeToAttack;
    public float maxTimeToAttack;
    public float minTimeToAttack;
    public bool canAttack;
    public bool onAttackAnimation;
    public bool attackFinish;
    public bool shooting;

    IEnumerator OnAttackAnimationCorrutine(float time)
    {
        onAttackAnimation = true;
        yield return new WaitForSeconds(time);
        onAttackAnimation = false;
        if (onDamageTime <= 0) attackFinish = true;
    }

    void Start()
    {
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
        _view = GetComponent<Viewer_E_Mage>();

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var takeDamage = new N_FSM_State("TAKE_DAMAGE");


        IdleEvent += _view.AnimIdleCombat;
        WalkEvent += _view.AnimWalkCombat;
        RetreatEvent += _view.AnimRetreat;
        WalkRightEvent += _view.AnimWalkRight;
        WalkLeftEvent += _view.AnimWalkLeft;
        MagicAttackEvent += _view.AnimShootMagicAttack;
        MagicAttackEvent += _view.HeavyHitAntisipation;
        GetHitEvent += _view.AnimGetHit;

        patrol.OnUpdate += () =>
        {
            if (canPersuit) myFSM_EventMachine.ChangeState(persuit);
        };

        persuit.OnUpdate += () =>
        {
            viewDistanceSurround = 6f;
            WalkEvent();
            player.CombatStateUp();

            MoveToTarget(player.transform);

            if (canSurround) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0) myFSM_EventMachine.ChangeState(takeDamage);
        };

        persuit.OnExit += () =>
        {
            viewDistancePersuit = 100;           
        };

        surround.OnEnter += () =>
        {

            surroundBehaviourID = UnityEngine.Random.Range(0, 3);

            surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);

            if (timeToAttack <= 0)
            {
                timeToAttack = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);
            }

        };

        surround.OnUpdate += () =>
        {
            viewDistanceSurround = 7f;

            player.CombatStateUp();

            surroundTimer -= Time.deltaTime;

            timeToAttack -= Time.deltaTime;

            var obs = Physics.OverlapSphere(transform.position, 1, layersObstacles);
            if (obs.Count() > 0) rb.MovePosition(transform.position + transform.forward * 2 * Time.deltaTime);

            if (surroundBehaviourID == 0)
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

            if (surroundTimer <= 0)
            {
                surroundBehaviourID = UnityEngine.Random.Range(0, 3);

                surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);
            }

            if (!canSurround && canPersuit && timeToAttack > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack <= 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0) myFSM_EventMachine.ChangeState(takeDamage);
        };

        surround.OnExit += () =>
        {
            
        };

        attack.OnEnter += () =>
        {
            shooting = true;
        };

        attack.OnUpdate += () =>
        {
            player.CombatStateUp();

           
           
            Quaternion targetRotation;
            var _dir = (player.transform.position - transform.position).normalized;
            _dir.y = 0;
            targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
            
            var _dirToTarget = (player.transform.position - transform.position).normalized;

            var _distanceToTarget = Vector3.Distance(transform.position, player.transform.position);

            RaycastHit hit;

            canAttack = false;

            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out hit, _distanceToTarget, layersCanSee))
            {
                if (hit.transform.name == player.name) canAttack = true;
                else canAttack = false;
            }

            if (!canSurround && canPersuit && attackFinish) myFSM_EventMachine.ChangeState(persuit);

            if (canSurround && attackFinish) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (canAttack && !onAttackAnimation && shooting)
            {            
                onAttackAnimation = true;
                MagicAttackEvent();            
                StartCoroutine(OnAttackAnimationCorrutine(1.2f));
            }

           
        };

        attack.OnExit += () =>
        {
           
            if (timeToAttack <= 0)
            {
                timeToAttack = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);                
            }

            surroundBehaviourID = UnityEngine.Random.Range(0, 2);

            surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);

            attackFinish = false;
            shooting = false;
        };


        myFSM_EventMachine = new N_FSM_EventMachine(patrol);

    }

   
    void Update()
    {
        canPersuit = CanSee(player.transform, viewDistancePersuit, angleToPersuit, layersCanSee);

        canSurround = CanSee(player.transform, viewDistanceSurround, angleToSurround, layersCanSee);

        myFSM_EventMachine.Update();
    }

    public void Shoot()
    {

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

    }
}
