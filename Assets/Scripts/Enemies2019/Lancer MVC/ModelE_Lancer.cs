using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.AI;

public class ModelE_Lancer : EnemyMeleeClass
{
    public enum EnemyInputs { PATROL, PERSUIT, WAIT, ATTACK, RETREAT, FOLLOW, DIE, ANSWER, DEFENCE, STUNED, KNOCK, DODGE }
    public Transform attackPivot;
    public EventFSM<EnemyInputs> _myFsm;
    public ViewerE_Lancer view;

    [Header("Enemy Stats:")]

    public float dodgePoints;
    public float angleToHit;
    public float distanceToHit;
    public float timesToCounterAttack;
    public float timeOnDodge;
    public bool onDefence;
    public bool damageDone;
    public bool impulse;
    public bool canBeHit;
    public string animClipName;
    Vector3 stunedForward;
    Quaternion startRotation;
    Vector3 startPos;

    public Action TakeDamageEvent;
    public Action CombatWalkEvent;
    public Action PointEvent;
    public Action AttackRunEvent;
    public Action AttackEvent;
    public Action WalkBackEvent;
    public Action DeadEvent;
    public Action ChatEvent;
    public Action StunedEvent;
    public Action KnockEvent;
    public Action BlockEvent;
    public Action PerfectBlockedEvent;
    public Action DodgeEvent;

    public IEnumerator OnDodge()
    {
        
        while (timeOnDodge > 0)
        {
            timeOnDodge -= Time.deltaTime;

            if(view.anim.GetBool("DodgeRight"))
            {
                transform.position += transform.right * 2 * Time.deltaTime;
            }

            if (view.anim.GetBool("DodgeLeft"))
            {
                transform.position -= transform.right * 2 * Time.deltaTime;
            }

            if (view.anim.GetBool("DodgeBack"))
            {
                transform.position -= transform.forward * 2 * Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator CanDodgeDelay()
    {
        canBeHit = true;
        yield return new WaitForSeconds(5f);
        canBeHit = false;
    }

    public IEnumerator OnDamageCorrutine()
    {
        onDamage = true;

        while (onDamage)
        {
            timeOnDamage -= Time.deltaTime;
            if (timeOnDamage <= 0) onDamage = false;
            yield return new WaitForEndOfFrame();
        }
    }

    public override IEnumerator ChangeDirRotation()
    {

        cooldwonReposition = true;
        yield return new WaitForSeconds(1);
        cooldwonReposition = true;
    }

    public IEnumerator DelayTurn()
    {
        yield return new WaitForSeconds(0.1f);
        checkTurn = false;

    }

    IEnumerator DelayPoint()
    {
        enemyPointer = true;
        PointEvent();
        yield return new WaitForSeconds(1);
        enemyPointer = false;
       // view.PointAnimationFalse();
    }

    public void RepositionMoveRight()
    {
        timeReposition -= Time.deltaTime;
        if (timeReposition <= 0) timeReposition = 0;

        if (timeReposition > 0) rb.MovePosition(rb.position + (transform.forward + transform.right) * speedRotation * Time.deltaTime);


        if (timeReposition <= 0 && timeReposition2 > 0)
        {
            timeReposition2 -= Time.deltaTime;
            if (timeReposition2 <= 0) timeReposition2 = 0;
            rb.MovePosition(rb.position + (-transform.forward + transform.right) * speedRotation * Time.deltaTime);
        }

    }

    public override IEnumerator AvoidWarriorRight()
    {
        timeReposition = 0.7f;
        timeReposition2 = 0.7f;
        reposition = true;
        while (timeReposition > 0)
        {
            RepositionMoveRight();
            yield return new WaitForEndOfFrame();
        }

        while (timeReposition2 > 0)
        {
            RepositionMoveRight();
            yield return new WaitForEndOfFrame();
        }

        cooldwonReposition = true;

        reposition = false;

        yield return new WaitForSeconds(1);

        cooldwonReposition = false;
    }

    public void RepositionMoveLeft()
    {
        timeReposition -= Time.deltaTime;
        if (timeReposition <= 0) timeReposition = 0;

        if (timeReposition > 0) rb.MovePosition(rb.position + (transform.forward - transform.right) * speedRotation * Time.deltaTime);


        if (timeReposition <= 0 && timeReposition2 > 0)
        {
            timeReposition2 -= Time.deltaTime;
            if (timeReposition2 <= 0) timeReposition2 = 0;
            rb.MovePosition(rb.position + (-transform.forward - transform.right) * speedRotation * Time.deltaTime);
        }

    }

    public override IEnumerator AvoidWarriorLeft()
    {
        timeReposition = 0.7f;
        timeReposition2 = 0.7f;
        reposition = true;
        while (timeReposition > 0)
        {
            RepositionMoveLeft();
            yield return new WaitForEndOfFrame();
        }

        while (timeReposition2 > 0)
        {
            RepositionMoveLeft();
            yield return new WaitForEndOfFrame();
        }

        cooldwonReposition = true;

        reposition = false;

        yield return new WaitForSeconds(1);

        cooldwonReposition = false;
    }

    private void Awake()
    {
        view = GetComponent<ViewerE_Lancer>();
        cm = FindObjectOfType<EnemyCombatManager>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerNodes.AddRange(FindObjectsOfType<CombatNode>());
        startPos = transform.position;
        startRotation = transform.rotation;
        myNodes.AddRange(NodePath.GetComponentsInChildren<Node>());

        var myEntites = FindObjectsOfType<EnemyEntity>().Where(x => x != this && x.EnemyID_Area == EnemyID_Area);
        nearEntities.Clear();
        nearEntities.AddRange(myEntites);

        var warriors = FindObjectsOfType<ModelE_Melee>().Where(x => x != this && x.EnemyID_Area == EnemyID_Area);
        EnemyMeleeFriends.Clear();
        EnemyMeleeFriends.AddRange(warriors);

        ca = FindObjectsOfType<CombatArea>().Where(x => x.EnemyID_Area == EnemyID_Area).FirstOrDefault();

        ChatEvent += view.ChangeChatAnimation;
        IdleEvent += view.IdleAnim;
        CombatIdleEvent += view.CombatIdleAnim;
        CombatWalkEvent += view.CombatWalkAnim;
        AttackRunEvent += view.RunAttackAnim;
        AttackEvent += view.AttackAnim;
        DeadEvent += view.DeadAnim;
        WalkBackEvent += view.WalckBackAnim;
        WalkLeftEvent += view.WalkLeftAnim;
        WalkRightEvent += view.WalkRightAnim;
        DodgeEvent += view.DodgeAnim;
        StunedEvent += view.StunedAnim;
        TakeDamageEvent += view.TakeDamageAnim;
        MoveEvent += view.CombatWalkAnim;

        var patrol = new FSM_State<EnemyInputs>("PATROL");
        var persuit = new FSM_State<EnemyInputs>("PERSUIT");
        var wait = new FSM_State<EnemyInputs>("WAIT");
        var attack = new FSM_State<EnemyInputs>("ATTACK");
        var retreat = new FSM_State<EnemyInputs>("RETREAT");
        var die = new FSM_State<EnemyInputs>("DIE");
        var follow = new FSM_State<EnemyInputs>("FOLLOW");
        var answerCall = new FSM_State<EnemyInputs>("ANSWER");
        var stuned = new FSM_State<EnemyInputs>("STUNED");
        var knocked = new FSM_State<EnemyInputs>("KNOCK");
        var dodge = new FSM_State<EnemyInputs>("DODGE");

        StateConfigurer.Create(patrol)
           .SetTransition(EnemyInputs.PERSUIT, persuit)
           .SetTransition(EnemyInputs.ANSWER, answerCall)
           .SetTransition(EnemyInputs.WAIT, wait)
           .SetTransition(EnemyInputs.DIE, die)
           .Done();

        StateConfigurer.Create(stuned)
          .SetTransition(EnemyInputs.PERSUIT, persuit)
          .SetTransition(EnemyInputs.FOLLOW, follow)
          .SetTransition(EnemyInputs.WAIT, wait)
          .SetTransition(EnemyInputs.PATROL, patrol)
          .SetTransition(EnemyInputs.DIE, die)
          .Done();

        StateConfigurer.Create(knocked)
       .SetTransition(EnemyInputs.PERSUIT, persuit)
       .SetTransition(EnemyInputs.FOLLOW, follow)
       .SetTransition(EnemyInputs.WAIT, wait)
       .SetTransition(EnemyInputs.PATROL, patrol)
       .SetTransition(EnemyInputs.DIE, die)
       .Done();

        StateConfigurer.Create(persuit)
         .SetTransition(EnemyInputs.WAIT, wait)
         .SetTransition(EnemyInputs.FOLLOW, follow)
         .SetTransition(EnemyInputs.STUNED, stuned)
         .SetTransition(EnemyInputs.KNOCK, knocked)
         .SetTransition(EnemyInputs.PATROL, patrol)
         .SetTransition(EnemyInputs.DODGE, dodge)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(wait)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.ATTACK, attack)
         .SetTransition(EnemyInputs.STUNED, stuned)
         .SetTransition(EnemyInputs.PATROL, patrol)
         .SetTransition(EnemyInputs.KNOCK, knocked)
         .SetTransition(EnemyInputs.DODGE, dodge)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(retreat)
       .SetTransition(EnemyInputs.PERSUIT, persuit)
       .SetTransition(EnemyInputs.FOLLOW, follow)
       .SetTransition(EnemyInputs.WAIT, wait)
       .SetTransition(EnemyInputs.STUNED, stuned)
       .SetTransition(EnemyInputs.PATROL, patrol)
       .SetTransition(EnemyInputs.KNOCK, knocked)
       .SetTransition(EnemyInputs.DODGE, dodge)
       .SetTransition(EnemyInputs.DIE, die)
       .Done();


        StateConfigurer.Create(attack)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.FOLLOW, follow)
        .SetTransition(EnemyInputs.WAIT, wait)
        .SetTransition(EnemyInputs.RETREAT, retreat)
        .SetTransition(EnemyInputs.STUNED, stuned)
        .SetTransition(EnemyInputs.PATROL, patrol)
        .SetTransition(EnemyInputs.KNOCK, knocked)
        .SetTransition(EnemyInputs.DODGE, dodge)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(dodge)
       .SetTransition(EnemyInputs.PERSUIT, persuit)
       .SetTransition(EnemyInputs.FOLLOW, follow)
       .SetTransition(EnemyInputs.WAIT, wait)
       .SetTransition(EnemyInputs.RETREAT, retreat)
       .SetTransition(EnemyInputs.STUNED, stuned)
       .SetTransition(EnemyInputs.PATROL, patrol)
       .SetTransition(EnemyInputs.KNOCK, knocked)
       .SetTransition(EnemyInputs.ATTACK, attack)
       .SetTransition(EnemyInputs.DIE, die)
       .Done();


        StateConfigurer.Create(follow)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.STUNED, stuned)
         .SetTransition(EnemyInputs.WAIT, wait)
         .SetTransition(EnemyInputs.PATROL, patrol)
         .SetTransition(EnemyInputs.KNOCK, knocked)
         .SetTransition(EnemyInputs.DODGE, dodge)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();


        StateConfigurer.Create(answerCall)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.WAIT, wait)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        patrol.OnEnter += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
        };

        patrol.OnFixedUpdate += () =>
        {

            if (patroling)
            {
                view.anim.SetBool("WalkForward", true);

                timeToPatrol -= Time.deltaTime;
                currentAction = new A_Patrol(this);

                if (target.fadeTimer > target.view.fadeTime) IdleEvent();

                if ((!isDead && isPersuit && !isWaitArea && target.fadeTimer > target.view.fadeTime) || onDamage) SendInputToFSM(EnemyInputs.PERSUIT);

                if (!isDead && isAnswerCall && target.fadeTimer > target.view.fadeTime) SendInputToFSM(EnemyInputs.ANSWER);

                if (!isDead && isWaitArea && target.fadeTimer > target.view.fadeTime) SendInputToFSM(EnemyInputs.WAIT);

                if (isDead) SendInputToFSM(EnemyInputs.DIE);

            }

            if (chating)
            {
                currentAction = new A_Chating(this);

                if (!isDead && isPersuit && !isWaitArea && target.fadeTimer > target.view.fadeTime && enemyPointer)
                {
                    var dir = target.transform.position - transform.position;
                    dir.y = 0;
                    transform.forward = dir;
                    StartCoroutine(DelayPoint());
                }

                if (target.fadeTimer > target.view.fadeTime && !enemyPointer) IdleEvent();

                if ((!isDead && isPersuit && !isWaitArea && target.fadeTimer > target.view.fadeTime && !enemyPointer) || onDamage) SendInputToFSM(EnemyInputs.PERSUIT);

                if (!isDead && isAnswerCall && target.fadeTimer > target.view.fadeTime && !enemyPointer) SendInputToFSM(EnemyInputs.ANSWER);

                if (!isDead && isWaitArea && target.fadeTimer > target.view.fadeTime && !enemyPointer) SendInputToFSM(EnemyInputs.WAIT);

                if (timeOnDodge > 0) SendInputToFSM(EnemyInputs.DODGE);

                if (isDead) SendInputToFSM(EnemyInputs.DIE);
            }


        };

        patrol.OnUpdate += () =>
        {
            if (myPointer)
            {
                target.ReturnPointer(myPointer);
                myPointer = null;
            }

            healthBar.SetActive(false);

            timeToPatrol -= Time.deltaTime;
        };

        patrol.OnExit += x =>
        {
            onCombat = true;
            view.anim.SetBool("Chat1", false);
            view.anim.SetBool("Chat2", false);
            view.anim.SetBool("Chat3", false);
        };

        answerCall.OnFixedUpdate += () =>
        {

            currentAction = new A_FollowTarget(this);
            CombatWalkEvent();
            if (!isDead && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isWaitArea) SendInputToFSM(EnemyInputs.WAIT);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };


        answerCall.OnExit += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
        };

        persuit.OnEnter += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
        };

        persuit.OnFixedUpdate += () =>
        {
           

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            if (aggressiveLevel == 1) viewDistanceAttack = 3.5f;

            if (aggressiveLevel == 2) viewDistanceAttack = 7f;


            if (!onDamage) CombatWalkEvent();

            isAnswerCall = false;

            firstSaw = true;

            if (timeToAttack && aggressiveLevel == 1) delayToAttack -= Time.deltaTime;

            if (timeToAttack && aggressiveLevel == 2) delayToAttack -= Time.deltaTime / 2;

            foreach (var item in nearEntities) if (!item.isAnswerCall && !item.firstSaw) item.isAnswerCall = true;

            currentAction = new A_Persuit(this);

            var d = Vector3.Distance(transform.position, target.transform.position);

            if (!isDead && isWaitArea && !isStuned) SendInputToFSM(EnemyInputs.WAIT);

            if (!isDead && isStuned && !isKnock) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && isKnock) SendInputToFSM(EnemyInputs.KNOCK);

            if (!isDead && !isPersuit && !isWaitArea && !isStuned) SendInputToFSM(EnemyInputs.FOLLOW);

            if (timeOnDodge > 0) SendInputToFSM(EnemyInputs.DODGE);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };

        wait.OnEnter += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            onDefence = false;

            if (!timeToAttack) delayToAttack = UnityEngine.Random.Range(timeMinAttack, timeMaxAttack);

            onRetreat = false;

            if (aggressiveLevel == 1) viewDistanceAttack = 5f;

            if (aggressiveLevel == 2) viewDistanceAttack = 8f;

            if (timeToChangeRotation <= 0) timeToChangeRotation = UnityEngine.Random.Range(1.5f, 3);

            var myNodes = playerNodes.Where(y => y.myOwner == this);

            foreach (var item in myNodes) item.myOwner = null;

            distanceToBack = aggressiveLevel;

        };


        wait.OnUpdate += () =>
        {
            

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            if (!reposition) timeToChangeRotation -= Time.deltaTime;

            if (timeToChangeRotation <= 0 && changeRotateWarrior)
            {
                timeToChangeRotation = UnityEngine.Random.Range(2f, 3f);
                flankDir = 2;
                changeRotateWarrior = false;
            }

            if (timeToChangeRotation <= 0 && !changeRotateWarrior)
            {
                flankDir = UnityEngine.Random.Range(0, 2);
                timeToChangeRotation = UnityEngine.Random.Range(2, 3);
                changeRotateWarrior = true;
            }

            var NearNodes = Physics.OverlapSphere(transform.position, distanceToHit + 1).Where(y => y.GetComponent<CombatNode>()).Select(y => y.GetComponent<CombatNode>()).Where(y => y.myOwner == null).ToList();

            foreach (var item in NearNodes)
            {
                item.myOwner = this;
            }

            if (NearNodes.Count() > 0)
            {
                var restOfNodes = new List<CombatNode>();

                restOfNodes.AddRange(playerNodes);

                foreach (var item in NearNodes)
                {
                    restOfNodes.Remove(item);
                }

                foreach (var item in restOfNodes)
                {
                    if (item.myOwner == this) item.myOwner = null;
                }
            }

            currentAction = new A_WarriorWait(this, flankDir);


            if (timeToAttack && aggressiveLevel == 1) delayToAttack -= Time.deltaTime;

            if (timeToAttack && aggressiveLevel == 2) delayToAttack -= Time.deltaTime / 2;

            angleToAttack = 110;

            isAnswerCall = false;

            firstSaw = true;

            foreach (var item in nearEntities) if (!item.isAnswerCall && !item.firstSaw) item.isAnswerCall = true;

            if (!isDead && !isWaitArea && isPersuit && !onDefence && !isStuned) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isWaitArea && !isPersuit && !onDefence && !isStuned) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && delayToAttack <= 0 && !onDefence && !isStuned) SendInputToFSM(EnemyInputs.ATTACK);

            if (!isDead && isStuned && !isKnock) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && isKnock) SendInputToFSM(EnemyInputs.KNOCK);

            if (timeOnDodge > 0) SendInputToFSM(EnemyInputs.DODGE);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);


        };

        wait.OnExit += x =>
        {

            if (aggressiveLevel == 1) viewDistanceAttack = 3.5f;

            if (aggressiveLevel == 2) viewDistanceAttack = 7f;
        };

        attack.OnEnter += x =>
        {
            if (myPointer) myPointer.StartAdvertisement();

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            onDefence = false;

            delayToAttack = 0;
            onRetreat = false;
            damageDone = false;
        };

        attack.OnFixedUpdate += () =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            view.anim.SetBool("WalkBack", false);

            AttackRunEvent();

            currentAction = new A_LancerAttack(this);

            isAnswerCall = false;

            firstSaw = true;

            foreach (var item in nearEntities) if (!item.isAnswerCall && !item.firstSaw) item.isAnswerCall = true;

            if (!isDead && !isWaitArea && isPersuit && !onRetreat && !onDefence) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isPersuit && !isWaitArea && !onRetreat && !onDefence) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && isStuned && !isKnock) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && isKnock) SendInputToFSM(EnemyInputs.KNOCK);

            if (onRetreat && !onDefence && !isStuned) SendInputToFSM(EnemyInputs.RETREAT);

            if(timeOnDodge>0) SendInputToFSM(EnemyInputs.DODGE);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

        };

        dodge.OnUpdate += () =>
        {
            currentAction = new A_Idle();

            if (!isDead && !isWaitArea && isPersuit && timeOnDodge<=0) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isWaitArea && !onRetreat && timeOnDodge <= 0) SendInputToFSM(EnemyInputs.WAIT);

            if (!isDead && !isWaitArea && !isPersuit && timeOnDodge <= 0) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && delayToAttack <= 0 && timeOnDodge <= 0) SendInputToFSM(EnemyInputs.ATTACK);

            if (!isDead && isStuned && !isKnock) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && isKnock) SendInputToFSM(EnemyInputs.KNOCK);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };

        stuned.OnEnter += x =>
        {
            stunedForward = transform.forward;

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
            cm.times++;
            onRetreat = false;
            timeToAttack = false;
            onDefence = false;
            view.anim.SetBool("WalkBack", false);
            checkTurn = false;
            timeStuned = 3;
        };

        stuned.OnUpdate += () =>
        {

            transform.forward = stunedForward;

            timeStuned -= Time.deltaTime;

            currentAction = new A_Idle();

            if (timeStuned <= 0) isStuned = false;

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

            if (!isDead && isWaitArea && !onRetreat && timeStuned <= 0) SendInputToFSM(EnemyInputs.WAIT);

            if (!isDead && isPersuit && !isWaitArea && !onRetreat && timeStuned <= 0) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isPersuit && !isWaitArea && !onRetreat && timeStuned <= 0) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && onRetreat && timeStuned <= 0) SendInputToFSM(EnemyInputs.FOLLOW);
        };

        stuned.OnExit += x =>
        {

            isStuned = false;
            view.StunedAnimFalse();
        };

        knocked.OnEnter += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
            cm.times++;
            onRetreat = false;
            timeToAttack = false;
            onDefence = false;
            view.anim.SetBool("WalkBack", false);
            checkTurn = false;
            timeKnocked = 4f;
            isKnock = true;
        };

        knocked.OnUpdate += () =>
        {

            currentAction = new A_Idle();

            timeKnocked -= Time.deltaTime;

            if (timeKnocked <= 0) isKnock = false;

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

            if (!isDead && isWaitArea && !onRetreat && !isKnock) SendInputToFSM(EnemyInputs.WAIT);

            if (!isDead && isPersuit && !isWaitArea && !onRetreat && !isKnock) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isPersuit && !isWaitArea && !onRetreat && !isKnock) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && onRetreat && !isKnock) SendInputToFSM(EnemyInputs.FOLLOW);
        };

        retreat.OnEnter += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
            onDefence = false;
            view.anim.SetBool("RunAttack", false);
            view.anim.SetBool("WalkCombat", false);
            view.anim.SetBool("WalkCombat", false);
            view.anim.SetBool("WalkL", false);
            view.anim.SetBool("WalkR", false);
            view.anim.SetBool("Idle", false);
            if (distanceToBack == 1) timeToRetreat = 0.5f;
            if (distanceToBack == 2) timeToRetreat = 1.5f;
        };

        retreat.OnFixedUpdate += () =>
        {


            currentAction = new A_LancerRetrat(this);

            var d = Vector3.Distance(transform.position, target.transform.position);

            if (!isDead && isPersuit && !isWaitArea && timeToRetreat <= 0 && !isStuned) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isPersuit && !isWaitArea && !isStuned) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && isStuned && !isKnock) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && isKnock) SendInputToFSM(EnemyInputs.KNOCK);

            if (!isDead && isWaitArea && timeToRetreat <= 0 && !isStuned) SendInputToFSM(EnemyInputs.WAIT);

            if (timeOnDodge > 0) SendInputToFSM(EnemyInputs.DODGE);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

        };

        retreat.OnUpdate += () =>
        {
            Debug.Log("retreat");

            view.anim.SetBool("WalkBack", true);

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            if (animClipName == view.animDictionary[ViewerE_Lancer.EnemyMeleeAnim.Retreat] || animClipName == view.animDictionary[ViewerE_Lancer.EnemyMeleeAnim.IdleCombat])
            {
                if (myPointer) myPointer.StopAdvertisement();

                timeToRetreat -= Time.deltaTime;

                var d = Vector3.Distance(transform.position, target.transform.position);

                float maxD = 0;

                if (aggressiveLevel == 1) maxD = 2.5f;
                if (aggressiveLevel == 2) maxD = 5f;

                if (d < maxD) view.WalckBackAnim();
                if (d > maxD) view.CombatIdleAnim();
            }

            if ((view.anim.GetBool("Attack") || view.anim.GetBool("Attack2") || view.anim.GetBool("Attack3") || view.anim.GetBool("HeavyAttack")) && !isDead && !onDefence)
            {
                transform.LookAt(target.transform.position);
            }

            if (animClipName != view.animDictionary[ViewerE_Lancer.EnemyMeleeAnim.Attack1]) impulse = false;

            if (impulse && animClipName == view.animDictionary[ViewerE_Lancer.EnemyMeleeAnim.Attack1]) transform.position += transform.forward * 2 * Time.deltaTime;

            view.anim.SetBool("WalkL", false);
            view.anim.SetBool("WalkR", false);
        };

        retreat.OnExit += x =>
        {

            StopRetreat();
        };

        follow.OnEnter += x =>
        {

        };

        follow.OnUpdate += () =>
        {
            Debug.Log("follow");

            if (!onDamage) CombatWalkEvent();

            if (!isDead && isPersuit && !isWaitArea && !isStuned) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isWaitArea && !isStuned) SendInputToFSM(EnemyInputs.WAIT);

            if (!isDead && isStuned && !isKnock) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && isKnock) SendInputToFSM(EnemyInputs.KNOCK);

            if (timeOnDodge > 0) SendInputToFSM(EnemyInputs.DODGE);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };

        follow.OnFixedUpdate += () =>
        {
            currentAction = new A_FollowTarget(this);
        };

        follow.OnExit += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

        };

        die.OnEnter += x =>
        {
            if (myPointer)
            {
                target.ReturnPointer(myPointer);
                myPointer = null;
            }

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
            DeadEvent();
            currentAction = null;
            timeToAttack = false;
            healthBar.SetActive(false);
            if (EnemyMeleeFriends.Count > 0)
            {
                foreach (var item in EnemyMeleeFriends)
                {
                    RemoveWarriorFriend(item);
                }
            }

            if (nearEntities.Count > 0)
            {
                foreach (var item in nearEntities)
                {
                    RemoveNearEntity(item);
                }
            }

            ca.myEntities--;
            cm.times++;
            cm.enemiesList.Remove(this);

            var myNodes = playerNodes.Where(y => y.myOwner == this);

            foreach (var item in myNodes) item.myOwner = null;
        };

        _myFsm = new EventFSM<EnemyInputs>(patrol);
    }

    void Start()
    {

    }

    private void Update()
    {
        animClipName = view.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        _myFsm.Update();

        if (target != null)
        {

            isPersuit = SearchForTarget.SearchTarget(target.transform, viewDistancePersuit, angleToPersuit, transform, true, layerObst) && target.isInCombatArea;

            isWaitArea = SearchForTarget.SearchTarget(target.transform, viewDistanceAttack, angleToAttack, transform, true, layerObst);

            onAttackArea = SearchForTarget.SearchTarget(target.transform, distanceToHit, angleToHit, transform, true, layerObst);


        }

        if (target.isInCombat && renderObject.isVisible && myPointer)
        {
            target.ReturnPointer(myPointer);
            myPointer = null;
        }

        if (target.isInCombat && !renderObject.isVisible && !myPointer && !isDead && onCombat)
        {
            myPointer = target.pointerPool.GetObjectFromPool();
            myPointer.owner = this;
        }
    }

    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
        if (currentAction != null) currentAction.Actions();

    }

    public void FollowState()
    {
        SendInputToFSM(EnemyInputs.FOLLOW);
    }

    public void SendInputToFSM(EnemyInputs inp)
    {
        _myFsm.SendInput(inp);
    }

    public override Vector3 EntitiesAvoidance()
    {
        var obs = Physics.OverlapSphere(transform.position, 1, layerEntites).Where(x => x.GetComponent<ModelE_Melee>()).Select(x => x.GetComponent<ModelE_Melee>()).Where(x => x != this);

        if (obs.Count() > 0)
        {
            var dir = transform.position - obs.First().transform.position;
            dir.y = 0;
            return dir.normalized;
        }
        else return Vector3.zero;
    }

    public override CombatNode FindNearAggressiveNode()
    {
        var node = playerNodes.Where(x => x.aggressive && !x.isBusy && (x.myOwner == null || x.myOwner == this)).OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            return d;

        }).FirstOrDefault();

        if (node)
        {
            myCombatNode = node;

            myCombatNode.myOwner = this;

            if (lastCombatNode == null)
            {
                lastCombatNode = node;
                lastCombatNode.myOwner = this;
            }

            if (myCombatNode != lastCombatNode)
            {
                lastCombatNode.myOwner = null;
                lastCombatNode = myCombatNode;
            }

            var NearNodes = Physics.OverlapSphere(myCombatNode.transform.position, distanceToHit + 1).Where(y => y.GetComponent<CombatNode>()).Select(y => y.GetComponent<CombatNode>()).Where(y => y.myOwner == null);

            foreach (var item in NearNodes)
            {
                item.myOwner = this;
            }

            return node;
        }

        else return playerNodes[0];
    }

    public override CombatNode FindNearNon_AggressiveNode()
    {
        var node = playerNodes.Where(x => x.Non_Aggressive && !x.isBusy && (x.myOwner == null || x.myOwner == this)).OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            return d;

        }).FirstOrDefault();

        if (node)
        {
            myCombatNode = node;

            myCombatNode.myOwner = this;

            if (lastCombatNode == null)
            {
                lastCombatNode = node;
                lastCombatNode.myOwner = this;
            }

            if (myCombatNode != lastCombatNode)
            {
                lastCombatNode.myOwner = null;
                lastCombatNode = myCombatNode;
            }

            var NearNodes = Physics.OverlapSphere(myCombatNode.transform.position, distanceToHit).Where(y => y.GetComponent<CombatNode>()).Select(y => y.GetComponent<CombatNode>()).Where(y => y.myOwner == null);

            foreach (var item in NearNodes)
            {
                item.myOwner = this;
            }

            return node;
        }
        else return playerNodes[0];
    }

    public override void GetDamage(float damage, string typeOfDamage, int damageAnimationIndex)
    {
        Vector3 dir = transform.position - target.transform.position;
        float angle = Vector3.Angle(dir, transform.forward);


        if(typeOfDamage == "Normal" && canBeHit)
        {
            TakeDamageEvent();        
            timeOnDamage = 0.5f;
            if (!onDamage) StartCoroutine(OnDamageCorrutine());
            life -= damage;
            view.blood.Play();
            view.damaged = true;
            view.timeShaderDamage = 1;
            view.LifeBar(life / totalLife);
        }
  
        if(typeOfDamage == "Proyectile")
        {
            if (!isStuned)
            {
                StunedEvent();
                isStuned = true;
                StartCoroutine(CanDodgeDelay());
            }

            else
            {
                TakeDamageEvent();
            }

            timeOnDamage = 0.5f;
            if (!onDamage) StartCoroutine(OnDamageCorrutine());
            life -= damage;
            view.LifeBar(life / totalLife);
        }

        if(angle > 90 && typeOfDamage == "Normal" && animClipName != view.animDictionary[ViewerE_Lancer.EnemyMeleeAnim.Attack1] && !canBeHit)
        {
            dodgePoints++;
            timeOnDodge = 1.5f;
            StartCoroutine(OnDodge());
            DodgeEvent();
            if(dodgePoints >=4)
            {
                view.anim.SetBool("Attack", true);
                dodgePoints = 0;
            }
        }

        if (life <= 0)
            isDead = true;
    }

    public override Node GetMyNode()
    {
        var myNode = myNodes.OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            return d;
        }).First();

        return myNode;
    }

    public override Node GetMyTargetNode()
    {
        var targetNode = myNodes.OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, target.transform.position);
            return d;

        }).First();

        return targetNode;
    }


    public void StopRetreat()
    {

        if (cm.influencedTarget == this) cm.secondBehaviour = false;
        cm.times++;
        onRetreat = false;
        timeToAttack = false;
        view.anim.SetBool("WalkBack", false);
        CombatIdleEvent();
        if (EnemyMeleeFriends.Count > 0) StartCoroutine(DelayTurn());
        else checkTurn = false;
    }

    public override Node GetRandomNode()
    {
        var r = UnityEngine.Random.Range(0, myNodes.Count);
        return myNodes[r];
    }

    public override void MakeDamage()
    {
        view.anim.SetBool("RunAttack", false);

        var player = Physics.OverlapSphere(attackPivot.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();

        var desMesh = Physics.OverlapSphere(attackPivot.position, radiusAttack).Where(x => x.GetComponent<DestructibleOBJ>()).Select(x => x.GetComponent<DestructibleOBJ>()).Distinct();

        if (player != null && !player.invulnerable)
        {
            damageDone = true;

            var dir = (target.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(dir, target.transform.forward);
            player.GetDamage(attackDamage, transform, false, 3, this);
            player.rb.AddForce(transform.forward * 2, ForceMode.Impulse);
        }

        foreach (var item in desMesh)
        {
            item.StartCoroutine(item.startDisolve());
        }
    }

    public override Vector3 ObstacleAvoidance()
    {
        var obs = Physics.OverlapSphere(transform.position, 1, layerObstAndBarrels).Where(x => !x.GetComponent<Model>());
        if (obs.Count() > 0)
        {
            var dir = transform.position - obs.First().transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;
    }

    public override void RemoveNearEntity(EnemyEntity e)
    {
        e.nearEntities.Remove(this);
    }

    public void RemoveWarriorFriend(EnemyMeleeClass e)
    {
        e.EnemyMeleeFriends.Remove(this);
    }

    public override void Respawn()
    {

        transform.position = startPos;
        transform.rotation = startRotation;
        life = totalLife;
        view.LifeBar(life / totalLife);
        view.anim.SetBool("IsDead", false);
        view.anim.SetBool("Idle", true);
        view.anim.SetBool("IdleCombat", false);
        view.anim.Play("Idle_EM");
        view.anim.SetInteger("TakeDamageCounter", 0);
        onDamage = false;
        view.anim.Play("Idle_EM");
        isDead = false;
        onCombat = false;
        view.anim.SetBool("Chat1", chat1);
        view.anim.SetBool("Chat2", chat2);
        view.anim.SetBool("Chat3", chat3);
        chatCurrentTime = 0;
        SendInputToFSM(EnemyInputs.PATROL);
    }

    public override void ChangeChatAnimation()
    {
        ChatEvent();
    }

    public override void SetChatAnimation()
    {
        view.anim.SetBool("Chat1", chat1);
        view.anim.SetBool("Chat2", chat2);
        view.anim.SetBool("Chat3", chat3);
    }

    // Start is called before the first frame update


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistancePersuit);

        Vector3 rightLimit = Quaternion.AngleAxis(angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistancePersuit));

        Vector3 leftLimit = Quaternion.AngleAxis(-angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistancePersuit));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceToHit);

        Vector3 rightLimit3 = Quaternion.AngleAxis(angleToHit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit3 * distanceToHit));

        Vector3 leftLimit3 = Quaternion.AngleAxis(-angleToHit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit3 * distanceToHit));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit2 = Quaternion.AngleAxis(angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceAttack));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceAttack));

        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);
    }
}
