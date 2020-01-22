using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EnemyMeleeClass : EnemyEntity
{

    [Header("ENEMY COMBAT:")]

    
    public bool reposition;
    public bool changeRotateWarrior;
    public bool checkTurn;
    public bool timeToAttack;
    public bool cooldwonReposition;
    public bool strafeAnim;
    public bool onAttackArea;
    public float timeMinAttack;
    public float timeMaxAttack;    
    public float timeToChangeRotation;
    public float distanceToBack;
    public float speedRotation;
    public float timeReposition;
    public float timeReposition2;
    public float timeKnocked;
    public float timeToRetreat;
    public float lookToTargetSpeed;
    public int flankDir;
    public EnemyCombatManager cm;

    public Action CombatIdleEvent;
    public Action WalkRightEvent;
    public Action WalkLeftEvent;

  

    public abstract IEnumerator AvoidWarriorRight();

    public abstract IEnumerator AvoidWarriorLeft();

    public abstract IEnumerator ChangeDirRotation();

    public List<EnemyMeleeClass> EnemyMeleeFriends = new List<EnemyMeleeClass>();

    public override void ChangeChatAnimation()
    {
        throw new System.NotImplementedException();
    }

    public override Vector3 EntitiesAvoidance()
    {
        throw new System.NotImplementedException();
    }

    public override CombatNode FindNearAggressiveNode()
    {
        throw new System.NotImplementedException();
    }

    public override CombatNode FindNearNon_AggressiveNode()
    {
        throw new System.NotImplementedException();
    }

    public override void GetDamage(float damage, DamageType typeOfDamage, int damageAnimationIndex)
    {
        throw new System.NotImplementedException();
    }

    public override Node GetMyNode()
    {
        throw new System.NotImplementedException();
    }

    public override Node GetMyTargetNode()
    {
        throw new System.NotImplementedException();
    }

    public override Node GetRandomNode()
    {
        throw new System.NotImplementedException();
    }

    public override void MakeDamage()
    {
        throw new System.NotImplementedException();
    }

    public override Vector3 ObstacleAvoidance()
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveNearEntity(EnemyEntity e)
    {
        throw new System.NotImplementedException();
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }

    public override void SetChatAnimation()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
