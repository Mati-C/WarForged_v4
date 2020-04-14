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
    PlayerCamera _camera;
    PlayerSword _sword;

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

    float _onAttackAnimationTimer;
    float _timeToWaitBeforeAttack;
    float _movementAttackTime;
    Vector3 _attackLastPos;
    string _currentAnimName;
    
    public Action idleEvent;
    public Action WalkEvent;
    public Action RunEvent;
    public Action TakeSwordEvent;
    public Action SaveSwordEvent;
    public Action<bool> CombatStateEvent;
    public Action<DogeDirecctions> DodgeEvent;

    public PlayerCamera GetPlayerCam() { return _camera; }

  

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
        _camera = FindObjectOfType<PlayerCamera>();
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
        _currentAnimName = _viewer.animClipName;
        
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
        if (!onDodge && !onAttackAnimation)
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

                var dir = _camera.transform.forward;

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

        if(direction == DogeDirecctions.Roll)
        {
            DodgeEvent(DogeDirecctions.Roll);
            if(!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll));
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

    public void SwordAttack()
    {
        if (isInCombat)
        {
            if (!cantAttack)
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
                    resetAttackTimer = 0.5f;
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

 
}
