using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sound;

public class Model_E_Shield : ClassEnemy
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
    public Action AttackEvent;
    public Action ParryEvent;

    Viewer_E_Shield _view;

    [Header("Enemy Surround Variables:")]

    public float surroundSpeed;
    public float surroundTimer;
    public float surroundTimerMin;
    public float surroundTimerMax;
    public int surroundBehaviourID;

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
        _view = GetComponent<Viewer_E_Shield>();
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
        ia_Manager = FindObjectOfType<IA_CombatManager>();

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var takeDamage = new N_FSM_State("TAKE_DAMAGE");
        var die = new N_FSM_State("DIE");
        var blocked = new N_FSM_State("BLOCKED");
        var parry = new N_FSM_State("Parry");

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

            WalkEvent();
            player.CombatStateUp();

            if (aggressiveLevel == 1) MoveToTarget(FindNearAggressiveNode().transform);

            else MoveToTarget(FindNearNon_AggressiveNode().transform);

            if (canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        persuit.OnExit += () =>
        {
            
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
        };

        attack.OnEnter += () =>
        {
            if (!permissionToAttack && !ia_Manager.enemyMeleePermisionAttack && !ia_Manager.decisionOnAttack) 
            {
                ia_Manager.PermissionsMelee(true);
                permissionToAttack = true;
            }

            if (!ia_Manager.decisionOnAttack)
            {
                ia_Manager.SetOrderAttack(this);
                ia_Manager.DecisionTake(true);
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
                MoveToTarget(player.transform);
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

                StartCoroutine(OnAttackAnimationCorrutine(1.2f));    
            }

            if (attackFinish && life > 0) myFSM_EventMachine.ChangeState(retreat);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        attack.OnExit += () =>
        {
            if (timeToAttack <= 0)
            {
                timeToAttack = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);
                if (aggressiveLevel == 2) timeToAttack += 1;
            }

            surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);

            if (attackFinish) StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission + 1.2f));

            else StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission));
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

            if (timeToRetreat <= 0 && canPersuit && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToRetreat <= 0 && canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (onParry && life > 0) myFSM_EventMachine.ChangeState(parry);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        takeDamage.OnEnter += () =>                           
        {
            timeToRetreat = 0;           
        };

        takeDamage.OnUpdate += () =>
        {
            waitingForRetreat = false;
            attackFinish = false;

            if (canPersuit && !canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && onDamageTime <= 0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        takeDamage.OnExit += () =>
        {
            timeToRetreat = 0;
        };

        blocked.OnEnter += () =>
        {
            timeToRetreat = 0;
        };

        blocked.OnUpdate += () =>
        {
            waitingForRetreat = false;
            attackFinish = false;

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
        };

        parry.OnEnter += () =>
        {
            timeToRetreat = 0;
        };

        parry.OnUpdate += () =>
        {
            waitingForRetreat = false;
            attackFinish = false;

            player.CombatStateUp();
            Vector3 _dir = Vector3.zero;
            Quaternion targetRotation;
            _dir = (player.transform.position - transform.position).normalized;
            _dir.y = 0;
            targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

            if (canPersuit && !canSurround && !onParry && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && !onParry && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && !onParry && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        parry.OnExit += () =>
        {
            timeToRetreat = 0;
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

    public void PlusAttackMove(float t) { _timeToMoveOnAttack = t; }

    public void DefenceBrokenStart() { StartCoroutine(DefenceBrokenTimer()); }

    public override void GetDamage(float d, Model_Player.DamageType t)
    {
        Vector3 toTarget = (player.transform.position - transform.position).normalized;

        if (onAttackAnimation || t == Model_Player.DamageType.Heavy)
        {
            DefenceBrokenStart();
            defenceBroken = true;
        }

        if (defenceBroken || knocked)
        {
            life -= d;
            onDamageTime = damageDelayTime;
            if (player.flamesOn) StartBurning();
            if (life <= 0) DieEvent();

            else
            {
                _view.CreatePopText(d);
                GetHitEvent();
                SoundManager.instance.PlayRandom(SoundManager.instance.damageVoice, transform.position, true);
                SoundManager.instance.Play(Entity.BODY_IMPACT_2, transform.position, true);
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

        else
        {
            ParryEvent();
            player.FailAttack();
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
        var p = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<Model_Player>()).Select(x => x.GetComponent<Model_Player>());

        if (p.Count() > 0)
        {
            if (CanSee(p.First().transform, viewDistanceAttack, angleToAttack, layersCanSee)) p.First().GetDamage(attackDamage, transform, Model_Player.DamageType.Light);
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
