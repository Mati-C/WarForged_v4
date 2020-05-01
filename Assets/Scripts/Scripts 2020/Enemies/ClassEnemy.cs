using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class ClassEnemy : MonoBehaviour
{
    Viewer_E_Melee _viewer;

    [Header("EnemyRoom ID:")]

    public float ID;

    public List<ClassEnemy> sameID_Enemies = new List<ClassEnemy>();

    [Header("Enemy Stats:")]

    public float life;
    public float maxLife;

    [Header("Enemy States:")]

    public bool isDead;
    public bool onDamage;

    private void Awake()
    {
        sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != this));
        _viewer = GetComponent<Viewer_E_Melee>();
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
}
