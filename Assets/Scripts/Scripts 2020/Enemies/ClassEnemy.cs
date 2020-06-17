using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public abstract class ClassEnemy : MonoBehaviour
{
    ClassEnemyViewer _viewer;
    public N_FSM_EventMachine myFSM_EventMachine;
    public List<Node2> nodes = new List<Node2>();
    public List<Node2> pathToTarget = new List<Node2>();
    public Pathfinding pathfinding;
    public Model_Player player;
    public Grid grid;
    public Rigidbody rb;
   

    [Header("EnemyRoom ID:")]

    public float ID;
    public LayerMask layersCanSee;
    public LayerMask layersObstacles;

    public List<ClassEnemy> sameID_Enemies = new List<ClassEnemy>();

    [Header("Enemy CombatNodes:")]

    public CombatNode myCombatNode;
    public CombatNode lastCombatNode;
    public List<CombatNode> playerNodes = new List<CombatNode>();
    public float distanceToOcupateNodes;

    [Header("Enemy Stats:")]

    public float life;
    public float maxLife;
    public float speed;
    public int aggressiveLevel;

    [Header("Enemy States:")]

    public bool isDead;
    public bool canPersuit;
    public bool canSurround;
    public bool OnDamage;
    public bool blockedAttack;
    public int currentIndex;

    [Header("Enemy GetHit Variables:")]
    public float onDamageTime;
    public float damageDelayTime;

    public Action GetHitEvent;
    public Action DieEvent;
    public Action BlockedEvent;
    public Action KnockedEvent;

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

    private void Awake()
    {
        player = FindObjectOfType<Model_Player>();
        pathfinding = GetComponent<Pathfinding>();
        playerNodes.AddRange(FindObjectsOfType<CombatNode>());
        sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != this));
        _viewer = GetComponent<ClassEnemyViewer>();
        rb = GetComponent<Rigidbody>();
    }


    void RemoveSameID_Enemy(ClassEnemy e)
    {
        sameID_Enemies.Remove(e);
    }

    public void BlockedAttack()
    {
        BlockedEvent();
        StartCoroutine(BlockedState(0.7f));
    }

    public void Knocked()
    {
        KnockedEvent();
        StartCoroutine(KnockedMovement());
        StartCoroutine(BlockedState(4.3f));
    }

    public void OnFire()
    {

    }

    IEnumerator KnockedMovement()
    {
        yield return new WaitForSeconds(0.6f);
        var t = 0.4f;
        while(t >0)
        {
            t -= Time.deltaTime;
            rb.MovePosition(transform.position + player.transform.forward * 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator BlockedState(float t)
    {
        blockedAttack = true;
        yield return new WaitForSeconds(t);
        blockedAttack = false;
    }

    public abstract void GetDamage(float d);

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

    public void MoveToTarget(Transform target)
    {
        FindPath(target.position);

        pathToTarget.Clear();
        pathToTarget.AddRange(pathfinding.myPath);

        if (currentIndex < pathToTarget.Count)
        {

            float d = Vector3.Distance(pathToTarget[currentIndex].worldPosition, transform.position);

            var _dir = Vector3.zero;

            if (d > 0.5f)
            {
                Quaternion targetRotation;
                _dir = (pathToTarget[currentIndex].worldPosition - transform.position).normalized;
                _dir.y = 0;
                targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
                rb.MovePosition(rb.position + _dir * speed * Time.deltaTime);

            }
            else
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
