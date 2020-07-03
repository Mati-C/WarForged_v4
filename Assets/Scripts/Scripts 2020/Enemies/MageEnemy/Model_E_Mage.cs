using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public class Model_E_Mage : ClassEnemy
{
    Viewer_E_Mage _view;

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

    [Header("EnemyMage Attack Variables:")]

    public Transform phShoot;
    public MageMissile missile;
    public float attackDamage;
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
        ListNodesToRetreat.Remove(ListNodesToRetreat[0]);
        ia_Manager = FindObjectOfType<IA_CombatManager>();
        playerFireSowrd = FindObjectOfType<FireSword>();
        exp = playerFireSowrd.mageExp;
        enemyLayer = layersCanSee;

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var takeDamage = new N_FSM_State("TAKE_DAMAGE");
        var blocked = new N_FSM_State("BLOCKED");
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

        patrol.OnEnter += () =>
        {
            onPatrol = true;
            RestartDistances_Angles();
            enemyLayer = layersCanSee;
        };

        patrol.OnUpdate += () =>
        {
            var distance = Vector3.Distance(transform.position, patrolPosition);

            if (distance > 1)
            {
                WalkEvent();
                MoveToTarget(patrolPosition);
            }

            if (distance <= 1)
            {
                IdleEvent();
                Quaternion targetRotation = Quaternion.LookRotation(patrolForward, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
            }

            if (canPersuit && !PlayerOnGrid()) myFSM_EventMachine.ChangeState(persuit);
        };

        patrol.OnExit += () =>
        {
            viewDistancePersuit = 100;
            angleToPersuit = 360;
            angleToSurround = 360;

            foreach (var item in sameID_Enemies)
            {
                item.viewDistancePersuit = 100;
                item.angleToPersuit = 360;
                item.angleToSurround = 360;
            }
            onPatrol = false;
            enemyLayer = layersPlayer;
        };

        persuit.OnUpdate += () =>
        {
            viewDistanceSurround = 8.5f;
            WalkEvent();
            player.CombatStateUp();

            MoveToTarget(FindNearNon_AggressiveNode().transform.position);

            if(PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life >0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

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
            viewDistanceSurround = 9f;

            var d = Vector3.Distance(player.transform.position, transform.position);

            player.CombatStateUp();

            surroundTimer -= Time.deltaTime;

            if (!ia_Manager.enemyRangePermisionAttack && !cantAskAgain && ia_Manager.enemiesListOnAttack.Count <= 0) timeToAttack -= Time.deltaTime;

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
                if (NearEnemy() && !cantAvoid) StartCoroutine(AvoidNearEntity());
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
                if (NearEnemy() && !cantAvoid) StartCoroutine(AvoidNearEntity());
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

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (!canSurround && canPersuit && timeToAttack > 0 && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack <= 0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

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

            MoveToTarget(nearNodeToRetreat.position);

            WalkEvent();

            var d = Vector3.Distance(nearNodeToRetreat.transform.position, transform.position);

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (d <= 1 && onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (d <= 1 && !canSurround && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (d <= 1 && timeToAttack <= 0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (d <= 1 && canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        attack.OnEnter += () =>
        {
            shooting = true;

            if (!permissionToAttack && !ia_Manager.enemyRangePermisionAttack && !ia_Manager.decisionOnAttackRange)
            {
                ia_Manager.PermissionsRange(true);
                permissionToAttack = true;
            }

            if (!ia_Manager.decisionOnAttackRange)
            {
                ia_Manager.SetOrderAttack(this);
                ia_Manager.DecisionTakeRange(true);
            }
        };

        attack.OnUpdate += () =>
        {
            player.CombatStateUp();
                     
            var _dir = (player.transform.position - transform.position);
            _dir.y = 0;
            transform.forward = _dir;

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (!canSurround && canPersuit && attackFinish && life >0) myFSM_EventMachine.ChangeState(persuit);

            if (canSurround && attackFinish && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

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

            if (attackFinish) StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission + 1.2f, false));

            else StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission, false));

            shooting = false;
            attackFinish = false;
        };



        blocked.OnUpdate += () =>
        {
       
            attackFinish = false;

            if (PlayerOnGrid() && !blockedAttack) myFSM_EventMachine.ChangeState(patrol);

            if (canPersuit && !canSurround && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };


        takeDamage.OnEnter += () =>
        {
            
        };

        takeDamage.OnUpdate += () =>
        {
            attackFinish = false;

            if (PlayerOnGrid() && onDamageTime <=0) myFSM_EventMachine.ChangeState(patrol);

            if (canPersuit && !canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && onDamageTime <= 0 && life>0) myFSM_EventMachine.ChangeState(attack);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
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

        myFSM_EventMachine.Update();
    }

    public override void Resume()
    {
        StartCoroutine(OnDamageTimer());
        _view.StartCoroutine(_view.DamageTimerAnim());
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

    public override void GetDamage(float d, Model_Player.DamageType t)
    {
        life -= d;
        onDamageTime = damageDelayTime;
        if (player.flamesOn) StartBurning();

        if (life <= 0)
        {
            StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission, true));
            playerFireSowrd.SwordExp(exp);
            DieEvent();
        }

        else
        {
            _view.CreatePopText(d);
            GetHitEvent();
            SoundManager.instance.PlayRandom(SoundManager.instance.damageVoice, transform.position, true);
            SoundManager.instance.Play(Entity.BODY_IMPACT_2, transform.position, true);
        }

        Vector3 toTarget = (player.transform.position - transform.position).normalized;

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
