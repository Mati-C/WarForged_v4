using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class ClassEnemy : MonoBehaviour
{
    [Header("EnemyRoom ID:")]

    public float ID;

    public List<ClassEnemy> sameID_Enemies = new List<ClassEnemy>();

    [Header("Enemy Stats:")]

    public float life;
    public float maxLife;

    [Header("Enemy States:")]

    public bool isDead;

    private void Awake()
    {
        sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != this));
    }

    private void Update()
    {
      
    }

    void RemoveSameID_Enemy(ClassEnemy e)
    {
        sameID_Enemies.Remove(e);
    }
}
