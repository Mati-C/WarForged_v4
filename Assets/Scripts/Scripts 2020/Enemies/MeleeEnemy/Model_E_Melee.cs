using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Model_E_Melee : ClassEnemy
{

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

    [Header("Enemy Attack Variables:")]

    public int ID_Attack;
   
    [Range(10, 100)]
    public int singleAttackProbability;

    public float attackDamageLight;
    public float attackDamageHeavy;
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
        if (onDamageTime <= 0) attackFinish = true;      
    }

  

    void Start()
    {
        _view = GetComponent<Viewer_E_Melee>();
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
        ia_Manager = FindObjectOfType<IA_CombatManager>();
        playerFireSowrd = FindObjectOfType<FireSword>();
        exp = playerFireSowrd.warriorExp;
        enemyLayer = layersCanSee;

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");
        var takeDamage = new N_FSM_State("TAKE_DAMAGE");
        var die = new N_FSM_State("DIE");
        var blocked = new N_FSM_State("BLOCKED");

        IdleEvent += _view.AnimIdleCombat;
        WalkEvent += _view.AnimWalkCombat;
        RunEvent += _view.AnimRunCombat;
        RetreatEvent += _view.AnimRetreat;
        WalkRightEvent += _view.AnimWalkRight;
        WalkLeftEvent += _view.AnimWalkLeft;
        ComboAttackEvent += _view.AnimComboAttack;
        HeavyAttackEvent += _view.AnimHeavyAttack;
        HeavyAttackEvent += _view.HeavyHitAntisipation;
        GetHitEvent += _view.AnimGetHit;
        BlockedEvent += _view.BlockedAnim;
        KnockedEvent += _view.KnockedAnim;
        DieEvent += _view.AnimDie;

        StartCoroutine(MoveOnAttack());      

        patrol.OnEnter += () =>
        {
            onPatrol = true;
            RestartDistances_Angles();
            enemyLayer = layersCanSee;
        };

        patrol.OnUpdate += () =>
        {
            var distancePH_patrol = Vector3.Distance(transform.position, patrolPosition);
            float distancePortalPH = 0;

            if (portal)
            {
                distancePortalPH = Vector3.Distance(transform.position, portal.phPortal.position);

                if (distancePortalPH > 1 && portalOrder)
                {
                    WalkEvent();
                    MoveToTarget(portal.phPortal.position);
                }

                if (distancePortalPH <= 1 && portalOrder)
                {
                    portalOrder = false;
                    IdleEvent();
                    Quaternion targetRotation = Quaternion.LookRotation(patrolForward, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
                }
            }

            if (distancePH_patrol > 1 && !portalOrder)
            {
                WalkEvent();
                MoveToTarget(patrolPosition);
            }

            if (distancePH_patrol <= 1 && !portalOrder)
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

            if (aggressiveLevel == 1) viewDistanceSurround = 5.5f;

            WalkEvent();
            player.CombatStateUp();

            if (aggressiveLevel == 2) viewDistanceSurround = 8f;

            if (aggressiveLevel == 1) MoveToTarget(FindNearAggressiveNode().transform.position);

            else MoveToTarget(FindNearNon_AggressiveNode().transform.position);
           
            if(PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if(onDamageTime >0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

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
           
            if (aggressiveLevel == 1) viewDistanceSurround = 6f;

            if (aggressiveLevel == 2) viewDistanceSurround = 10f;

            player.CombatStateUp();

            surroundTimer -= Time.deltaTime;

            if(!ia_Manager.enemyMeleePermisionAttack && !cantAskAgain && ia_Manager.enemiesListOnAttack.Count <=0) timeToAttack -= Time.deltaTime;

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

            if(surroundTimer <=0)
            {
                surroundBehaviourID = UnityEngine.Random.Range(0, 3);

                surroundTimer = UnityEngine.Random.Range(surroundTimerMin, surroundTimerMax);
            }

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (!canSurround && canPersuit && timeToAttack >0 && life >0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack <=0 && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        surround.OnExit += () =>
        {
            if (aggressiveLevel == 1) viewDistanceSurround = 4.5f;

            if (aggressiveLevel == 2) viewDistanceSurround = 7f;
        };

        attack.OnEnter += () =>
        {
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

            int r = UnityEngine.Random.Range(1, 101);

            if (r < singleAttackProbability) ID_Attack = 1;
            else ID_Attack = 2;
        };

        attack.OnUpdate += () =>
        {
            player.CombatStateUp();

            if (!onAttackAnimation && !canAttack && !waitingForRetreat)
            {
                var d = Vector3.Distance(transform.position, player.transform.position);
                if (d > 0.5f) RunEvent();

                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

                MoveToTarget(player.transform.position);
            }

            if(canAttack && !onAttackAnimation && !waitingForRetreat)
            {
                waitingForRetreat = true;

                if (ID_Attack == 1) ComboAttackEvent();

                else HeavyAttackEvent();

                Vector3 _dir = Vector3.zero;
                Quaternion targetRotation;
                _dir = (player.transform.position - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

                if (ID_Attack == 1) StartCoroutine(OnAttackAnimationCorrutine(2));

                else StartCoroutine(OnAttackAnimationCorrutine(1.2f));
            }

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (attackFinish && life > 0) myFSM_EventMachine.ChangeState(retreat);

            if (onDamageTime > 0 && life > 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

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

            if (attackFinish)
            {
                if (ID_Attack == 1) StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission + 1.2f, true)); 

                if (ID_Attack == 2) StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission + 2, true)); 
            }

            
            else StartCoroutine(ReturnIA_Manager(TimeToRrturnPermission, true));

            ID_Attack = 0;

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

            var d = Vector3.Distance(transform.position, player.transform.position);

            if (d > 2) timeToRetreat = 0;

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

            if (PlayerOnGrid()) myFSM_EventMachine.ChangeState(patrol);

            if (timeToRetreat <=0 && canPersuit && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToRetreat <=0 && canSurround && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (onDamageTime > 0 && life> 0) myFSM_EventMachine.ChangeState(takeDamage);

            if (blockedAttack && life > 0) myFSM_EventMachine.ChangeState(blocked);

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

            if (PlayerOnGrid() && onDamageTime <=0) myFSM_EventMachine.ChangeState(patrol);

            if (canPersuit && !canSurround && onDamageTime <= 0 && life >0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 &&  canSurround &&  onDamageTime <= 0 && life >0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && onDamageTime <=0 && life>0) myFSM_EventMachine.ChangeState(attack);

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

            if (PlayerOnGrid() && !blockedAttack) myFSM_EventMachine.ChangeState(patrol);

            if (canPersuit && !canSurround && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(persuit);

            if (timeToAttack > 0 && canSurround && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(surround);

            if (timeToAttack <= 0 && !blockedAttack && life > 0) myFSM_EventMachine.ChangeState(attack);

            if (onDamageTime > 0 && life > 0 && !blockedAttack) myFSM_EventMachine.ChangeState(takeDamage);

            if (life <= 0) myFSM_EventMachine.ChangeState(die);
        };

        blocked.OnExit += () =>
        {
            timeToRetreat = 0;
        };

        die.OnEnter += () =>
        {
            
        };

        myFSM_EventMachine = new N_FSM_EventMachine(patrol);
    }

    
    void Update()
    {
       
        canPersuit = CanSee(player.transform, viewDistancePersuit, angleToPersuit, enemyLayer);

        canSurround = CanSee(player.transform, viewDistanceSurround, angleToSurround, enemyLayer);

        canAttack = CanSee(player.transform, viewDistanceAttack, 360, enemyLayer);

        myFSM_EventMachine.Update();
    }

    public void PlusAttackMove(float t) { _timeToMoveOnAttack = t; }

    public override void Resume()
    {
        StartCoroutine(OnDamageTimer());
        StartCoroutine(MoveOnAttack());
        _view.StartCoroutine(_view.DamageTimerAnim());
    }

    public override void GetDamage(float d, Model_Player.DamageType t)
    {
        life -= d;
        if (player.flamesOn) StartBurning();

        if (life <= 0)
        {
            RuturnIA_ManagerInstant(true);
            playerFireSowrd.SwordExp(exp);
            DieEvent();
        }

        else
        {
            _view.CreatePopText(d);

            if (!onAttackAnimation)
            {
                GetHitEvent();
                onDamageTime = damageDelayTime;
            }
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

    public void MakeDamage()
    {
        var p = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<Model_Player>()).Select(x => x.GetComponent<Model_Player>());

        if(p.Count() >0)
        {
            if (CanSee(p.First().transform, viewDistanceAttack, angleToAttack, layersCanSee)) p.First().GetDamage(attackDamageLight, transform, Model_Player.DamageType.Light);
        } 

    }

    public void MakeHeavyDamage()
    {
        var p = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<Model_Player>()).Select(x => x.GetComponent<Model_Player>());

        if (p.Count() > 0)
        {
            if (CanSee(p.First().transform, viewDistanceAttack, angleToAttack, layersCanSee)) p.First().GetDamage(attackDamageLight, transform, Model_Player.DamageType.Heavy);
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
