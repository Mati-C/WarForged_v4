using System.Collections;
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
    PlayerSword _sword;
    Camera _mainCam;

  
    [Header("Player Life:")]
    public float life;
    public float maxLife;

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

    [Header("Player Layers:")]

    public LayerMask playerCanSee;

    [Header("Player CombatValues:")]

    public int attackCombo;
    public float resetAttackTimer;
    public float timeOnCombat;
    public float maxTimeOnCombat;
    public enum DogeDirecctions { Left, Right, Back, Roll };
    public DogeDirecctions dirToDodge;

    [Header("Player AttackMovementImpulses:")]
    public float impulseAttackMovement1;
    public float impulseAttackMovement2;
    public float impulseAttackMovement3;
    public float impulseAttackMovement4;

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
    public Action<bool> DefenceEvent;

    public Action<bool> CombatStateEvent;
    public Action<DogeDirecctions> DodgeEvent;

    public PlayerCamera GetPlayerCam() { return _playerCamera; }

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

                if (resetAttackTimer < 0.2f && attackCombo == 1) cantAttack = false;
                if (resetAttackTimer < 0.3f && attackCombo != 1) cantAttack = false;
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

    IEnumerator DodgeMovement(float timer, Vector3 dir, float dodgeSpeed)
    {
        onDodge = true;

        while(timer >0)
        {
            timer -= Time.deltaTime;
            _rb.MovePosition(transform.position + dir * dodgeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.3f);
        onDodge = false;
    }

    public void ActivateSword() { _sword.ActivateSword(); }

    public void DesactivateSword() { _sword.DesactivateSword(); }

    private void Awake()
    {
        _viewer = GetComponent<Viewer_Player>();
        _playerCamera = FindObjectOfType<PlayerCamera>();
        _mainCam = _playerCamera.GetComponent<Camera>();
        _controller = new Controller_Player(this,_viewer);
        _rb = GetComponent<Rigidbody>();
        _sword = FindObjectOfType<PlayerSword>();
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

        if(!onDodge && !onAttackAnimation && onLock && !onDefence)
        {
            WalkEvent();
            _rb.MovePosition(_rb.position + d  * speed * Time.deltaTime);
        }

        if (!onDodge && !onAttackAnimation && !onLock && !onDefence)
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
                RunEvent();
                _rb.MovePosition(transform.position + d * runSpeed * Time.deltaTime);
            }
        }
    }

    public void Dodge(DogeDirecctions direction)
    {
        attackCombo = 0;

        if (!onDefence)
        {

            if (direction == DogeDirecctions.Roll)
            {
                DodgeEvent(DogeDirecctions.Roll);
                if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll));
            }

            if (direction == DogeDirecctions.Back)
            {
                if (!isInCombat)
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll));
                }

                else if (!onDodge)
                {
                    DodgeEvent(DogeDirecctions.Back);
                    StartCoroutine(DodgeMovement(0.3f, -transform.forward, dodgeSpeedBack));
                }
            }

            if (direction == DogeDirecctions.Right)
            {
                if (!isInCombat)
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll));
                }

                else if (!onDodge)
                {
                    DodgeEvent(DogeDirecctions.Right);
                    StartCoroutine(DodgeMovement(0.3f, transform.right, dodgeSpeedRight));
                }
            }

            if (direction == DogeDirecctions.Left)
            {
                if (!isInCombat)
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll));
                }

                else if (!onDodge)
                {
                    DodgeEvent(DogeDirecctions.Left);
                    StartCoroutine(DodgeMovement(0.3f, -transform.right, dodgeSpeedLeft));
                }
            }
        }
    }

    public void SwordAttack()
    {
        if (isInCombat)
        {
            if (!cantAttack && !onDefence)
            {
                if(attackCombo == 3)
                {
                    resetAttackTimer = 0.6f;
                    _onAttackAnimationTimer = 0.7f;
                    _timeToWaitBeforeAttack = 0.07f;
                    _movementAttackTime = 0.25f;
                    cantAttack = true;
                    onAttackAnimation = true;
                    attackCombo++;
                }

                if (attackCombo == 2)
                {
                    resetAttackTimer = 0.6f;
                    _onAttackAnimationTimer = 0.5f;
                    _timeToWaitBeforeAttack = 0.15f;
                    _movementAttackTime = 0.25f;
                    cantAttack = true;
                    onAttackAnimation = true;
                    attackCombo++;
                }

                if (attackCombo == 1)
                {
                    resetAttackTimer = 0.8f;
                    _onAttackAnimationTimer = 0.8f;
                    _timeToWaitBeforeAttack = 0.1f;
                    _movementAttackTime = 0.35f;
                    cantAttack = true;
                    onAttackAnimation = true;
                    attackCombo++;
                }

                if (attackCombo == 0)
                {
                    resetAttackTimer = 0.6f;
                    _onAttackAnimationTimer = 0.6f;
                    _timeToWaitBeforeAttack = 0.1f;
                    _movementAttackTime = 0.35f;
                    cantAttack = true;
                    onAttackAnimation = true;
                    attackCombo++;
                }
            }
       
        }

        CombatStateUp();
       
    }

    public void CombatStateUp()
    {
        timeOnCombat = maxTimeOnCombat;

        if (!isInCombat) StartCoroutine(SetTimerCombat());
        
    }

    public void Defence()
    {
        attackCombo = 0;
        resetAttackTimer = 0;
        _onAttackAnimationTimer = 0;
        _timeToWaitBeforeAttack = 0;
        _movementAttackTime = 0;
        StartCoroutine(DefenceMove());
        onDefence = true;
        DefenceEvent(true);
    }

    public void DefenceOff()
    {
        onDefence = false;
        DefenceEvent(false);
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

}
