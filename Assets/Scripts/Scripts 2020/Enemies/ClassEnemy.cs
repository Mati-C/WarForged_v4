using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class ClassEnemy : MonoBehaviour
{
    Viewer_E_Melee _viewer;
    public List<Node2> nodes = new List<Node2>();
    public Pathfinding pathfinding;
    public Model_Player player;
    public Grid grid;

    [Header("EnemyRoom ID:")]

    public float ID;
    public float myPathCount;

    public List<ClassEnemy> sameID_Enemies = new List<ClassEnemy>();

    [Header("Enemy Stats:")]

    public float life;
    public float maxLife;

    [Header("Enemy States:")]

    public bool isDead;
    public bool onDamage;

    private void Awake()
    {
        player = FindObjectOfType<Model_Player>();
        pathfinding = GetComponent<Pathfinding>();
        sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != this));
        _viewer = GetComponent<Viewer_E_Melee>();
    }

    private void Start()
    {
        
        
    }

    private void Update()
    {
        
        
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
        pathfinding.StartCoroutine(pathfinding.FindPath(FindNearNode(transform.position), FindNearNode(endPos)));
    }
}
