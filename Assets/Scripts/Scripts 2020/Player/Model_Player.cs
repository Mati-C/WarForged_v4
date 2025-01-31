﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Sound;

public class Model_Player : MonoBehaviour
{
    Controller_Player _controller;
    Viewer_Player _viewer;
    public Rigidbody rb;
    public PlayerCamera _playerCamera;
    Camera _mainCam;
    IA_CombatManager _IA_CM;
    FadeLevel _levelFade;

    [HideInInspector]
    public FireSword fireSword;

    [Header("Player Life:")]
    public float life;
    public float maxLife;
    public float distanceAggressiveNodes;
    public float distanceNon_AggressiveNodes;
    public Vector3 revivePos;
    public Vector3 reviveForward;

    [Header("Player Speeds:")]
    public float speed;
    public float runSpeed;
    public float dodgeSpeedRoll;
    public float dodgeSpeedBack;
    public float dodgeSpeedLeft;
    public float dodgeSpeedRight;

    [Header("Player States:")]
    public bool run;
    public bool isInCombat;
    public bool onDodge;
    public bool cantAttack;
    public bool onAttackAnimation;
    public bool onDefence;
    public bool onAction;
    public bool OnDamage;
    public bool onCinematic;
    public bool isDead;

    [Header("Player Layers:")]

    public LayerMask playerCanSee;

    [Header("Player CombatValues:")]

    public int attackCombo;
    public float resetAttackTimer;
    public float timeOnCombat;
    public float maxTimeOnCombat;
    public float chargeAttackAmount;
    public float chargeAttackTime;
    public float chargeAttackColdown;
    public float chargeAttackColdownMax;
    public float viewDistanceAttack;
    public float angleToAttack;
    public float defenceTimer;
    public float onDamageTime;
    public bool chargeAttackCasted;
    bool _chargeAttackSound;

    [Header("Player Powers Values:")]
    public bool  flamesOn;
    public float fireEnergy;
    public float fireSwordCurrentTime;

    [Header("Player Damage Values:")]
    public float AttackDamageCombo1;
    public float AttackDamageCombo2;
    public float AttackDamageCombo3;
    public float ChargeAttackDamage;

    public enum DogeDirecctions { Left, Right, Back, Roll };
    public DogeDirecctions dirToDodge;

    public enum DamageType { Light,Heavy };
    public DamageType damageType;

    [Header("Player AttackMovementImpulses:")]
    public float impulseAttackMovement1;
    public float impulseAttackMovement2;
    public float impulseAttackMovement3;
    public float impulseAttackMovement4;
    public float impulseChargeAttackMovement;

    [Header("Player Fail Attack Values:")]
    public bool onFailAttack;
    public float timeOnFailAttack;

    [Header("Player LockEnemies:")]

    public bool onLock;
    public List<ClassEnemy> lockedEnemies = new List<ClassEnemy>();
    public ClassEnemy targetEnemy;
    int _indexEnemyLock;


    float _onAttackAnimationTimer;
    float _timeToWaitBeforeAttack;
    float _movementAttackTime;
    Vector3 _attackLastPos;
    
    public Action idleEvent;
    public Action WalkEvent;
    public Action RunEvent;
    public Action TakeSwordEvent;
    public Action SaveSwordEvent;
    public Action LockedOnEvent;
    public Action LockedOffEvent;
    public Action FireSwordEvent;
    public Action PowerDesactivatedEvent;
    public Action <float>HitEnemyEvent;
    public Action ChargeAttackEvent;
    public Action<bool> DefenceEvent;
    public Action<bool> BlockEvent;
    public Action GetHitEvent;
    public Action<bool> GetHitHeavyEvent;
    public Action<string> FailAttackEvent;
    public Action MakeDamageTutorialEvent;
    public Action DefendTutorialEvent;
    public Action KickTutorialEvent;
    public Action DodgeTutorialEvent;
    public Action DieEvent;

    public Action<bool> CombatStateEvent;
    public Action<DogeDirecctions> DodgeEvent;

    public PlayerCamera GetPlayerCam() { return _playerCamera; }
    public IA_CombatManager GetIA_CombatManager() { return _IA_CM; }

    public IEnumerator OnDamageTimer()
    {
        while (true)
        {
            if (onDamageTime > 0)
            {
                onDamageTime -= Time.deltaTime;
                OnDamage = true;
            }

            if (onDamageTime <= 0) OnDamage = false;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FailAttackTimer()
    {
        while(true)
        {
            if (timeOnFailAttack > 0)
            {
                timeOnFailAttack -= Time.deltaTime;
                onFailAttack = true;
            }

            else onFailAttack = false;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator AttackRotation(Vector3 dir)
    {

        while (resetAttackTimer > 0)
        {
            Quaternion targetRotation;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DefenceMove()
    {
        while(onDefence)
        {
            var dir = _mainCam.transform.forward;
            dir.y = 0;
            Quaternion targetRotation;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
  
    IEnumerator LockOnMovement()
    {
        while(onLock)
        {
            if (targetEnemy.isDead)
            {
                lockedEnemies.Remove(targetEnemy);
                if (lockedEnemies.Count > 0) ChangeTarget();

                if (lockedEnemies.Count <= 0) LockEnemies();
            }

            var dir = (targetEnemy.transform.position - transform.position).normalized;
            dir.y = 0;
            Quaternion targetRotation;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator AttackSword()
    {
      
        while (true)
        {
            while (resetAttackTimer > 0)
            {
                resetAttackTimer -= Time.deltaTime;

                if (resetAttackTimer < 0.2f ) cantAttack = false;
                if (resetAttackTimer > 0.3f) cantAttack = true;

                yield return new WaitForEndOfFrame();
            }

            attackCombo = 0;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator AttackMovement()
    {
        while (true)
        {
            while (_movementAttackTime > 0)
            {
                _timeToWaitBeforeAttack -= Time.deltaTime;

                if (_timeToWaitBeforeAttack <= 0)
                {
                    _movementAttackTime -= Time.deltaTime;

                    switch (attackCombo)
                    {
                        case -1:
                            transform.position = Vector3.Lerp(_attackLastPos, transform.position + transform.forward * impulseChargeAttackMovement * Time.deltaTime, 1);
                            break;

                        case 1:
                            transform.position = Vector3.Lerp(_attackLastPos, transform.position + transform.forward * impulseAttackMovement1 * Time.deltaTime, 1);
                            break;

                        case 2:
                            transform.position = Vector3.Lerp(_attackLastPos, transform.position + transform.forward * impulseAttackMovement2 * Time.deltaTime, 1);
                            break;

                        case 3:
                            transform.position = Vector3.Lerp(_attackLastPos, transform.position + transform.forward * impulseAttackMovement3 * Time.deltaTime, 1);
                            break;

                        case 4:
                            transform.position = Vector3.Lerp(_attackLastPos, transform.position + transform.forward * impulseAttackMovement4 * Time.deltaTime, 1);
                            break;
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator AttackAnimationTimer()
    {
        while (true)
        {

            while (_onAttackAnimationTimer > 0)
            {
                _onAttackAnimationTimer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            onAttackAnimation = false;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator SetTimerCombat()
    {
        isInCombat = true;
        
        CombatStateEvent(true);
      
        TakeSwordEvent();

        while (timeOnCombat >0)
        {
            timeOnCombat -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        isInCombat = false;
        SaveSwordEvent();
        CombatStateEvent(false);
        DefenceOff();

    }

    IEnumerator DodgeMovement(float timer, Vector3 dir, float dodgeSpeed, bool delay)
    {
        onDodge = true;

        if(delay) yield return new WaitForSeconds(0.2f);

        while (timer >0)
        {
            timer -= Time.deltaTime;
            rb.MovePosition(transform.position + dir * dodgeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.1f);
        onDodge = false;
    }

    IEnumerator OnActionState(float time)
    {
        onAction = true;
        yield return new WaitForSeconds(time);
        onAction = false;
    }

    private void Awake()
    {
        _viewer = GetComponent<Viewer_Player>();
        _playerCamera = FindObjectOfType<PlayerCamera>();
        _mainCam = _playerCamera.GetComponent<Camera>();
        _IA_CM = FindObjectOfType<IA_CombatManager>();
        _levelFade = FindObjectOfType<FadeLevel>();
        _controller = new Controller_Player(this, _viewer, _levelFade);
        rb = GetComponent<Rigidbody>();
        fireSword = GetComponent<FireSword>();
        revivePos = transform.position;
        reviveForward = transform.forward;

        MakeDamageTutorialEvent += () => { };
        DefendTutorialEvent += () => { };
        KickTutorialEvent += () => { };
        DodgeTutorialEvent += () => { };
        ModifyNodes();
    }

    void Start()
    {
        StartCoroutine(AttackMovement());
        StartCoroutine(AttackSword());
        StartCoroutine(AttackAnimationTimer());
        StartCoroutine(FailAttackTimer());
        StartCoroutine(OnDamageTimer());
    }

    
    void Update()
    {
        _controller.ControllerUpdate();       
    }

    public void Movement(Vector3 d)
    {
        if (!onDodge && !onAttackAnimation)
        {
            Quaternion targetRotation;

            d.y = 0;

            targetRotation = Quaternion.LookRotation(d, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

            if (!run)
            {
                WalkEvent();
                rb.MovePosition(transform.position + d * speed * Time.deltaTime);
            }

            else
            {
                RunEvent();
                rb.MovePosition(transform.position + d * runSpeed * Time.deltaTime);
            }
        }
    }

    public void FailAttack(string objName)
    {
        cantAttack = false;
        onAttackAnimation = false;       
        timeOnFailAttack = 1f;
        chargeAttackAmount = 0;
        FailAttackEvent(objName);
        attackCombo = 0;
    }

    public void CombatMovement(Vector3 d, bool turnDir, bool opposite)
    {

        if(!onDodge && !onAttackAnimation && onLock  && !onAction && !OnDamage && !onFailAttack)
        {
            WalkEvent();
            rb.MovePosition(rb.position + d  * speed * Time.deltaTime);
        }

        if (!onDodge && !onAttackAnimation && !onLock && !onAction && !OnDamage && !onFailAttack)
        {
            Quaternion targetRotation;

            d.y = 0;

            if ((turnDir && !opposite) || run)
            {

                targetRotation = Quaternion.LookRotation(d, Vector3.up);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (!turnDir && !opposite && !run)
            {

                var dir = _playerCamera.transform.forward;

                dir.y = 0;

                targetRotation = Quaternion.LookRotation(dir, Vector3.up);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (opposite && !run)
            {

                targetRotation = Quaternion.LookRotation(-d, Vector3.up);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (!run)
            {
                WalkEvent();
                rb.MovePosition(transform.position + d * speed * Time.deltaTime);
            }

            else
            {

                if (!onDefence)
                {
                    RunEvent();
                    rb.MovePosition(transform.position + d * runSpeed * Time.deltaTime);
                }

                else
                {
                    WalkEvent();
                    rb.MovePosition(transform.position + d * speed * Time.deltaTime);
                }
            }
        }
    }

    public void Dodge(DogeDirecctions direction)
    {
        DodgeTutorialEvent();
        attackCombo = 0;
        if (direction == DogeDirecctions.Roll && !OnDamage)
        {
            DodgeEvent(DogeDirecctions.Roll);
            StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll, false));
        }

        if (direction == DogeDirecctions.Back && !OnDamage)
        {
            if (!isInCombat)
            {
                DodgeEvent(DogeDirecctions.Roll);
                if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll, false));
            }

            else if (!onDodge)
            {
                if (!run)
                {                   
                    DodgeEvent(DogeDirecctions.Back);
                    StartCoroutine(DodgeMovement(0.4f, -transform.forward, dodgeSpeedBack, true));
                }

                else
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedBack, false));
                }

            }
        }

        if (direction == DogeDirecctions.Right && !OnDamage)
        {
            if (!isInCombat)
            {
                DodgeEvent(DogeDirecctions.Roll);
                if (!onDodge) StartCoroutine(DodgeMovement(0.7f, _playerCamera.transform.right, dodgeSpeedRoll, false));
            }

            else if (!onDodge)
            {
                if (!run)
                {
                    DodgeEvent(DogeDirecctions.Right);
                    StartCoroutine(DodgeMovement(0.5f, _playerCamera.transform.right, dodgeSpeedRight, false));
                }

                else
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    StartCoroutine(DodgeMovement(0.7f, _playerCamera.transform.right, dodgeSpeedRight, false));
                }
            }
        }

        if (direction == DogeDirecctions.Left && !OnDamage)
        {
            if (!isInCombat)
            {
                DodgeEvent(DogeDirecctions.Roll);
                if (!onDodge) StartCoroutine(DodgeMovement(0.7f, -_playerCamera.transform.right, dodgeSpeedRoll, false));
            }

            else if (!onDodge)
            {
                if (!run)
                {
                    DodgeEvent(DogeDirecctions.Left);
                    StartCoroutine(DodgeMovement(0.5f, -_playerCamera.transform.right, dodgeSpeedLeft, false));
                }

                else
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    StartCoroutine(DodgeMovement(0.7f, -_playerCamera.transform.right, dodgeSpeedLeft, false));
                }
            }
        }


    }

    public void SwordAttack(Vector3 dir)
    {
        dir.y = 0;

        if (isInCombat && !onAction && !OnDamage && !cantAttack && !onDefence && !onFailAttack && !_viewer.anim.GetBool("FailAttack"))
        {

            if (attackCombo == 2 && !_viewer.anim.GetBool("FailAttack"))
            {
                resetAttackTimer = 0.8f;
                StartCoroutine(AttackRotation(dir));
                _onAttackAnimationTimer = 1f;
                _timeToWaitBeforeAttack = 0.1f;
                _movementAttackTime = 0.25f;
                StartCoroutine(MakeAttackDamageDelay2(AttackDamageCombo3));
                cantAttack = true;
                onAttackAnimation = true;
                if (!onFailAttack && !_viewer.anim.GetBool("FailAttack")) attackCombo++;
            }

            if (attackCombo == 1 && !_viewer.anim.GetBool("FailAttack"))
            {
                resetAttackTimer = 0.7f;
                StartCoroutine(AttackRotation(dir));
                _onAttackAnimationTimer = 1f;
                _timeToWaitBeforeAttack = 0.1f;
                _movementAttackTime = 0.35f;
                StartCoroutine(MakeAttackDamageDelay1(AttackDamageCombo2));
                cantAttack = true;
                onAttackAnimation = true;
                if (!onFailAttack && !_viewer.anim.GetBool("FailAttack")) attackCombo++;
            }

            if (attackCombo == 0 && !_viewer.anim.GetBool("FailAttack"))
            {
                resetAttackTimer = 0.4f;
                StartCoroutine(AttackRotation(dir));
                _onAttackAnimationTimer = 0.65f;
                _timeToWaitBeforeAttack = 0.2f;
                _movementAttackTime = 0.35f;
                StartCoroutine(MakeAttackDamageDelay3(AttackDamageCombo1));
                cantAttack = true;
                onAttackAnimation = true;
                if(!onFailAttack && !_viewer.anim.GetBool("FailAttack")) attackCombo++;
            }

            SoundManager.instance.PlayRandom(SoundManager.instance.swing, transform.position, true);
        }

        if(!_viewer.layerUpActive) CombatStateUp(false);
       
    }
     
    IEnumerator MakeAttackDamageDelay1(float d)
    {       
        yield return new WaitForSeconds(0.35f);
        MakeDamage(d);
    }

    IEnumerator MakeAttackDamageDelay2(float d)
    {
       
        yield return new WaitForSeconds(0.3f);
        MakeDamage(d);
    }

    IEnumerator MakeAttackDamageDelay3(float d)
    {

        yield return new WaitForSeconds(0.2f);
        MakeDamage(d);
    }

    public void MakeDamage(float d)
    {
        var enemies = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>()).Where(x=> x.life >0);
        var destructibles = Physics.OverlapSphere(transform.position, viewDistanceAttack / 2).Where(x => x.GetComponent<DestructibleOBJ>()).Select(x => x.GetComponent<DestructibleOBJ>());
        var roots = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<Roots>()).Select(x => x.GetComponent<Roots>());
        var rune = Physics.OverlapSphere(transform.position, viewDistanceAttack / 2).Where(x => x.GetComponent<Portal_Rune>()).Select(x => x.GetComponent<Portal_Rune>());

        if (enemies.Count() == 0 && destructibles.Count() == 0)
        {
            var obstacles = Physics.OverlapSphere(transform.position, viewDistanceAttack / 2).Where(x => x.gameObject.layer == 16 && !x.GetComponent<DestructibleOBJ>());
            if (obstacles.Count() > 0 || rune.Count() > 0)
            {
                FailAttack(obstacles.FirstOrDefault().gameObject.name);
                if (rune.FirstOrDefault() != null)
                    rune.First().Damage();
            }
        }

        if (roots.Count() > 0 )
        {
            if (flamesOn)

                foreach (var item in roots)
                {
                    if(!item.tutoRoot) item.StartDissolve();

                    else FailAttack("Crate");
                }

            else FailAttack("Crate");
        }

        foreach (var item in enemies)
        {
            if (CanSee(item.transform, viewDistanceAttack, angleToAttack, playerCanSee))
            {
                MakeDamageTutorialEvent();
                item.GetDamage(d,DamageType.Light);

                if (fireSwordCurrentTime <= 0)
                {
                    fireEnergy += d;
                    HitEnemyEvent(d / fireSword.energyToUseFireSword);
                }
                    
            }
        }

        foreach (var item in destructibles)
            item.Break();

        if (enemies.Count() > 0 || destructibles.Count() > 0)
            _playerCamera.CameraShakeSmooth(0.5f, 5, 0.5f);
    }

    public void MakeDamageChargeAttack()
    {
        StartCoroutine(ChargeAttackDamageCorruntine());
    }

    public IEnumerator ChargeAttackDamageCorruntine()
    {
        yield return new WaitForSeconds(0.1f);
        var enemies = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>());
        var destructibles = Physics.OverlapSphere(transform.position, viewDistanceAttack / 2).Where(x => x.GetComponent<DestructibleOBJ>()).Select(x => x.GetComponent<DestructibleOBJ>());

        

        foreach (var item in enemies)
        {
            item.GetDamage(ChargeAttackDamage, DamageType.Heavy);
            if (fireSwordCurrentTime <= 0)
            {
                fireEnergy += ChargeAttackDamage;
                HitEnemyEvent(ChargeAttackDamage / fireSword.energyToUseFireSword);
            }
                
        }

        foreach (var item in destructibles)
            item.Break();
    }

    public void CombatStateUp(bool causedByEnemy = true)
    {
        if (causedByEnemy)
            SoundManager.instance.CombatMusic(true);

        timeOnCombat = maxTimeOnCombat;

        if (!isInCombat) StartCoroutine(SetTimerCombat());
        
    }

    public void Defence()
    {
        if (isInCombat && chargeAttackAmount <=0.1f && !onAction && !OnDamage)
        {
            defenceTimer += Time.deltaTime; 
            attackCombo = 0;
            resetAttackTimer = 0;
            _onAttackAnimationTimer = 0;
            _timeToWaitBeforeAttack = 0;
            _movementAttackTime = 0;
            StartCoroutine(DefenceMove());
            onDefence = true;
            cantAttack = false;
            DefenceEvent(true);
        }
    }

    public void DefenceOff()
    {
        defenceTimer = 0;
        onDefence = false;
        DefenceEvent(false);
    }

    public void ChargingAttack()
    {
        if (chargeAttackColdown <= 0 && !onDefence && timeOnFailAttack <=0)
        {
            DefenceOff();
            chargeAttackAmount += Time.deltaTime;
        }
    }

    public void ChargeAttackZero()
    {
        chargeAttackAmount = 0;
    }
   
    public void CantChargeAttack()
    {
       if(!_chargeAttackSound) SoundManager.instance.Play(Player.CANT_HEAVY_ATTACK, transform.position, true);
    }

    public void ChargeAttack(float time)
    {       
        if (time >= chargeAttackTime && !chargeAttackCasted && !OnDamage)
        {
            CombatStateUp(false);
            ChargeAttackEvent();
            attackCombo = -1;
            resetAttackTimer = 0.7f;
            _movementAttackTime = 0.05f;
            StartCoroutine(CanCastChargeAttack());
            StartCoroutine(DecressChargeAttackColdown());
            StartCoroutine(OnActionState(0.3f));
            MakeDamageChargeAttack();
            StartCoroutine(CanChargeAttackSound());
        }
        chargeAttackAmount = 0;
    }

    IEnumerator CanChargeAttackSound()
    {
        _chargeAttackSound = true;
        yield return new WaitForSeconds(0.4f);
        _chargeAttackSound = false;
    }

    IEnumerator DecressChargeAttackColdown()
    {
        while(chargeAttackColdown < chargeAttackColdownMax)
        {
            chargeAttackColdown += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        chargeAttackColdown = 0;
    }

    public void ChangeActionState(bool b) { onAction = b; }

 
    public void PowerWeapon()
    {
        if(fireEnergy >= fireSword.energyToUseFireSword && isInCombat)
        {
            StartCoroutine(PowerOn());
            FireSwordEvent();
        }
    }

    public IEnumerator PowerOn()
    {
        fireEnergy = 0;
        flamesOn = true;
        StartCoroutine(OnActionState(1));
        while(fireSwordCurrentTime < fireSword.fireSwordTime)
        {
            CombatStateUp(false);
            fireSwordCurrentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PowerDesactivatedEvent();
        fireSwordCurrentTime = 0;
        flamesOn = false;
    }

    public IEnumerator CanCastChargeAttack()
    {
        _viewer.chargeAttackBar.transform.GetChild(0).gameObject.SetActive(true);
        chargeAttackCasted = true;       
        yield return new WaitForSeconds(3f);
        chargeAttackCasted = false;
        _viewer.chargeAttackBar.transform.GetChild(0).gameObject.SetActive(false);
        attackCombo = 0;
    }

    public void UpdateLife(float val)
    {
        life += val;
        if (life > maxLife) life = maxLife;
        _viewer.UpdateLife(life / maxLife);
    }

    public void GetDamage(float d, Transform target, DamageType type)
    {
        if (target.GetComponent<ClassEnemy>() && !onDodge && !isDead)
        {
            Vector3 toTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Dot(toTarget, transform.forward) > 0 && type == DamageType.Light)
            {
                if (!onDefence)
                {
                    UpdateLife(-d);
                    if (!onAction)
                    {
                        onDamageTime = 0.5f;
                        DefenceOff();
                        rb.AddForce(-transform.forward * 80, ForceMode.Impulse);
                        GetHitEvent();
                    }
                }

                if (defenceTimer <= 0.5f && !onAction && onDefence && !target.GetComponent<Model_B_Ogre1>())
                {
                    DefenceOff();
                    BlockEvent(true);
                    KickTutorialEvent();
                    StartCoroutine(OnActionState(0.9f));
                    target.GetComponent<ClassEnemy>().Knocked();

                }

                if (onDefence && defenceTimer > 0.5f || onDefence && target.GetComponent<Model_B_Ogre1>())
                {
                    BlockEvent(false);
                    DefendTutorialEvent();
                    StartCoroutine(OnActionState(0.4f));
                    if(!target.GetComponent<Model_B_Ogre1>()) target.GetComponent<ClassEnemy>().BlockedAttack();
                }
           
            }

            if (Vector3.Dot(toTarget, transform.forward) < 0 && type == DamageType.Light)
            {
                UpdateLife(-d);
                if (!onAction)
                {               
                    onDamageTime = 0.5f;
                    DefenceOff();
                    rb.AddForce(transform.forward * 80, ForceMode.Impulse);
                    GetHitEvent();
                }
            }

            if (Vector3.Dot(toTarget, transform.forward) > 0 && type == DamageType.Heavy)
            {
                UpdateLife(-d);
                if (onDamageTime <= 0 && !onAction)
                {
                    onDamageTime = 2f;
                    DefenceOff();
                    rb.AddForce(-transform.forward * 250, ForceMode.Impulse);
                    GetHitHeavyEvent(false);
                }
            }

            if (Vector3.Dot(toTarget, transform.forward) < 0 && type == DamageType.Heavy)
            {
                UpdateLife(-d);
                if (onDamageTime <= 0 && !onAction)
                {
                    DefenceOff();
                    onDamageTime = 1.9f;
                    rb.AddForce(transform.forward * 250, ForceMode.Impulse);
                    GetHitHeavyEvent(true);
                }
            }
        }

        if (target.GetComponent<MageMissile>() && !onDodge)
        {
            UpdateLife(-d);
            if (!onAction)
            {
                onDamageTime = 0.5f;
                DefenceOff();
                GetHitEvent();
            }
        }

        if (life<=0 && !isDead)
        {
            DieEvent();
            StartCoroutine(DeadCorrutine());
        }

    }

    public IEnumerator DeadCorrutine()
    {
        isDead = true;
        yield return new WaitForSeconds(1);
        _levelFade.FadeIn(false);
        while (_levelFade.fadeIn)
        {
            yield return new WaitForEndOfFrame();
        }
        transform.position = revivePos;
        transform.forward = reviveForward;
        _viewer.AnimRevive();
        life = maxLife;
        UpdateLife(maxLife);
        fireSword.currentExp = 0;
        yield return new WaitForSeconds(1);
        timeOnCombat = 0;
        _levelFade.FadeOut(false);
        isDead = false;
    }

    public void LockEnemies()
    {
       
        if (onLock && isInCombat)
        {
            LockedOffEvent();
            onLock = false;
            lockedEnemies.Clear();
            targetEnemy = null;
            _indexEnemyLock = 0;
            return;
        }

        if (!onLock && isInCombat)
        {
            lockedEnemies.Clear();

            var enemies = Physics.OverlapSphere(transform.position, 20).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>()).Where(x => 
            {
                var _dirToTarget = (x.transform.position - transform.position).normalized;

                var _distanceToTarget = Vector3.Distance(transform.position, x.transform.position);

                RaycastHit hit;

                bool onSight = false;

                if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), _dirToTarget, out hit, _distanceToTarget, playerCanSee))
                {
                    if (hit.transform.GetComponent<ClassEnemy>()) onSight = true;
                }

                if (onSight) return true;

                else return false; 

            })
            .OrderBy(x=> 
            {
                var enemyCamPos = _mainCam.WorldToScreenPoint(x.transform.position);

                var d = Vector3.Distance(enemyCamPos, new Vector3(Screen.width / 2, Screen.height / 2));

                return d;
            });


            if (enemies.Count() > 0)
            {
                targetEnemy = enemies.First();
                lockedEnemies.Add(enemies.First());
                lockedEnemies.AddRange(targetEnemy.sameID_Enemies);                
                LockedOnEvent();
                onLock = true;
                StartCoroutine(LockOnMovement());
                return;
            }

           
        }
    }

    public void ChangeTarget()
    {
        if (onLock)
        {
            _indexEnemyLock++;

            if (_indexEnemyLock >= lockedEnemies.Count) _indexEnemyLock = 0;

            targetEnemy = lockedEnemies[_indexEnemyLock];
        }
    }

    public void ModifyNodes()
    {

        var aggresisveNodes = Physics.OverlapSphere(transform.position, distanceAggressiveNodes).Where(x => x.GetComponent<CombatNode>()).Select(x => x.GetComponent<CombatNode>());

        if (aggresisveNodes.Count() > 0)
        {
            foreach (var item in aggresisveNodes)
            {
                item.aggressive = true;
            }
        }

        var non_AggresisveNodes = Physics.OverlapSphere(transform.position, distanceNon_AggressiveNodes).Where(x => x.GetComponent<CombatNode>()).Select(x => x.GetComponent<CombatNode>());

        if (non_AggresisveNodes.Count() > 0)
        {
            foreach (var item in non_AggresisveNodes)
            {
                if (!item.aggressive) item.Non_Aggressive = true;
            }
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

    public void OnDrawGizmos()
    {
      
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.3f, 0), distanceAggressiveNodes);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.3f, 0), distanceNon_AggressiveNodes);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit2 = Quaternion.AngleAxis(angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceAttack));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceAttack));

        Gizmos.color = Color.black;

        Vector3 rightLimit = Quaternion.AngleAxis(60, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistanceAttack));

        Vector3 leftLimit = Quaternion.AngleAxis(-60, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistanceAttack));
    }

}
