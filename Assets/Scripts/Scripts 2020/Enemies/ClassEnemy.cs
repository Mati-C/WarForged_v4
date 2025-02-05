﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public abstract class ClassEnemy : MonoBehaviour
{

    ClassEnemyViewer _viewer;
    public Portal portal;
    public N_FSM_EventMachine myFSM_EventMachine;
    public List<Node2> nodes = new List<Node2>();
    public List<Node2> pathToTarget = new List<Node2>();
    public Pathfinding pathfinding;
    public Model_Player player;
    public Grid grid;
    public Rigidbody rb;
    public IA_CombatManager ia_Manager;
    public FireSword playerFireSowrd;
    public PlayerCamera playerCamera;

    [Header("EnemyRoom ID:")]

    public float ID;
    public LayerMask enemyLayer;
    public LayerMask layersCanSee;
    public LayerMask layersObstacles;
    public LayerMask layersPlayer;

    public List<ClassEnemy> sameID_Enemies = new List<ClassEnemy>();

    [Header("Enemy CombatNodes:")]

    public CombatNode myCombatNode;
    public CombatNode lastCombatNode;
    public List<CombatNode> playerNodes = new List<CombatNode>();
    public float distanceToOcupateNodes;

    [Header("Enemy Stats:")]

    public bool range;
    public bool melee;
    public bool boss;
    public float life;
    public float maxLife;
    public float exp;
    public float speed;
    public int aggressiveLevel;

    [Header("Enemy States:")]

    public bool isDead;
    public bool canPersuit;
    public bool canSurround;
    public bool onPatrol;
    public bool OnDamage;
    public bool blockedAttack;
    public bool burning;
    public bool knocked;
    public bool permissionToAttack;
    public bool cantAskAgain;
    public bool portalOrder;
    public bool dontMove;
    public int currentIndex;

    [Header("EnemyClass Attack Variables:")]

    public float timeToAttack;
    public float maxTimeToAttack;
    public float minTimeToAttack;
    public float TimeToRrturnPermission;

    [Header("EnemyClass Persuit Variables:")]
     public float viewDistancePersuit;
     public float angleToPersuit;
    float _startAngleToPersuit;
    float _startDistanceToPersuit;

    [Header("EnemyClass Surround Variables:")]
    public float viewDistanceSurround;
    public float angleToSurround;
    public int surroundBehaviourID;
    public bool cantAvoid;
    float _startAngleToSurround;
    float _startDistanceToSurround;

    [Header("EnemyClass Patrol Variables:")]
    public Vector3 patrolPosition;
    public Vector3 patrolForward;


    [Header("Enemy GetHit Variables:")]
    public float onDamageTime;
    public float damageDelayTime;
    public float timeBurning;

    public Action GetHitEvent;
    public Action DieEvent;
    public Action BlockedEvent;
    public Action KnockedEvent;
    public N_FSM_State patrolState;

    bool _burnSound;
    AudioSource _burnAudio;

    public void ReturnToLife()
    {
        isDead = false;
        life = maxLife;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        PatrolState();
        _viewer.ReturnToIdleAnim();
    }

    public abstract void PatrolState();

    public bool NearEnemy()
    {
        
        var enemies = Physics.OverlapSphere(transform.position, 1f).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>()).Where(x => x != this);

        if (enemies.Count() > 0) return true;

        else return false;
    }

    public IEnumerator AvoidNearEntity()
    {

        cantAvoid = true;
        switch (surroundBehaviourID)
        {
            case 1:
                surroundBehaviourID = 2;
                break;

            case 2:
                surroundBehaviourID = 1;
                break;
        }

        yield return new WaitForSeconds(1.7f);
        cantAvoid = false;
    }

    public void ReturnIA_Manager()
    {
        ia_Manager.PermissionsMelee(false);
        ia_Manager.DecisionTakeMelee(false);
        ia_Manager.PermissionsRange(false);
        ia_Manager.DecisionTakeRange(false);
        permissionToAttack = false;
    }

    public IEnumerator ReturnIA_Manager(float time, bool b)
    {
        yield return new WaitForSeconds(time);

        if (permissionToAttack)
        {
            StartCoroutine(CanAttackAgain());
            if (b)
            {
                ia_Manager.PermissionsMelee(false);
                ia_Manager.DecisionTakeMelee(false);
            }
            else
            {
                ia_Manager.PermissionsRange(false);
                ia_Manager.DecisionTakeRange(false);
            }
            permissionToAttack = false;
        }
    }

    public void RuturnIA_ManagerInstant(bool b)
    {

        if (permissionToAttack)
        {
            cantAskAgain = false;
            if (b)
            {
                ia_Manager.PermissionsMelee(false);
                ia_Manager.DecisionTakeMelee(false);
            }
            else
            {
                ia_Manager.PermissionsRange(false);
                ia_Manager.DecisionTakeRange(false);
            }
            permissionToAttack = false;
        }
    }

    public IEnumerator CanAttackAgain()
    {
        cantAskAgain = true;
        yield return new WaitForSeconds(0.5f);
        cantAskAgain = false;
    }

    public IEnumerator BurnDamageTimer()
    {
        burning = true;
        float tic = 1;
        timeBurning = playerFireSowrd.fireSwordBurnTimeOnEnemy;
        _viewer.BurnOn_Off(true);
        if (!_burnSound)
        {
            _burnAudio = SoundManager.instance.PlayAndCreate(Objects.CHECKPOINT_IDLE, transform.position, true);
            _burnSound = true;
        }
       
        while (timeBurning > 0)
        {
            timeBurning -= Time.deltaTime;
            tic -= Time.deltaTime;
            if (tic <= 0)
            {              
                _viewer.CreatePopText(playerFireSowrd.fireSwordBurnDamage);
                life -= playerFireSowrd.fireSwordBurnDamage;
                if(!boss)_viewer.UpdateLifeBar(life / maxLife);
                tic = 1;
                if(life<= 0 && !isDead)
                {
                    isDead = true;
                    if (portal) portal.PortalRemove();
                    RuturnIA_ManagerInstant(true);
                    _viewer.CreateExpPopText(exp);
                    playerFireSowrd.SwordExp(exp);
                    DieEvent();
                    timeBurning = 0;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        _burnSound = false;
        _burnAudio.Stop();
        Destroy(_burnAudio.gameObject);
        _viewer.BurnOn_Off(false);
        burning = false;
    }

    public void StartBurning()
    {
        timeBurning = playerFireSowrd.fireSwordBurnTimeOnEnemy;
        if (!burning) StartCoroutine(BurnDamageTimer());
    }


    public void PushKnocked()
    {
        var dir = (player.transform.position - transform.position).normalized;
        rb.AddForce(-dir * 1.5f, ForceMode.Impulse);
        StartCoroutine(BlockedState(3.4f));
        _viewer.PushKnockedAnim();
    }

    public IEnumerator OnDamageTimer()
    {
        while(true)
        {
            if(onDamageTime > 0)
            {
                onDamageTime -= Time.deltaTime;
                OnDamage = true;
            }

            if (onDamageTime <= 0) OnDamage = false;

            yield return new WaitForEndOfFrame();
        }
    }

    public abstract void Resume();

    private void Awake()
    {

        portal = FindObjectsOfType<Portal>().Where(x => x.ID == ID).FirstOrDefault();
        grid = FindObjectsOfType<Grid>().Where(x => x.ID == ID).First();
        player = FindObjectOfType<Model_Player>();
        pathfinding = GetComponent<Pathfinding>();
        playerNodes.AddRange(FindObjectsOfType<CombatNode>());
        sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != this));
        _viewer = GetComponent<ClassEnemyViewer>();
        _viewer.GetMeshes();
        _viewer.AwakeViewer();
        rb = GetComponent<Rigidbody>();
        patrolPosition = transform.position;
        patrolForward = transform.forward;
        patrolForward.y = 0;
        playerCamera = FindObjectOfType<PlayerCamera>();

        _startDistanceToPersuit = viewDistancePersuit;
        _startAngleToPersuit = angleToPersuit;
        _startDistanceToSurround = viewDistanceSurround;
        _startAngleToSurround = angleToSurround;
       
        StartCoroutine(OnDamageTimer());
    }

    public void RestartDistances_Angles()
    {
        viewDistancePersuit = _startDistanceToPersuit;
        angleToPersuit = _startAngleToPersuit;
        viewDistanceSurround = _startDistanceToSurround;
        angleToSurround = _startAngleToSurround;
        life = maxLife;
    }

    void RemoveSameID_Enemy(ClassEnemy e)
    {
        sameID_Enemies.Remove(e);
    }

    public void BlockedAttack()
    {
        BlockedEvent();
        StartCoroutine(BlockedState(1.2f));
    }

    public void Knocked()
    {
        KnockedEvent();
        StartCoroutine(KnockedMovement());
        StartCoroutine(BlockedState(4.3f));
    }

    IEnumerator KnockedMovement()
    {
        knocked = true;
        yield return new WaitForSeconds(0.6f);
        var t = 0.4f;
        while(t >0)
        {
            t -= Time.deltaTime;
            rb.MovePosition(transform.position + player.transform.forward * 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(3.3f);
        knocked = false;
    }

    IEnumerator BlockedState(float t)
    {
        blockedAttack = true;
        yield return new WaitForSeconds(t);
        blockedAttack = false;
    }

    public abstract void GetDamage(float d, Model_Player.DamageType t);

    private Vector3 FindNearNode(Vector3 pos)
    {
        var n = nodes.OrderBy(x =>
        {
            var distance = Vector3.Distance(x.worldPosition, pos);
            return distance;
        });


        return n.First().worldPosition;

    }

    public void FindPath(Vector3 endPos)
    {
        pathfinding.FindPath(FindNearNode(transform.position), FindNearNode(endPos));
    }

    public bool PlayerOnGrid()
    {
        Vector3 targetNearestNode = Vector3.zero;
        targetNearestNode = FindNearNode(player.transform.position);
        targetNearestNode.y = 0;
        var playerPos = player.transform.position;
        playerPos.y = 0;

        var distance = Vector3.Distance(targetNearestNode, playerPos);

        if (distance > 1.5f) return true;
        
        else return false;

    }

    public void MoveToTarget(Vector3 target)
    {
        FindPath(target);

        pathToTarget.Clear();
        pathToTarget.AddRange(pathfinding.myPath);

        if (currentIndex < pathToTarget.Count)
        {
            float d = Vector3.Distance(pathToTarget[currentIndex].worldPosition, transform.position);

            var _dir = Vector3.zero;

            if (d >= 0.4f)
            {
                Quaternion targetRotation;
                _dir = (pathToTarget[currentIndex].worldPosition - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
                rb.MovePosition(rb.position + _dir * speed * Time.deltaTime);
            }
            if(d < 0.4f)
                currentIndex++;
        }

        else currentIndex = 0;
    }

    public bool CanSee(Transform target, float d, float a, LayerMask layer)
    {
        var _viewAngle = a;
        var _viewDistance = d;
        

        if (target == null) return false;

        var _dirToTarget = (target.position + new Vector3(0, 0.5f, 0) - transform.position + new Vector3(0, 0.5f, 0)).normalized;

        var _angleToTarget = Vector3.Angle(transform.forward, _dirToTarget);

        var _distanceToTarget = Vector3.Distance(transform.position, target.position);

        bool obstaclesBetween = false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), _dirToTarget, out hit, _distanceToTarget, layer))
        {
            if (hit.transform.name == target.name)
            {
                Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0), hit.point, Color.yellow);
            }
            else
            {
                Debug.DrawLine(transform.position + new Vector3(0, 0.5f, 0), hit.point, Color.red);
                obstaclesBetween = true;
            }


        }

        if (_angleToTarget <= _viewAngle && _distanceToTarget <= _viewDistance && !obstaclesBetween)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public CombatNode FindNearAggressiveNode()
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

            var NearNodes = Physics.OverlapSphere(myCombatNode.transform.position, distanceToOcupateNodes + 1).Where(y => y.GetComponent<CombatNode>()).Select(y => y.GetComponent<CombatNode>()).Where(y => y.myOwner == null);

            foreach (var item in NearNodes)
            {
                item.myOwner = this;
            }

            return node;
        }

        else return playerNodes[0];
    }

    public CombatNode FindNearNon_AggressiveNode()
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

            var NearNodes = Physics.OverlapSphere(myCombatNode.transform.position, distanceToOcupateNodes).Where(y => y.GetComponent<CombatNode>()).Select(y => y.GetComponent<CombatNode>()).Where(y => y.myOwner == null);

            foreach (var item in NearNodes)
            {
                item.myOwner = this;
            }

            return node;
        }
        else return playerNodes[0];
    }

    public void OnTriggerStay(Collider c)
    {
         if (c.GetComponent<CombatNode>()) c.GetComponent<CombatNode>().myOwner = this;
    }

    public void OnTriggerExit(Collider c)
    {
         if (c.GetComponent<CombatNode>()) c.GetComponent<CombatNode>().myOwner = null;
    }
}
