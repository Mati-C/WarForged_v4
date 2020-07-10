using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Model_B_Ogre1 : ClassEnemy
{
    Viewerl_B_Ogre1 _view;

    [Header("Boss Faces:")]
    public bool face1;
    public bool face2;
    public bool face3;
    [Range(10, 100)]
    public float lightAttackCoef;
    [Range(10, 100)]
    public float HeavyAttackCoef;
    [Range(10, 100)]
    public float ComboAttackCoef;

    [Header("Boss Attack Variables:")]

    public float angleToAttack;
    public float viewDistanceAttack;
    public bool canAttack;
    public int attackID;

    [Header("Boss Surround Variables:")]

    public float surroundSpeed;
    public float surroundTimer;
    public float surroundTimerMin;
    public float surroundTimerMax;

    public Action IdleEvent;
    public Action WalkEvent;
    public Action WalkRightEvent;
    public Action WalkLeftEvent;
    public Action TauntEvent;

    bool _onTaunt;

    IEnumerator TauntCorrutine()
    {
        _onTaunt = true;
        yield return new WaitForSeconds(2.3f);
        _onTaunt = false;
    }


    void Start()
    {
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
        _view = GetComponent<Viewerl_B_Ogre1>();
        playerFireSowrd = FindObjectOfType<FireSword>();
        
        exp = playerFireSowrd.BossOgre1Exp;

        var attack = new N_FSM_State("ATTACK");
        var surround = new N_FSM_State("SURROUND");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");

        patrolState = patrol;
        IdleEvent += _view.AnimIdle;
        WalkEvent += _view.AnimWalk;
        WalkLeftEvent += _view.animWalkLeft;
        WalkRightEvent += _view.animWalkRight;
        TauntEvent += _view.AnimTaunt;

        patrol.OnEnter += () =>
        {

        };

        patrol.OnUpdate += () =>
        {
            if(canPersuit) myFSM_EventMachine.ChangeState(persuit);
        };

        patrol.OnExit += () =>
        {
            var dir = (player.transform.position - transform.position).normalized;
            dir.y = 0;
            transform.forward = dir;
            TauntEvent();
            StartCoroutine(TauntCorrutine());
        };

        persuit.OnEnter += () =>
        {
            
        };

        persuit.OnUpdate += () =>
        {
            player.CombatStateUp();
            if (!_onTaunt)
            {
                WalkEvent();
                MoveToTarget(player.transform.position);
            }

            if(canSurround) myFSM_EventMachine.ChangeState(surround);
        };

        persuit.OnExit += () =>
        {

        };

        surround.OnEnter += () =>
        {
            viewDistanceSurround += 1;
        };

        surround.OnUpdate += () =>
        {
            player.CombatStateUp();

            surroundTimer -= Time.deltaTime;
           
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
        };

        surround.OnExit += () =>
        {
            viewDistanceSurround -= 1;
        };

        attack.OnEnter += () =>
        {
            attackID = StartAttackStrategy();
        };

        attack.OnUpdate += () =>
        {
            player.CombatStateUp();

            if (!canAttack)
            {
                var d = Vector3.Distance(transform.position, player.transform.position);
                if (d > 0.5f) WalkEvent();

                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

                MoveToTarget(player.transform.position);
            }

            if (canAttack)
            {                
                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

            }
        };

        attack.OnExit += () =>
        {

        };

        myFSM_EventMachine = new N_FSM_EventMachine(patrol);
    }

    
    void Update()
    {
        canPersuit = CanSee(player.transform, viewDistancePersuit, angleToPersuit, layersCanSee);

        canSurround = CanSee(player.transform, viewDistanceSurround, angleToSurround, layersCanSee);

        canAttack = CanSee(player.transform, viewDistanceAttack, 360, layersCanSee);

        myFSM_EventMachine.Update();
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

    public override void GetDamage(float d, Model_Player.DamageType t)
    {

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
