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
    IA_CombatManager _IA_CM;

  
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
    bool _chargeAttackCasted;

    [Header("Player Damage Values:")]
    public float AttackDamage;
    public float AttackDamageCombo1;
    public float AttackDamageCombo2;
    public float AttackDamageCombo3;

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

    public void ActivateSwordAttack() { _sword.ActivateSword(); }

    public void DesactivateSwordAttack() { _sword.DesactivateSword(); }

    private void Awake()
    {
        _viewer = GetComponent<Viewer_Player>();
        _playerCamera = FindObjectOfType<PlayerCamera>();
        _mainCam = _playerCamera.GetComponent<Camera>();
        _IA_CM = FindObjectOfType<IA_CombatManager>();
        _controller = new Controller_Player(this,_viewer);
        _rb = GetComponent<Rigidbody>();
        _sword = FindObjectOfType<PlayerSword>();

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
                RunEvent();
                if(!onDefence) _rb.MovePosition(transform.position + d * runSpeed * Time.deltaTime);

                else _rb.MovePosition(transform.position + d * speed * Time.deltaTime);
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
                if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll, false));
            }

            if (direction == DogeDirecctions.Back)
            {
                if (!isInCombat)
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll,false));
                }

                else if (!onDodge)
                {
                    DodgeEvent(DogeDirecctions.Back);
                    StartCoroutine(DodgeMovement(0.4f, -transform.forward, dodgeSpeedBack, true));
                }
            }

            if (direction == DogeDirecctions.Right)
            {
                if (!isInCombat)
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll, false));
                }

                else if (!onDodge)
                {
                    DodgeEvent(DogeDirecctions.Right);
                    StartCoroutine(DodgeMovement(0.4f, transform.right, dodgeSpeedRight, true));
                }
            }

            if (direction == DogeDirecctions.Left)
            {
                if (!isInCombat)
                {
                    DodgeEvent(DogeDirecctions.Roll);
                    if (!onDodge) StartCoroutine(DodgeMovement(0.7f, transform.forward, dodgeSpeedRoll, false));
                }

                else if (!onDodge)
                {
                    DodgeEvent(DogeDirecctions.Left);
                    StartCoroutine(DodgeMovement(0.4f, -transform.right, dodgeSpeedLeft, true));
                }
            }
        }
    }

    public void SwordAttack(Vector3 dir)
    {
        dir.y = 0;

        if (isInCombat)
        {        
            if (!cantAttack && !onDefence)
            {
                if (_sword == null) _sword = FindObjectOfType<PlayerSword>();

                if (attackCombo == 2)
                {
                    AttackDamage = AttackDamageCombo3;
                    resetAttackTimer = 0.6f;
                    StartCoroutine(AttackRotation(dir));
                    _onAttackAnimationTimer = 0.7f;
                    _timeToWaitBeforeAttack = 0.1f;
                    _movementAttackTime = 0.25f;
                    cantAttack = true;
                    onAttackAnimation = true;                   
                    attackCombo++;
                }

                if (attackCombo == 1)
                {
                    AttackDamage = AttackDamageCombo2;
                    resetAttackTimer = 0.7f;
                    StartCoroutine(AttackRotation(dir));
                    _onAttackAnimationTimer = 0.8f;
                    _timeToWaitBeforeAttack = 0.1f;
                    _movementAttackTime = 0.35f;
                    cantAttack = true;
                    onAttackAnimation = true;
                    attackCombo++;
                }

                if (attackCombo == 0)
                {
                    AttackDamage = AttackDamageCombo1;
                    resetAttackTimer = 0.55f;
                    StartCoroutine(AttackRotation(dir));                   
                    _onAttackAnimationTimer = 0.65f;
                    _timeToWaitBeforeAttack = 0.2f;
                    _movementAttackTime = 0.35f;
                    cantAttack = true;
                    onAttackAnimation = true;  
                    attackCombo++;
                }
            }
       
        }

        if(!_viewer.layerUpActive) CombatStateUp();
       
    }

    public void CombatStateUp()
    {
        timeOnCombat = maxTimeOnCombat;

        if (!isInCombat) StartCoroutine(SetTimerCombat());
        
    }

    public void Defence()
    {
        if (isInCombat)
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
    }

    public void DefenceOff()
    {
        onDefence = false;
        DefenceEvent(false);
    }

    public void ChargeAttack(float time)
    {
        
        if (!_chargeAttackCasted)
        {         
           if (time >= 1.6f) ChargeAttackDone(time);
        }
    }

    public void ChargeAttackDone(float time)
    {
        chargeAttackAmount = 0;
        if (time >= 1.6f)
        {
            StartCoroutine(CanCastChargeAttack());
            StartCoroutine(OnActionState(0.3f));
        }
    }

    public void ChangeActionState(bool b) { onAction = b; }


    public IEnumerator CanCastChargeAttack()
    {
        _chargeAttackCasted = true;
        yield return new WaitForSeconds(0.8f);
        _chargeAttackCasted = false;        
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

    public void OnDrawGizmos()
    {
      
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.3f, 0), distanceAggressiveNodes);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.3f, 0), distanceNon_AggressiveNodes);
    }

}
