﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.AI;
using Sound;

public class ModelE_Sniper : EnemyEntity
{
    public enum EnemyInputs { PATROL, PERSUIT, ATTACK, MELEE_ATTACK, FOLLOW, DIE, ANSWER, RETREAT, STUNED }
    private EventFSM<EnemyInputs> _myFsm;
    public ViewerE_Sniper view;
    public Transform attackPivot;
    public float timeToShoot;
    public EnemyAmmo munition;
    public float distanceToMeleeAttack;
    public float angleToMeleeAttack;
    public bool onMeleeAttack;
    public float timeToMeleeAttack;
    public bool cooldwonToGoBack;
    public float timeToRetreat;
    public float maxTimeToRetreat;
    public float maxTimeDelayMeleeAttack;
    public float minTimeDelayMeleeAttack;
    public float timeToMove;
    public bool changeMovementOnAttack;
    public Transform scapeNodesParent;
    public int dirToMoveOnAttack;
    public string animClipName;
    public float maxTimeToAttack;
    public float minTimeToAttack;

    public Action TakeDamageEvent;
    public Action DeadEvent;
    public Action AttackEvent;
    public Action AttackMeleeEvent;
    public Action StunedEvent;
    public Action FlyRightEvent;
    public Action FlyLeftEvent;
    public Action ChatEvent;
	
	public float maxLife;

    Vector3 startPos;

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

    public void Awake()
    {

        startPos = transform.position;
        var myEntites = FindObjectsOfType<EnemyEntity>().Where(x => x != this && x.EnemyID_Area == EnemyID_Area);
        nearEntities.Clear();
        nearEntities.AddRange(myEntites);
        ca = FindObjectsOfType<CombatArea>().Where(x => x.EnemyID_Area == EnemyID_Area).FirstOrDefault();

        navMeshAgent = GetComponent<NavMeshAgent>();
        playerNodes.AddRange(FindObjectsOfType<CombatNode>());
        rb = gameObject.GetComponent<Rigidbody>();
        view = GetComponent<ViewerE_Sniper>();
        munition = FindObjectOfType<EnemyAmmo>();
        timeToShoot = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);
        timeToMeleeAttack = UnityEngine.Random.Range(minTimeDelayMeleeAttack, maxTimeDelayMeleeAttack);
        timeToStopBack = UnityEngine.Random.Range(3, 4);
		maxLife = life;


        var patrol = new FSM_State<EnemyInputs>("PATROL");
        var persuit = new FSM_State<EnemyInputs>("PERSUIT");
        var attack = new FSM_State<EnemyInputs>("ATTACK");
        var melee_attack = new FSM_State<EnemyInputs>("MELEE_ATTACK");
        var die = new FSM_State<EnemyInputs>("DIE");
        var follow = new FSM_State<EnemyInputs>("FOLLOW");
        var answerCall = new FSM_State<EnemyInputs>("ANSWER");
        var retreat = new FSM_State<EnemyInputs>("RETREAT");
        var stuned = new FSM_State<EnemyInputs>("STUNED");

        TakeDamageEvent += view.TakeDamageAnim;
        DeadEvent += view.DeadAnim;
        AttackEvent += view.AttackRangeAnim;
        AttackMeleeEvent += view.AttackMeleeAnim;
        IdleEvent += view.IdleAnim;
        MoveEvent += view.MoveFlyAnim;
        StunedEvent += view.StunedAnim;
        FlyRightEvent += view.FlyRightAnim;
        FlyLeftEvent += view.FlyLeftAnim;
        ChatEvent += view.ChangeChatAnimation;

        StateConfigurer.Create(patrol)
           .SetTransition(EnemyInputs.PERSUIT, persuit)
           .SetTransition(EnemyInputs.ANSWER, answerCall)
           .SetTransition(EnemyInputs.ATTACK, attack)
           .SetTransition(EnemyInputs.MELEE_ATTACK, melee_attack)
           .SetTransition(EnemyInputs.FOLLOW, follow)
           .SetTransition(EnemyInputs.DIE, die)
           .Done();

        StateConfigurer.Create(retreat)
          .SetTransition(EnemyInputs.PERSUIT, persuit)
          .SetTransition(EnemyInputs.ATTACK, attack)
          .SetTransition(EnemyInputs.MELEE_ATTACK, melee_attack)
          .SetTransition(EnemyInputs.FOLLOW, follow)
          .SetTransition(EnemyInputs.STUNED, stuned)
          .SetTransition(EnemyInputs.PATROL, patrol)
          .SetTransition(EnemyInputs.DIE, die)
          .Done();

        StateConfigurer.Create(persuit)
         .SetTransition(EnemyInputs.ATTACK, attack)
         .SetTransition(EnemyInputs.MELEE_ATTACK, melee_attack)
         .SetTransition(EnemyInputs.FOLLOW, follow)
          .SetTransition(EnemyInputs.PATROL, patrol)
         .SetTransition(EnemyInputs.STUNED, stuned)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(attack)
         .SetTransition(EnemyInputs.MELEE_ATTACK, melee_attack)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.FOLLOW, follow)
          .SetTransition(EnemyInputs.PATROL, patrol)
         .SetTransition(EnemyInputs.RETREAT, retreat)
         .SetTransition(EnemyInputs.STUNED, stuned)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(melee_attack)
         .SetTransition(EnemyInputs.ATTACK, attack)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.FOLLOW, follow)
         .SetTransition(EnemyInputs.RETREAT, retreat)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(stuned)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.ATTACK, attack)
          .SetTransition(EnemyInputs.PATROL, patrol)
         .SetTransition(EnemyInputs.FOLLOW, follow)
         .SetTransition(EnemyInputs.RETREAT, retreat)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(follow)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.ATTACK, attack)
          .SetTransition(EnemyInputs.PATROL, patrol)
         .SetTransition(EnemyInputs.STUNED, stuned)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(answerCall)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.ATTACK, attack)
         .SetTransition(EnemyInputs.PATROL, patrol)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(die)
        .SetTransition(EnemyInputs.PATROL, patrol)
        .Done();

        patrol.OnEnter += x =>
        {
           
        };

        patrol.OnFixedUpdate += () =>
        {
            if (myPointer) target.ReturnPointer(myPointer);

            healthBar.SetActive(false);
          
            if(patroling) currentAction = new A_SniperPatrol(this);

            if (chating) currentAction = new A_Chating(this);

            if (!isDead && isPersuit && !isWaitArea && target.fadeTimer > target.view.fadeTime) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isAnswerCall && target.fadeTimer > target.view.fadeTime) SendInputToFSM(EnemyInputs.ANSWER);

            if (!isDead && isWaitArea && target.fadeTimer > target.view.fadeTime) SendInputToFSM(EnemyInputs.ATTACK);

            if (!isDead && !isWaitArea && !isPersuit && !onMeleeAttack && onDamage && target.fadeTimer > target.view.fadeTime) SendInputToFSM(EnemyInputs.FOLLOW);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);


        };

        patrol.OnExit += x =>
        {
            angleToPersuit = 180;
            onCombat = true;
        };

        answerCall.OnEnter += x =>
        {

        };

        answerCall.OnFixedUpdate += () =>
        {

            angleToPersuit = 180;
            currentAction = new A_FollowTarget(this);
            if (!onDamage) MoveEvent();
            if (!isDead && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isWaitArea) SendInputToFSM(EnemyInputs.ATTACK);
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
            MoveEvent();
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
        };

        persuit.OnFixedUpdate += () =>
        {
            navMeshAgent.enabled = false;

            isAnswerCall = false;

            firstSaw = true;

            foreach (var item in nearEntities) if (!item.isAnswerCall && !item.firstSaw) item.isAnswerCall = true;

            if (!onDamage) MoveEvent();

            currentAction = new A_Persuit(this);

          //  if (!isDead && onMeleeAttack) SendInputToFSM(EnemyInputs.MELEE_ATTACK);

            if (!isDead && isWaitArea) SendInputToFSM(EnemyInputs.ATTACK);

            if (!isDead && !isWaitArea && !isPersuit) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && isStuned) SendInputToFSM(EnemyInputs.STUNED);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };

        attack.OnEnter += x =>
        {
            IdleEvent();

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
        };

        attack.OnUpdate += () =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            if (myPointer && timeToShoot< 1.5f) myPointer.StartAdvertisement();

            isAnswerCall = false;

            firstSaw = true;

            foreach (var item in nearEntities) if (!item.isAnswerCall && !item.firstSaw) item.isAnswerCall = true;

            timeToShoot -= Time.deltaTime;

            currentAction = new A_SniperAttack(this);

            var d = Vector3.Distance(transform.position, target.transform.position);

            if (timeToRetreat <= 0 && d <= 1.5f) onRetreat = true;

            else onRetreat = false;

            // if (!isDead && onMeleeAttack) SendInputToFSM(EnemyInputs.MELEE_ATTACK);

            if (!isDead && !isWaitArea && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isWaitArea && !isPersuit) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && isStuned) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && onRetreat && timeToRetreat <= 0 && d<=1.5) SendInputToFSM(EnemyInputs.RETREAT);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

        };

        stuned.OnEnter += x =>
        {
            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }
            timeStuned = 3;
            StunedEvent();
        };

        stuned.OnUpdate += () =>
        {
            timeStuned -= Time.deltaTime;

            currentAction = new A_Idle();

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

            if (!isDead && !isWaitArea && isPersuit && timeStuned <=0) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isWaitArea && !isPersuit && timeStuned <= 0) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && onRetreat && timeToRetreat < 0 && timeStuned <= 0) SendInputToFSM(EnemyInputs.RETREAT);

            if (!isDead && isWaitArea && timeStuned <=0 ) SendInputToFSM(EnemyInputs.ATTACK);

        };

        stuned.OnExit += x =>
        {
            isStuned = false;
            view.StunedAnimFalse();
        };

        melee_attack.OnUpdate += () =>
        {

            isAnswerCall = false;

            firstSaw = true;

            foreach (var item in nearEntities) if (!item.isAnswerCall && !item.firstSaw) item.isAnswerCall = true;

            timeToMeleeAttack -= Time.deltaTime;

            timeToRetreat -= Time.deltaTime;

            currentAction = new A_SniperMeleeAttack(this);

            if (!isDead && isWaitArea && !onRetreat) SendInputToFSM(EnemyInputs.ATTACK);

            if (!isDead && !isWaitArea && isPersuit && !onRetreat && timeToRetreat < 0) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isWaitArea && !isPersuit && !onRetreat && timeToStopBack<=0 && timeToRetreat < 0) SendInputToFSM(EnemyInputs.FOLLOW);
          

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };

        retreat.OnEnter += x =>
        {
            view.anim.SetBool("Retreat", true);

            MoveEvent();

            if (navMeshAgent)
            {
                if (navMeshAgent.enabled) navMeshAgent.enabled = false;
            }

            timeToStopBack = UnityEngine.Random.Range(5, 6);

            positionToBack = FindNearScapeNode();

            timeToRetreat = maxTimeToRetreat;
        };

        retreat.OnUpdate += () =>
        {
            if (myPointer) myPointer.StopAdvertisement();

            view.BackFromAttackRange();

            timeToStopBack -= Time.deltaTime;

            currentAction = new A_GoBackFromAttack(this);

            if (!isDead && isPersuit && !isWaitArea && !onRetreat) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isWaitArea && !onRetreat) SendInputToFSM(EnemyInputs.ATTACK);

            // if (!isDead && onMeleeAttack && !onRetreat) SendInputToFSM(EnemyInputs.MELEE_ATTACK);

            if (!isDead && isStuned) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && !isWaitArea && !isPersuit && !onMeleeAttack && !onRetreat) SendInputToFSM(EnemyInputs.FOLLOW);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };

        retreat.OnExit += x =>
        {
            view.anim.SetBool("Retreat", false);
        };

        follow.OnEnter += x =>
        {
            MoveEvent();
        };


        follow.OnUpdate += () =>
        {

            currentAction = new A_FollowTarget(this);

            if (!onDamage) MoveEvent();

            if (!isDead && !isWaitArea && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isStuned) SendInputToFSM(EnemyInputs.STUNED);

            if (!isDead && isWaitArea) SendInputToFSM(EnemyInputs.ATTACK);
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
            if(myPointer)
            {
                target.ReturnPointer(myPointer);
                myPointer = null;
            }
            healthBar.SetActive(false);
            navMeshAgent.enabled = false;
            DeadEvent();
            currentAction = null;
            SoundManager.instance.PlayRandom(SoundManager.instance.deathVoice, transform.position, true, 0.9f);

            if (nearEntities.Count > 0)
            {
                foreach (var item in nearEntities)
                {
                    RemoveNearEntity(item);
                }
            }

            ca.myEntities--;
        };


        _myFsm = new EventFSM<EnemyInputs>(patrol);
    }


    public void SendInputToFSM(EnemyInputs inp)
    {
        _myFsm.SendInput(inp);
    }

    public void Start()
    {
        if (!firstEnemyToSee) gameObject.SetActive(false);
    }

    private void Update()
    {
        _myFsm.Update();

        animClipName = view.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        entitiesAvoidVect = EntitiesAvoidance();

        if (target != null && SearchForTarget.SearchTarget(target.transform, viewDistancePersuit, angleToPersuit, transform, true, layerObst)) isPersuit = true;
        else isPersuit = false;

        if (target != null &&  SearchForTarget.SearchTarget(target.transform, viewDistanceAttack, angleToAttack, transform, true, layerObst)) isWaitArea = true;
        else isWaitArea = false;

        if (target != null && SearchForTarget.SearchTarget(target.transform, distanceToMeleeAttack, angleToMeleeAttack, transform, true, layerObst)) onMeleeAttack = true;
        else onMeleeAttack = false;


        if (renderObject.isVisible && myPointer)
        {
            target.ReturnPointer(myPointer);
            myPointer = null;
        }

        if (!renderObject.isVisible && !myPointer && !isDead && onCombat)
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
    
    public void Shoot()
    {

        if (!onRetreat)
        {
            Arrow newArrow = munition.arrowsPool.GetObjectFromPool();
            newArrow.gameObject.SetActive(false);
            newArrow.ammoAmount = munition;
            newArrow.owner = this;
            newArrow.transform.position = attackPivot.position;
            var dir = (target.transform.position - newArrow.transform.position).normalized;
            dir.y = 0;
            newArrow.fireBallParticles.SetActive(true);
            newArrow.gameObject.SetActive(true);
            newArrow.transform.forward = dir;
            timeToShoot = UnityEngine.Random.Range(minTimeToAttack, maxTimeToAttack);
            view.BackFromAttackRange();
            SoundManager.instance.Play(Entity.FIREBALL, transform.position, true, 0.5f);
        }
    }

    public override Vector3 EntitiesAvoidance()
    {
        var obs = Physics.OverlapSphere(transform.position, 1, layerEntites);
        if (obs.Count() > 0)
        {
            var dir = transform.position - obs.First().transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;
    }

    public override void StartPursuit()
    {
        SendInputToFSM(EnemyInputs.PERSUIT);
    }

    public override void GetDamage(float damage, DamageType typeOfDamage, int damageAnimationIndex)
    {
        foreach (var item in nearEntities)
            item.StartPursuit();

        timeOnDamage = 1f;
        if (!onDamage)
        {
            onDamage = true;
            StartCoroutine(OnDamageCorrutine());
        }

        if (typeOfDamage == EnemyEntity.DamageType.Normal || typeOfDamage == DamageType.Proyectile)
        {
            timeStuned = 0;
            TakeDamageEvent();
            life -= damage;
            view.LifeBar(life / maxLife);
            view.CreatePopText(damage);
        }

        if (typeOfDamage == EnemyEntity.DamageType.Stune)
        {
            isStuned = true;
            life -= damage;
            view.LifeBar(life / maxLife);
            view.CreatePopText(damage);
        }

        SoundManager.instance.Play(Entity.BODY_IMPACT_2, transform.position, true);

        if (life <= 0)
        {
            isDead = true;
            SendInputToFSM(EnemyInputs.DIE);
        }
    }

    public Vector3 FindNearScapeNode()
    {
      
        var nodes = scapeNodesParent.GetComponentsInChildren<Transform>().OrderByDescending(x=> 
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            return d;
        });

        return nodes.First().position;
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

    public override Node GetRandomNode()
    {
        var r = UnityEngine.Random.Range(0, myNodes.Count);
        return myNodes[r];
    }


    public override Vector3 ObstacleAvoidance()
    {
        var obs = Physics.OverlapSphere(transform.position, 1, layerObstAndBarrels);
        if (obs.Count() > 0)
        {
            var dir = transform.position - obs.First().transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;
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
        Gizmos.DrawWireSphere(transform.position, distanceToMeleeAttack);

        Vector3 rightLimit2 = Quaternion.AngleAxis(angleToMeleeAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * distanceToMeleeAttack));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-angleToMeleeAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * distanceToMeleeAttack));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit3 = Quaternion.AngleAxis(angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit3 * viewDistanceAttack));

        Vector3 leftLimit3 = Quaternion.AngleAxis(-angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit3 * viewDistanceAttack));

        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);
    }

 
    public override void MakeDamage()
    {
        var player = Physics.OverlapSphere(attackPivot.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();
        if (player != null)
        {          
            player.GetDamage(attackDamage, transform, false, Model.DamagePlayerType.Normal, this);
        }
    }

    public void OnDamageFalse()
    {
        onDamage = false;
    }

    public override void RemoveNearEntity(EnemyEntity e)
    {
        e.nearEntities.Remove(this);
    }

    public override CombatNode FindNearAggressiveNode()
    {
        var node = playerNodes.Where(x => x.aggressive && !x.isBusy && (x.myOwner == null || x.myOwner == this)).OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            return d;

        }).FirstOrDefault();

        myCombatNode = node;

      //  myCombatNode.myOwner = this;

        if (lastCombatNode == null)
        {
            lastCombatNode = node;
           // lastCombatNode.myOwner = this;
        }

        if (myCombatNode != lastCombatNode)
        {
            lastCombatNode.myOwner = null;
            lastCombatNode = myCombatNode;
        }

        var NearNodes = Physics.OverlapSphere(myCombatNode.transform.position, 1).Where(y => y.GetComponent<CombatNode>()).Select(y => y.GetComponent<CombatNode>()).Where(y => y.myOwner == null);

        foreach (var item in NearNodes)
        {
          //  item.myOwner = this;
        }

        if (NearNodes.Count() > 0)
        {

            var restOfNodes = new List<CombatNode>();

            restOfNodes.AddRange(playerNodes);

            restOfNodes.RemoveAll(y => NearNodes.Contains(y));

            foreach (var item in restOfNodes)
            {
                if (item.myOwner == this) item.myOwner = null;
            }

        }

        return node;
    }

    public override CombatNode FindNearNon_AggressiveNode()
    {
        var node = playerNodes.Where(x => x.Non_Aggressive && !x.isBusy && (x.myOwner == null || x.myOwner == this)).OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            return d;

        }).FirstOrDefault();

        myCombatNode = node;

      //  myCombatNode.myOwner = this;

        if (lastCombatNode == null)
        {
            lastCombatNode = node;
          //  lastCombatNode.myOwner = this;
        }

        if (myCombatNode != lastCombatNode)
        {
            lastCombatNode.myOwner = null;
            lastCombatNode = myCombatNode;
        }

        var NearNodes = Physics.OverlapSphere(myCombatNode.transform.position, 1).Where(y => y.GetComponent<CombatNode>()).Select(y => y.GetComponent<CombatNode>()).Where(y => y.myOwner == null);

        foreach (var item in NearNodes)
        {
           // item.myOwner = this;
        }

        if (NearNodes.Count() > 0)
        {

            var restOfNodes = new List<CombatNode>();

            restOfNodes.AddRange(playerNodes);

            restOfNodes.RemoveAll(y => NearNodes.Contains(y));

            foreach (var item in restOfNodes)
            {
                if (item.myOwner == this) item.myOwner = null;
            }

        }

        return node;
    }

    void OnDestroy()
    {
        target.ReturnPointer(myPointer);
        myPointer = null;
    }

    public override void Respawn()
    {
        transform.position = startPos;
        life = maxLife;
        view.LifeBar(life / maxLife);
        view.anim.SetBool("IsDead", false);
        view.anim.SetBool("IdleCombat", true);
        view.BackFromAttackRange();
        view.BackFromDamage();
        onDamage = false;
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
}
