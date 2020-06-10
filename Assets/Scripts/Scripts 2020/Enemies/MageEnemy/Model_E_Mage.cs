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

    public Transform phShoot;
    public MageMissile missile;
    public float attackDamage;
    public float timeToAttack;
    public float maxTimeToAttack;
    public float minTimeToAttack;
    public bool onAttackAnimation;
    public bool attackFinish;
    public bool shooting;

    [Header("Enemy Retreat Variables:")]

    public Transform nodesToRetreat;
    public Transform nearNodeToRetreat;
    public List<Transform> ListNodesToRetreat;
    public float timeToRetreatAgain;
    public float maxTimeToRetreatAgain;
    public float distanceToRetreat;

    IEnumerator TimerRetreatAgain()
    {
        timeToRetreatAgain = maxTimeToRetreatAgain;
        while(timeToRetreatAgain > 0)
        {
            timeToRetreatAgain -= Time.deltaTime;
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
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
        _view = GetComponent<Viewer_E_Mage>();
        ListNodesToRetreat.AddRange(nodesToRetreat.GetComponentsInChildren<Transform>());

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var takeDamage = new N_FSM_State("TAKE_DAMAGE");
        var die = new N_FSM_State("DIE");


        IdleEvent += _view.AnimIdleCombat;
        WalkEvent += _view.AnimWalkCombat;
        RetreatEvent += _view.AnimRetreat;
        WalkRightEvent += _view.AnimWalkRight;
        WalkLeftEvent += _view.AnimWalkLeft;
        MagicAttackEvent += _view.AnimShootMagicAttack;
        MagicAttackEvent += _view.HeavyHitAntisipation;
        GetHitEvent += _view.AnimGetHit;
        DieEvent += _view.AnimDie;

        StartCoroutine(OnDamageTimer());

        patrol.OnUpdate += () =>
        {
            if (canPersuit) myFSM_EventMachine.ChangeState(persuit);
        };

        patrol.OnExit += () =>
        {
           viewDistancePersuit = 100;
           angleToPersuit = 360;
           angleToSurround = 360;
        };

        persuit.OnUpdate += () =>
        {
            viewDistanceSurround = 6f;
            WalkEvent();
            player.CombatStateUp();

            MoveToTarget(player.transform);

            if (canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life >0) myFSM_EventMachine.ChangeState(takeDamage);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
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

            var d = Vector3.Distance(player.transform.position, transform.position);

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
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
            }

            if (surroundBehaviourID == 1)
            {
                WalkRightEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
                rb.MovePosition(rb.position + transform.right * surroundSpeed * Time.deltaTime);
            }

            if (surroundBehaviourID == 2)
            {
                WalkLeftEvent();
                Quaternion targetRotation;
                var dir = (player.transform.position - transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);
                rb.MovePosition(rb.position - transform.right * surroundSpeed * Time.deltaTime);

            }

            if (surroundTimer <= 0)
            {
                surroundBehaviourID = UnityEngine.Random.Range(0, 3);

                surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);
            }

            if (!canSurround && canPersuit && timeToAttack > 0 && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack <= 0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (d <= distanceToRetreat && timeToRetreatAgain <=0 && life >0) myFSM_EventMachine.ChangeState(retreat);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        surround.OnExit += () =>
        {
            
        };

        retreat.OnEnter += () =>
        {
            nearNodeToRetreat = ListNodesToRetreat.OrderByDescending(x =>
            {
                var d = Vector3.Distance(x.transform.position, transform.position);
                return d;
            }).First();

            StartCoroutine(TimerRetreatAgain());
        };

        retreat.OnUpdate += () =>
        {

            MoveToTarget(nearNodeToRetreat);

            WalkEvent();

            var d = Vector3.Distance(nearNodeToRetreat.transform.position, transform.position);

            Debug.Log(d);

            if (d <= 1 && onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (d <= 1 && !canSurround && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (d <= 1 && timeToAttack <= 0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (d <= 1 && canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        attack.OnEnter += () =>
        {
            shooting = true;
        };

        attack.OnUpdate += () =>
        {
            player.CombatStateUp();
                     
            var _dir = (player.transform.position - transform.position);
            _dir.y = 0;
            transform.forward = _dir;
                     
            if (!canSurround && canPersuit && attackFinish && life >0) myFSM_EventMachine.ChangeState(persuit);

            if (canSurround && attackFinish && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);

            if (!onAttackAnimation && shooting)
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

   
        takeDamage.OnUpdate += () =>
        {
            attackFinish = false;

            if (canPersuit && !canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && onDamageTime <= 0 && life>0) myFSM_EventMachine.ChangeState(attack);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
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
        var _dir = (player.transform.position - transform.position);
        _dir.y = 0;
        var m = Instantiate(missile);               
        m.damage = attackDamage;
        m.transform.position = phShoot.position;
        m.transform.forward = _dir;

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
