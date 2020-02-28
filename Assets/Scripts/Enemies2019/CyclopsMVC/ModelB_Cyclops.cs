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
    public bool changeAttack;

    public float actualDamage;
    public float damage1;
    public float damage2;
    public float damage3;

    ViewB_Cyclops _view;
    bool _restart;
    bool activeHeavyParticlesAgain;
    Vector3 _startPos;

    public bool onAttackArea;
    public float distanceToHit;
    public float angleToHit;

    public GameObject attackPivot;

    public Action Attack1Event;
    public Action Attack2Event;
    public Action Attack3Event;
    public Action AwakeEvent;
    public Action DieEvent;

    private EventFSM<EnemyInputs> _myFsm;

    public int _ID_Behaviour;

    IEnumerator DelayChangeAttackState(float t)
    {        
        yield return new WaitForSeconds(t);
        activeHeavyParticlesAgain = false;
        changeAttack = false;
    }

    IEnumerator StopHeavyParticles()
    {
        yield return new WaitForSeconds(0.5f);
        _view.HeavyParticlesActiveFalse();
    }

    IEnumerator MoveWhenAttck()
    {
        var pos1 = transform.position;
        float time1 = 1;

        while (time1 > 0)
        {
            time1 -= Time.deltaTime;
            transform.position = Vector3.Lerp(pos1, transform.position + transform.forward  * Time.deltaTime, 2);
            yield return new WaitForEndOfFrame();
        }

        var pos2 = transform.position;
        float time2 = 1.5f;

        while(time2 >0)
        {
            time2 -= Time.deltaTime;
            transform.position = Vector3.Lerp(pos2, transform.position + transform.forward  * Time.deltaTime, 2);
            yield return new WaitForEndOfFrame();
        }
             
        var pos3 = transform.position;
        float time3 = 1;
        radiusAttack = 3;
        while (time3 > 0)
        {
            time3 -= Time.deltaTime;
            transform.position = Vector3.Lerp(pos3, transform.position + transform.forward  * Time.deltaTime, 2);
            yield return new WaitForEndOfFrame();
        }
        radiusAttack = 2;
    }

    private void Awake()
    {
        _view = GetComponent<ViewB_Cyclops>();
        rb = GetComponent<Rigidbody>();
        _startPos = transform.position;

        Attack1Event += _view.Attack1OnPlaceAnimation;
        Attack2Event += _view.Attack2OnPlaceAnimation;
        Attack3Event += _view.Attack3Combo;
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
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(attack1)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(attack2)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.IDLE, idle)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(attack3)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.IDLE, idle)
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

            targetRotation = Quaternion.LookRotation(dir , Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);

            if (fase1 && !fase2 && onAttackArea) SendInputToFSM(EnemyInputs.ATTACK1);

            if (!fase3 && fase2 && onAttackArea) SendInputToFSM(EnemyInputs.ATTACK2);

            if (fase3 && onAttackArea) SendInputToFSM(EnemyInputs.ATTACK3);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if(target.life<=0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

        };

        attack1.OnEnter += x =>
        {
            actualDamage = damage1;
        };

        attack1.OnUpdate += () =>
        {
            delayToAttack -= Time.deltaTime;

            if (delayToAttack <= 0.5f && !activeHeavyParticlesAgain)
            {
                _view.HeavyParticlesActive();
                activeHeavyParticlesAgain = true;
                StartCoroutine(StopHeavyParticles());
            }

            if (delayToAttack <=0)
            {
               // _view.HeavyParticlesActiveFalse();
                Attack1Event();
                onAnimationAttack = true;
                delayToAttack = maxDelayToAttack;
                StartCoroutine(DelayChangeAttackState(1.5f));
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

            if (fase2 && onAttackArea) SendInputToFSM(EnemyInputs.ATTACK2);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);
        };

        attack2.OnEnter += x =>
        {
            
        };

        attack2.OnUpdate += () =>
        {
            if(!changeAttack)
            {
                _ID_Behaviour = UnityEngine.Random.Range(0, 2);
                changeAttack = true;
            }

            if (delayToAttack <= 0.5f && !activeHeavyParticlesAgain)
            {
                _view.HeavyParticlesActive();
                activeHeavyParticlesAgain = true;
                StartCoroutine(StopHeavyParticles());
            }

            if (_ID_Behaviour == 0)
            {
              
                if (delayToAttack <= 0)
                {
                    radiusAttack = 2;
                    // _view.HeavyParticlesActiveFalse();
                    Attack1Event();
                    actualDamage = damage1;
                    onAnimationAttack = true;
                    delayToAttack = maxDelayToAttack;
                    StartCoroutine(DelayChangeAttackState(1.5f));
                }

                if (delayToAttack > 0 && !onAnimationAttack)
                {
                    _view.anim.SetBool("Attack1OnPlace", false);
                    IdleEvent();
                }
            }

            if (_ID_Behaviour == 1)
            {
                delayToAttack -= Time.deltaTime;

                if (delayToAttack <= 0)
                {
                    radiusAttack = 2;
                    // _view.HeavyParticlesActiveFalse();
                    Attack2Event();
                    actualDamage = damage2;
                    onAnimationAttack = true;
                    delayToAttack = maxDelayToAttack;
                    StartCoroutine(DelayChangeAttackState(1.5f));
                }

                if (delayToAttack > 0 && !onAnimationAttack)
                {
                    _view.anim.SetBool("Attack1OnPlace", false);
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

            if (fase3 && onAttackArea) SendInputToFSM(EnemyInputs.ATTACK3);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);
        };

        attack3.OnUpdate += () =>
        {

            if (!changeAttack)
            {
                _ID_Behaviour = UnityEngine.Random.Range(0, 3);
                changeAttack = true;
            }

            if (delayToAttack <= 0.25f && !activeHeavyParticlesAgain)
            {
                _view.HeavyParticlesActive();
                activeHeavyParticlesAgain = true;
                StartCoroutine(StopHeavyParticles());
            }

            if (_ID_Behaviour == 0)
            {
                delayToAttack -= Time.deltaTime;

                if (delayToAttack <= 0)
                {
                    radiusAttack = 2;
                    // _view.HeavyParticlesActiveFalse();
                    Attack1Event();
                    actualDamage = damage1;
                    onAnimationAttack = true;
                    delayToAttack = maxDelayToAttack;
                    StartCoroutine(DelayChangeAttackState(1.5f));
                }

                if (delayToAttack > 0 && !onAnimationAttack)
                {
                    _view.anim.SetBool("Attack1OnPlace", false);
                    IdleEvent();
                }
            }

            if (_ID_Behaviour == 1)
            {
                delayToAttack -= Time.deltaTime;

                if (delayToAttack <= 0)
                {
                    radiusAttack = 3;
                    //  _view.HeavyParticlesActiveFalse();
                    Attack2Event();
                    actualDamage = damage2;
                    onAnimationAttack = true;
                    delayToAttack = maxDelayToAttack;
                    StartCoroutine(DelayChangeAttackState(1.5f));
                }

                if (delayToAttack > 0 && !onAnimationAttack)
                {
                    _view.anim.SetBool("Attack1OnPlace", false);
                    IdleEvent();
                }
            }

            if (_ID_Behaviour == 2)
            {
                delayToAttack -= Time.deltaTime;

                if (delayToAttack <= 0)
                {
                  //  _view.HeavyParticlesActiveFalse();
                    Attack3Event();
                    actualDamage = damage3;
                    onAnimationAttack = true;
                    delayToAttack = maxDelayToAttack;
                    StartCoroutine(DelayChangeAttackState(4f));
                    StartCoroutine(MoveWhenAttck());

                }

                if (delayToAttack > 0 && !onAnimationAttack)
                {
                    _view.anim.SetBool("Attack1OnPlace", false);
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

    public void MakeDamageBoss()
    {
        var t = Physics.OverlapSphere(attackPivot.transform.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>());

        if (t.Count() >0)
        {
            t.First().GetDamage(actualDamage, transform, false, Model.DamagePlayerType.Heavy, this);
            if (!t.First().invulnerable)
                t.First().rb.AddForce(transform.forward * 5, ForceMode.Impulse);
        }
    }

    public void MakeDamageBossUnstopable()
    {
        var t = Physics.OverlapSphere(attackPivot.transform.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>());

        if (t.Count() > 0)
        {
            t.First().GetDamage(actualDamage, transform, false, Model.DamagePlayerType.Unstopable, this);
            t.First().rb.AddForce(transform.forward * 5, ForceMode.Impulse);
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
        _view.CreatePopText(damage);
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
        AwakeAnimation = false;
        life = totalLife;
        _view.auxAwake = false;
        _view.healthBar.gameObject.SetActive(false);
        fase2 = false;
        fase3 = false;
        transform.position = _startPos;
        _restart = true;
        viewDistancePersuit = 21;
        angleToPersuit = 180;
    }

    public override void SetChatAnimation()
    {
       // throw new System.NotImplementedException();
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
