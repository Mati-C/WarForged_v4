using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class ClassEnemy : MonoBehaviour
{
    Viewer_E_Melee _viewer;
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

    public List<ClassEnemy> sameID_Enemies = new List<ClassEnemy>();

    [Header("Enemy Stats:")]

    public float life;
    public float maxLife;
    public float speed;
    public int aggressiveLevel;

    [Header("Enemy States:")]

    public bool isDead;
    public bool onDamage;
    public bool canPersuit;
    public bool canSurround;

    public int currentIndex;

    private void Awake()
    {
        player = FindObjectOfType<Model_Player>();
        pathfinding = GetComponent<Pathfinding>();
        sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != this));
        _viewer = GetComponent<Viewer_E_Melee>();
        rb = GetComponent<Rigidbody>();
    }


    void RemoveSameID_Enemy(ClassEnemy e)
    {
        sameID_Enemies.Remove(e);
    }

    public void ChangeCanGetDamage(bool b) { onDamage = b; }

    public void GetDamage(float d)
    {
        if (onDamage)
        {
            life -= d;
            onDamage = false;
            _viewer.CreatePopText(d);
        }
    }

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
        currentIndex = 0;
        FindPath(target.position);

        pathToTarget.Clear();
        pathToTarget.AddRange(pathfinding.myPath);

        if (currentIndex < pathToTarget.Count)
        {
            
            float d = Vector3.Distance(pathToTarget[currentIndex].worldPosition, transform.position);

            var _dir = Vector3.zero;

            if (d >= 1)
            {
                Debug.Log("asda");
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
}
