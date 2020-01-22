using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public class ModelB_Cyclops : EnemyEntity
{

    public enum EnemyInputs { PERSUIT, ATTACK1, ATTACK2, ATTACK3, DIE, STUNED, IDLE }

    [Header("Enemy Boss Variables:")]

    public bool AwakeAnimation;
    public bool fase1;
    public bool fase2;
    public bool fase3;
    public bool onAnimationAttack;

    public float actualDamage;
    public float damage1;
    public float damage2;
    public float damage3;

    ViewB_Cyclops _view;

    public bool onAttackArea;
    public float distanceToHit;
    public float angleToHit;

    public GameObject attackPivot;

    public Action Attack1Event;
    public Action AwakeEvent;
    public Action DieEvent;

    private EventFSM<EnemyInputs> _myFsm;

    private void Awake()
    {
        _view = GetComponent<ViewB_Cyclops>();
        rb = GetComponent<Rigidbody>();

        Attack1Event += _view.Attack1OnPlaceAnimation;
        MoveEvent += _view.MoveAnimation;
        IdleEvent += _view.IdleAnimation;
        AwakeEvent += _view.AwakeAnim;
        DieEvent += _view.DieAnim;

        var idle = new FSM_State<EnemyInputs>("IDLE");
        var persuit = new FSM_State<EnemyInputs>("PERSUIT");
        var attack1 = new FSM_State<EnemyInputs>("ATTACK1");
        var attack2 = new FSM_State<EnemyInputs>("ATTACK2");
        var attack3 = new FSM_State<EnemyInputs>("ATTACK3");
        var die = new FSM_State<EnemyInputs>("DIE");


         StateConfigurer.Create(idle)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .Done();

        StateConfigurer.Create(persuit)
        .SetTransition(EnemyInputs.ATTACK1, attack1)
        .SetTransition(EnemyInputs.ATTACK2, attack2)
        .SetTransition(EnemyInputs.ATTACK3, attack3)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(attack1)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(attack2)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(attack3)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        idle.OnUpdate += () =>
        {
            IdleEvent();

            if (isPersuit) AwakeEvent();

            if (isPersuit && AwakeAnimation) SendInputToFSM(EnemyInputs.PERSUIT);

        };

        persuit.OnUpdate += () =>
        {
            delayToAttack -= Time.deltaTime;

            MoveEvent();

            target.CombatState();
            Quaternion targetRotation;

            var dir = Vector3.zero;

            dir = (target.transform.position - transform.position).normalized;
     
            dir.y = 0;

            targetRotation = Quaternion.LookRotation(dir , Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);

            if (fase1 && onAttackArea) SendInputToFSM(EnemyInputs.ATTACK1);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

        };

        attack1.OnEnter += x =>
        {
            actualDamage = damage1;
        };

        attack1.OnUpdate += () =>
        {
            delayToAttack -= Time.deltaTime;

            if(delayToAttack <=0)
            {
                Attack1Event();
                onAnimationAttack = true;
                delayToAttack = maxDelayToAttack;
            }

            if (delayToAttack > 0 && !onAnimationAttack)
            {
                _view.anim.SetBool("Attack1OnPlace", false);
                IdleEvent();
            }

            target.CombatState();
            Quaternion targetRotation;

            var dir = Vector3.zero;

            dir = (target.transform.position - transform.position).normalized;

            dir.y = 0;

            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

            if (!onAttackArea && isPersuit && !onAnimationAttack) SendInputToFSM(EnemyInputs.PERSUIT);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);
        };

        die.OnEnter += x =>
        {
            isDead = true;
            SoundManager.instance.CombatMusic(false);
            DieEvent();
        };

        _myFsm = new EventFSM<EnemyInputs>(idle);
    }

    private void Update()
    {
        _myFsm.Update();

        if (target.isDead)
        {
            isPersuit = false;
            onAttackArea = false;
            SendInputToFSM(EnemyInputs.IDLE);
            _view.healthBar.gameObject.SetActive(false);
        }
      
        if (target != null)
        {

            isPersuit = SearchForTarget.SearchTarget(target.transform, viewDistancePersuit, angleToPersuit, transform, true, layerObst);

            onAttackArea = SearchForTarget.SearchTarget(target.transform, distanceToHit, angleToHit, transform, true, layerObst);
        }
    }

    public void MakeDamageBoss()
    {
        var t = Physics.OverlapSphere(attackPivot.transform.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).First();

        if (t)
        {
            t.GetDamage(actualDamage, transform, false, 2, this);
            if (!t.invulnerable)
                t.rb.AddForce(transform.forward * 5, ForceMode.Impulse);
        }
    }

    public void AwakeStateActivate()
    {
        AwakeAnimation = true;
    }

    public void AnimationAttackFalse()
    {
        onAnimationAttack = false;
    }

    public void SendInputToFSM(EnemyInputs inp)
    {
        _myFsm.SendInput(inp);
    }

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
        SoundManager.instance.Play(EntitySound.BODY_IMPACT_2, transform.position, true);
        life -= damage;
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

    public override void StartPursuit()
    {
        throw new System.NotImplementedException();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistancePersuit);

        Vector3 rightLimit = Quaternion.AngleAxis(angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistancePersuit));

        Vector3 leftLimit = Quaternion.AngleAxis(-angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistancePersuit));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToHit);

        Vector3 rightLimit3 = Quaternion.AngleAxis(angleToHit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit3 * distanceToHit));

        Vector3 leftLimit3 = Quaternion.AngleAxis(-angleToHit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit3 * distanceToHit));

        Gizmos.DrawWireSphere(attackPivot.transform.position, radiusAttack);
    }
}
