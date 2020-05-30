﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Model_Player : MonoBehaviour
{
    Controller_Player _controller;
    Viewer_Player _viewer;
    Rigidbody _rb;
    PlayerCamera _playerCamera;
    Camera _mainCam;
    IA_CombatManager _IA_CM;
    public LayerMask layersCanSee;

  
    [Header("Player Life:")]
    public float life;
    public float maxLife;
    public float distanceAggressiveNodes;
    public float distanceNon_AggressiveNodes;

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
    bool _chargeAttackCasted;

    [Header("Player Powers Values:")]
    public float powerAmount;
    public float powerMax;
    public float powerCurrentTime;
    public float powerCurrentMaxTime;

    [Header("Player Damage Values:")]
    public float AttackDamageCombo1;
    public float AttackDamageCombo2;
    public float AttackDamageCombo3;
    public float ChargeAttackDamage;

    public enum DogeDirecctions { Left, Right, Back, Roll };
    public DogeDirecctions dirToDodge;

    [Header("Player AttackMovementImpulses:")]
    public float impulseAttackMovement1;
    public float impulseAttackMovement2;
    public float impulseAttackMovement3;
    public float impulseAttackMovement4;
    public float impulseChargeAttackMovement;

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
    public Action PowerActivatedEvent;
    public Action PowerDesactivatedEvent;
    public Action <float>HitEnemyEvent;
    public Action ChargeAttackEvent;
    public Action<bool> DefenceEvent;

    public Action<bool> CombatStateEvent;
    public Action<DogeDirecctions> DodgeEvent;

    public PlayerCamera GetPlayerCam() { return _playerCamera; }
    public IA_CombatManager GetIA_CombatManager() { return _IA_CM; }

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
        while (onDodge)
        {
            yield return new WaitForEndOfFrame();
        }

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

    }

    IEnumerator DodgeMovement(float timer, Vector3 dir, float dodgeSpeed, bool delay)
    {
        onDodge = true;

        if(delay) yield return new WaitForSeconds(0.2f);

        while (timer >0)
        {
            timer -= Time.deltaTime;
            _rb.MovePosition(transform.position + dir * dodgeSpeed * Time.deltaTime);
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
        _controller = new Controller_Player(this,_viewer);
        _rb = GetComponent<Rigidbody>();

        ModifyNodes();
    }

    void Start()
    {
        StartCoroutine(AttackMovement());
        StartCoroutine(AttackSword());
        StartCoroutine(AttackAnimationTimer());
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
                _rb.MovePosition(transform.position + d * speed * Time.deltaTime);
            }

            else
            {
                RunEvent();
                _rb.MovePosition(transform.position + d * runSpeed * Time.deltaTime);
            }
        }
    }

    public void CombatMovement(Vector3 d, bool turnDir, bool opposite)
    {

        if(!onDodge && !onAttackAnimation && onLock  && !onAction)
        {
            WalkEvent();
            _rb.MovePosition(_rb.position + d  * speed * Time.deltaTime);
        }

        if (!onDodge && !onAttackAnimation && !onLock && !onAction)
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
                _rb.MovePosition(transform.position + d * speed * Time.deltaTime);
            }

            else
            {

                if (!onDefence)
                {
                    RunEvent();
                    _rb.MovePosition(transform.position + d * runSpeed * Time.deltaTime);
                }

                else
                {
                    WalkEvent();
                    _rb.MovePosition(transform.position + d * speed * Time.deltaTime);
                }
            }
        }
    }

    public void Dodge(DogeDirecctions direction)
    {
        attackCombo = 0;
        if (direction == DogeDirecctions.Roll)
        {
            DodgeEvent(DogeDirecctions.Roll);

            StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll, false));
        }

        if (direction == DogeDirecctions.Back)
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
                    StartCoroutine(DodgeMovement(0.7f, -transform.forward, dodgeSpeedBack, false));
                }

            }
        }

        if (direction == DogeDirecctions.Right)
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

        if (direction == DogeDirecctions.Left)
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

        if (isInCombat)
        {        
            if (!cantAttack)
            {
              
                if (attackCombo == 2)
                {
   
                    resetAttackTimer = 0.8f;
                    StartCoroutine(AttackRotation(dir));
                    _onAttackAnimationTimer = 1f;
                    _timeToWaitBeforeAttack = 0.1f;
                    _movementAttackTime = 0.25f;
                    StartCoroutine(MakeAttackDamageDelay2(AttackDamageCombo3));
                    cantAttack = true;
                    onAttackAnimation = true;                   
                    attackCombo++;
                }

                if (attackCombo == 1)
                {

                    resetAttackTimer = 0.7f;
                    StartCoroutine(AttackRotation(dir));
                    _onAttackAnimationTimer = 1f;
                    _timeToWaitBeforeAttack = 0.1f;
                    _movementAttackTime = 0.35f;
                    StartCoroutine(MakeAttackDamageDelay1(AttackDamageCombo2));
                    cantAttack = true;
                    onAttackAnimation = true;
                    attackCombo++;
                }

                if (attackCombo == 0)
                {
                    
                    resetAttackTimer = 0.4f;
                    StartCoroutine(AttackRotation(dir));                   
                    _onAttackAnimationTimer = 0.65f;
                    _timeToWaitBeforeAttack = 0.2f;
                    _movementAttackTime = 0.35f;
                    MakeDamage(AttackDamageCombo1);
                    cantAttack = true;
                    onAttackAnimation = true;  
                    attackCombo++;
                }
            }
       
        }

        if(!_viewer.layerUpActive) CombatStateUp();
       
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

    public void MakeDamage(float d)
    {
        var enemies = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>());


        foreach (var item in enemies)
        {
            if (CanSee(item.transform, viewDistanceAttack, angleToAttack, layersCanSee))
            {
                item.GetDamage(d);
                if (powerCurrentTime <= 0)
                {
                    HitEnemyEvent(d / powerMax);
                    powerAmount += d;
                }
            }
        }
    }

    public void MakeDamageChargeAttack()
    {
        StartCoroutine(ChargeAttackDamageCorruntine());
    }

    public IEnumerator ChargeAttackDamageCorruntine()
    {
        yield return new WaitForSeconds(0.1f);
        var enemies = Physics.OverlapSphere(transform.position, viewDistanceAttack).Where(x => x.GetComponent<ClassEnemy>()).Select(x => x.GetComponent<ClassEnemy>());


        foreach (var item in enemies)
        {
            item.GetDamage(ChargeAttackDamage);
            if (powerCurrentTime <= 0)
            {
                HitEnemyEvent(ChargeAttackDamage / powerMax);
                powerAmount += ChargeAttackDamage;
            }
        }
    }

    public void CombatStateUp()
    {
        timeOnCombat = maxTimeOnCombat;

        if (!isInCombat) StartCoroutine(SetTimerCombat());
        
    }

    public void Defence()
    {
        if (isInCombat && chargeAttackAmount <=0 && !onAction)
        {
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
        onDefence = false;
        DefenceEvent(false);
    }

    public void ChargingAttack()
    {
        if (chargeAttackColdown <= 0)
        {
            DefenceOff();
            chargeAttackAmount += Time.deltaTime;
        }
    }

    public void ChargeAttackZero() { chargeAttackAmount = 0; }
   
    public void ChargeAttack(float time)
    {       
        if (time >= chargeAttackTime && !_chargeAttackCasted)
        {
            resetAttackTimer = 0.6f;
            attackCombo = -1;
            _movementAttackTime = 0.05f;
            StartCoroutine(CanCastChargeAttack());
            ChargeAttackEvent();
            if(chargeAttackColdown <= 0) StartCoroutine(DecressChargeAttackColdown());
            StartCoroutine(OnActionState(0.3f));
            MakeDamageChargeAttack();
        }
        chargeAttackAmount = 0;
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
        if(powerAmount >= powerMax)
        {
            StartCoroutine(PowerOn());
            PowerActivatedEvent();
        }
    }

    public IEnumerator PowerOn()
    {
        powerAmount = 0;
        while(powerCurrentTime < powerCurrentMaxTime)
        {
            powerCurrentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PowerDesactivatedEvent();
        powerCurrentTime = 0;
    }

    public IEnumerator CanCastChargeAttack()
    {
        _chargeAttackCasted = true;       
        yield return new WaitForSeconds(0.8f);
        _chargeAttackCasted = false;
        attackCombo = 0;
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
    }

}
