using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public class Model_B_Ogre : EnemyEntity
{
    public enum EnemyInputs { PERSUIT, ATTACK, SUMMON1, SUMMON2, SUMMON3, DIE, IDLE }

    [Header("Enemy Boss Variables:")]

    public bool AwakeAnimation;
    public bool fase1;
    public bool fase2;
    public bool fase3;
    public bool onAnimationAttack;
    public bool changeAttack;

    public List<EnemyEntity> enemiesSummon1 = new List<EnemyEntity>();
    public List<EnemyEntity> enemiesSummon2 = new List<EnemyEntity>();
    public List<EnemyEntity> enemiesSummon3 = new List<EnemyEntity>();

    public float actualDamage;
    public float damage1;
    public float damage2;

    bool _restart;
    Vector3 _startPos;

    public bool onAttackArea;
    public float distanceToHit;
    public float angleToHit;
    bool _delaySummon;

    public GameObject attackPivot;
    Viewer_B_Ogre _view;

    public Action Attack1Event;
    public Action Attack2Event;
    public Action Attack3Event;
    public Action AwakeEvent;
    public Action DieEvent;

    private EventFSM<EnemyInputs> _myFsm;

    public int _ID_Behaviour;

    IEnumerator DelaySummon()
    {
        _delaySummon = true;
        yield return new WaitForSeconds(1.5f);
        _delaySummon = false;
    }

    IEnumerator DelayChangeAttackState(float t)
    {
        yield return new WaitForSeconds(t);
        changeAttack = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        _view = GetComponent<Viewer_B_Ogre>();

        Attack1Event += _view.Attack1Anim;
        Attack2Event += _view.Attack2Anim;
        MoveEvent += _view.MoveAnimation;
        IdleEvent += _view.IdleAnimation;
        AwakeEvent += _view.AwakeAnim;
        DieEvent += _view.DieAnim;

        var idle = new FSM_State<EnemyInputs>("IDLE");
        var persuit = new FSM_State<EnemyInputs>("PERSUIT");
        var attack = new FSM_State<EnemyInputs>("ATTACK");
        var summon1 = new FSM_State<EnemyInputs>("SUMMON1");
        var summon2 = new FSM_State<EnemyInputs>("SUMMON2");
        var summon3 = new FSM_State<EnemyInputs>("SUMMON3");
        var die = new FSM_State<EnemyInputs>("DIE");


        StateConfigurer.Create(idle)
       .SetTransition(EnemyInputs.PERSUIT, persuit)
       .Done();

        StateConfigurer.Create(persuit)
        .SetTransition(EnemyInputs.ATTACK, attack)
        .SetTransition(EnemyInputs.SUMMON1, summon1)
        .SetTransition(EnemyInputs.SUMMON2, summon2)
        .SetTransition(EnemyInputs.SUMMON3, summon3)
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(attack)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.SUMMON1, summon1)
        .SetTransition(EnemyInputs.SUMMON2, summon2)
        .SetTransition(EnemyInputs.SUMMON3, summon3)
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(summon1)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.SUMMON2, summon2)
        .SetTransition(EnemyInputs.SUMMON3, summon3)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(summon2)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.SUMMON1, summon1)
        .SetTransition(EnemyInputs.SUMMON3, summon3)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(summon3)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.SUMMON1, summon1)
        .SetTransition(EnemyInputs.SUMMON2, summon2)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        idle.OnUpdate += () =>
        {
            _restart = false;

            IdleEvent();

            if (target.life >= 0)
            {

                if (isPersuit) AwakeEvent();

                if (isPersuit && AwakeAnimation) SendInputToFSM(EnemyInputs.PERSUIT);

            }
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

            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);

            if (onAttackArea) SendInputToFSM(EnemyInputs.ATTACK);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

        };

        attack.OnUpdate += () =>
        {
            if (!changeAttack)
            {
                _ID_Behaviour = UnityEngine.Random.Range(0, 2);
                changeAttack = true;
            }

            if (_ID_Behaviour == 0)
            {
                delayToAttack -= Time.deltaTime;

                if (delayToAttack <= 0)
                {
                    Attack1Event();
                    actualDamage = damage1;
                    onAnimationAttack = true;
                    delayToAttack = maxDelayToAttack;
                    StartCoroutine(DelayChangeAttackState(1.5f));
                }

                if (delayToAttack > 0 && !onAnimationAttack)
                {
                    _view.anim.SetBool("Attack", false);
                    IdleEvent();
                }
            }

            if (_ID_Behaviour == 1)
            {
                delayToAttack -= Time.deltaTime;

                if (delayToAttack <= 0)
                {
                    Attack2Event();
                    actualDamage = damage2;
                    onAnimationAttack = true;
                    delayToAttack = maxDelayToAttack;
                    StartCoroutine(DelayChangeAttackState(1.5f));
                }

                if (delayToAttack > 0 && !onAnimationAttack)
                {
                    _view.anim.SetBool("Attack2", false);
                    IdleEvent();
                }
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

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

        };

        summon1.OnEnter += x =>
        {
            AwakeEvent();
            StartCoroutine(DelaySummon());
            foreach (var item in enemiesSummon1)
            {
                item.viewDistancePersuit = 25;
                item.angleToPersuit = 180;
            }
        };

        summon1.OnUpdate += () =>
        {
            if (onAttackArea) SendInputToFSM(EnemyInputs.ATTACK);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

            if(!onAttackArea && isPersuit && !onAnimationAttack) SendInputToFSM(EnemyInputs.PERSUIT);

        };

        summon2.OnEnter += x =>
        {
            AwakeEvent();
            StartCoroutine(DelaySummon());
            foreach (var item in enemiesSummon2)
            {
                item.viewDistancePersuit = 25;
                item.angleToPersuit = 180;
            }
        };

        summon2.OnUpdate += () =>
        {
            if (onAttackArea) SendInputToFSM(EnemyInputs.ATTACK);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

            if (!onAttackArea && isPersuit && !onAnimationAttack) SendInputToFSM(EnemyInputs.PERSUIT);

        };

        summon3.OnEnter += x =>
        {
            AwakeEvent();
            StartCoroutine(DelaySummon());
            foreach (var item in enemiesSummon3)
            {
                item.viewDistancePersuit = 25;
                item.angleToPersuit = 180;
            }
        };

        summon3.OnUpdate += () =>
        {
            if (onAttackArea) SendInputToFSM(EnemyInputs.ATTACK);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

            if (!onAttackArea && isPersuit && !onAnimationAttack) SendInputToFSM(EnemyInputs.PERSUIT);

        };
    }

    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        _myFsm.Update();

        if (target.isDead)
        {
            isPersuit = false;
            onAttackArea = false;
            SendInputToFSM(EnemyInputs.IDLE);
            _view.healthBar.gameObject.SetActive(false);
            viewDistancePersuit = 0;
            angleToPersuit = 0;
        }

        if (life <= 180 && life > 100) fase2 = true;

        if (life <= 100) fase3 = true;

        if (target != null)
        {

            isPersuit = SearchForTarget.SearchTarget(target.transform, viewDistancePersuit, angleToPersuit, transform, true, layerObst);

            onAttackArea = SearchForTarget.SearchTarget(target.transform, distanceToHit, angleToHit, transform, true, layerObst);
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

    public void MakeDamageBoss()
    {
        var t = Physics.OverlapSphere(attackPivot.transform.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>());

        if (t.Count() > 0)
        {
            t.First().GetDamage(actualDamage, transform, false, Model.DamagePlayerType.Heavy, this);
            if (!t.First().invulnerable)
                t.First().rb.AddForce(transform.forward * 5, ForceMode.Impulse);
        }
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

    public override void Respawn()
    {
        AwakeAnimation = false;
        life = totalLife;
     //   _view.auxAwake = false;
      //  _view.healthBar.gameObject.SetActive(false);
        fase2 = false;
        fase3 = false;
        transform.position = _startPos;
        _restart = true;
        viewDistancePersuit = 21;
        angleToPersuit = 180;
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
