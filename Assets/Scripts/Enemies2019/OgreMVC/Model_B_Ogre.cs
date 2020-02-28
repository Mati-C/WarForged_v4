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

    public GameObject bossBlocks;
    public bool AwakeAnimation;
    public bool fase1;
    public bool fase2;
    public bool fase3;
    public bool summoned1;
    public bool summoned2;
    public bool summoned3;
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

    Vector3 summonPos;
    bool enemies1Dead;
    bool enemies2Dead;
    bool enemies3Dead;
    bool activeHeavyParticlesAgain;

    IEnumerator StopHeavyParticles()
    {
        yield return new WaitForSeconds(0.5f);
        _view.HeavyParticlesActiveFalse();
    }

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

    IEnumerator MoveWhenAttck()
    {
        var pos1 = transform.position;
        float time1 = 0.3f;

        while (time1 > 0)
        {
            time1 -= Time.deltaTime;
            transform.position = Vector3.Lerp(pos1, transform.position + transform.forward * Time.deltaTime, 2);
            yield return new WaitForEndOfFrame();
        }

        var pos2 = transform.position;
        float time2 = 0.5f;

        while (time2 > 0)
        {
            time2 -= Time.deltaTime;
            transform.position = Vector3.Lerp(pos2, transform.position + transform.forward * Time.deltaTime, 2);
            yield return new WaitForEndOfFrame();
        }
        
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

            if (fase1 && life > 0) SendInputToFSM(EnemyInputs.SUMMON1);

            if (fase2 && life > 0) SendInputToFSM(EnemyInputs.SUMMON2);

            if (fase3 && life > 0) SendInputToFSM(EnemyInputs.SUMMON3);
        };

        attack.OnUpdate += () =>
        {
            if (!changeAttack)
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
                    StartCoroutine(MoveWhenAttck());
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

            if (fase1 && life>0) SendInputToFSM(EnemyInputs.SUMMON1);

            if (fase2 && life > 0) SendInputToFSM(EnemyInputs.SUMMON2);

            if (fase3 && life > 0) SendInputToFSM(EnemyInputs.SUMMON3);

        };

        summon1.OnEnter += x =>
        {
            bossStage = true;
            target.targetLockedOn = false;
            target._camController.StopLockedTarget();
            target.targetLocked = null;
            target.enemiesToLock.Clear();
            AwakeAnimation = true;
            AwakeEvent();
            _view.Attack1AnimFalse();
            _view.Attack2AnimFalse();
            SoundManager.instance.Play(Sound.EntitySound.ROAR, transform.position, true, 1);
            StartCoroutine(DelaySummon());
            nearEntities.AddRange(enemiesSummon1);
            target.enemiesToLock.AddRange(enemiesSummon1);
            foreach (var item in enemiesSummon1)
            {
                item.viewDistancePersuit = 10;
                item.angleToPersuit = 180;
                item.bossSummon = true;
            }
            summoned1 = true;
            fase1 = false;
            summonPos = transform.position;
        };

        summon1.OnUpdate += () =>
        {

            if (!enemiesSummon1.Any(x => x.life > 0)) enemies1Dead = true;

            target.CombatState();

            var d = Vector3.Distance(_startPos, transform.position);

            if (!_view.anim.GetBool("Awake") && d >0.5f)
            {
                MoveEvent();
                Quaternion targetRotation;

                var dir = Vector3.zero;

                dir = (_startPos - transform.position).normalized;

                dir.y = 0;

                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
                rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
            }

            if (!_view.anim.GetBool("Awake") && d < 0.5f)
            {
                bossBlocks.SetActive(true);

                IdleEvent();

                Quaternion targetRotation;

                var dir = Vector3.zero;

                dir = (target.transform.position - transform.position).normalized;

                dir.y = 0;

                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (onAttackArea && enemies1Dead) SendInputToFSM(EnemyInputs.ATTACK);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

            if(!onAttackArea && isPersuit && enemies1Dead) SendInputToFSM(EnemyInputs.PERSUIT);

        };

        summon1.OnExit += x =>
        {
            bossStage = false;
            bossBlocks.SetActive(false);
        };

        summon2.OnEnter += x =>
        {
            bossStage = true;

            target.targetLockedOn = false;
            target._camController.StopLockedTarget();
            target.targetLocked = null;
            target.enemiesToLock.Clear();
            AwakeAnimation = true;
            AwakeEvent();
            _view.Attack1AnimFalse();
            _view.Attack2AnimFalse();
            SoundManager.instance.Play(Sound.EntitySound.ROAR, transform.position, true, 1);
            StartCoroutine(DelaySummon());
            nearEntities.AddRange(enemiesSummon2);
            target.enemiesToLock.AddRange(enemiesSummon2);
            foreach (var item in enemiesSummon2)
            {
                item.viewDistancePersuit = 10;
                item.angleToPersuit = 180;
                item.bossSummon = true;
            }
            summoned2 = true;
            fase2 = false;
            summonPos = transform.position;
            bossBlocks.SetActive(true);
        };

        summon2.OnUpdate += () =>
        {
            if (!enemiesSummon2.Any(x => x.life > 0)) enemies2Dead = true;

            target.CombatState();

            var d = Vector3.Distance(_startPos, transform.position);

            if (!_view.anim.GetBool("Awake") && d > 0.5f)
            {
                MoveEvent();
                Quaternion targetRotation;

                var dir = Vector3.zero;

                dir = (_startPos - transform.position).normalized;

                dir.y = 0;

                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
                rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
            }

            if (!_view.anim.GetBool("Awake") && d < 0.5f)
            {
                IdleEvent();

                Quaternion targetRotation;

                var dir = Vector3.zero;

                dir = (target.transform.position - transform.position).normalized;

                dir.y = 0;

                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (onAttackArea && enemies2Dead) SendInputToFSM(EnemyInputs.ATTACK);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

            if (!onAttackArea && isPersuit && enemies2Dead) SendInputToFSM(EnemyInputs.PERSUIT);

        };

        summon2.OnExit += x =>
        {
            bossStage = false;
            bossBlocks.SetActive(false);
        };

        summon3.OnEnter += x =>
        {
            bossStage = true;
            target.targetLockedOn = false;
            target._camController.StopLockedTarget();
            target.targetLocked = null;
            target.enemiesToLock.Clear();
            AwakeAnimation = true;
            AwakeEvent();
            _view.Attack1AnimFalse();
            _view.Attack2AnimFalse();
            SoundManager.instance.Play(Sound.EntitySound.ROAR, transform.position, true, 1);
            StartCoroutine(DelaySummon());
            nearEntities.AddRange(enemiesSummon3);
            target.enemiesToLock.AddRange(enemiesSummon3);
            foreach (var item in enemiesSummon3)
            {
                item.viewDistancePersuit = 10;
                item.angleToPersuit = 180;
                item.bossSummon = true;
            }
            summoned3 = true;
            fase3 = false;
            summonPos = transform.position;
            bossBlocks.SetActive(true);
        };

        summon3.OnUpdate += () =>
        {
            if (!enemiesSummon3.Any(x => x.life > 0)) enemies3Dead = true;

            target.CombatState();

            var d = Vector3.Distance(_startPos, transform.position);

            if (!_view.anim.GetBool("Awake") && d > 0.5f)
            {
                MoveEvent();
                Quaternion targetRotation;

                var dir = Vector3.zero;

                dir = (_startPos - transform.position).normalized;

                dir.y = 0;

                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
                rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
            }

            if (!_view.anim.GetBool("Awake") && d < 0.5f)
            {
                IdleEvent();

                Quaternion targetRotation;

                var dir = Vector3.zero;

                dir = (target.transform.position - transform.position).normalized;

                dir.y = 0;

                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (onAttackArea && enemies3Dead) SendInputToFSM(EnemyInputs.ATTACK);

            if (life <= 0) SendInputToFSM(EnemyInputs.DIE);

            if (target.life <= 0 || _restart) SendInputToFSM(EnemyInputs.IDLE);

            if (!onAttackArea && isPersuit && enemies3Dead) SendInputToFSM(EnemyInputs.PERSUIT);

        };

        summon3.OnExit += x =>
        {
            bossStage = false;
            bossBlocks.SetActive(false);
        };

        die.OnEnter += x =>
        {
            isDead = true;
            SoundManager.instance.CombatMusic(false);
            DieEvent();
        };

        _myFsm = new EventFSM<EnemyInputs>(idle);
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

        if (target != null)
        {

            isPersuit = SearchForTarget.SearchTarget(target.transform, viewDistancePersuit, angleToPersuit, transform, true, layerObst);

            onAttackArea = SearchForTarget.SearchTarget(target.transform, distanceToHit, angleToHit, transform, true, layerObst);
        }

        if(life <= 150 && !summoned1) fase1 = true;

        if(life <= 100 && !summoned2) fase2 = true;

        if(life <= 50 && !summoned3) fase3 = true;

    }

    public void AwakeStateActivate()
    {
        AwakeAnimation = true;
    }

    public void AwakeStateDisable()
    {
        AwakeAnimation = false;
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
        if (!bossStage)
        {
            SoundManager.instance.Play(EntitySound.BODY_IMPACT_2, transform.position, true);
            _view.ActiveBloodParticles();
            _view.CreatePopText(damage);
            life -= damage;
        }
    }

    public override void Respawn()
    {
        AwakeAnimation = false;
        life = totalLife;
        _view.auxAwake = false;
        _view.healthBar.gameObject.SetActive(false);
        fase1 = false;
        fase2 = false;
        fase3 = false;
        transform.position = _startPos;
        _restart = true;
        viewDistancePersuit = 21;
        angleToPersuit = 180;
        summoned1 = false;
        summoned2 = false;
        summoned3 = false;
        foreach (var item in enemiesSummon1) item.life = item.totalLife;
        foreach (var item in enemiesSummon2) item.life = item.totalLife;
        foreach (var item in enemiesSummon3) item.life = item.totalLife;

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
