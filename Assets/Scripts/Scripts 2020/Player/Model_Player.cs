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

    [Header("Player Life:")]
    public float life;
    public float maxLife;

    [Header("Player Speeds:")]
    public float speed;
    public float runSpeed;
    public float dodgeSpeed;

    [Header("Player States:")]
    public bool run;
    public bool isInCombat;
    public bool onDodge;

    [Header("Player CombatValues:")]

    public float timeOnCombat;
    public float maxTimeOnCombat;

    public Action idleEvent;
    public Action WalkEvent;
    public Action RunEvent;
    public Action TakeSwordEvent;
    public Action SaveSwordEvent;
    public Action<bool> CombatStateEvent;

    public PlayerCamera GetPlayerCam() { return _camera; }

    public enum DogeDirecctions { Left, Right, Back, Roll };
    public DogeDirecctions dirToDodge;

    IEnumerator SetTimerCombat()
    {
        isInCombat = true;
        TakeSwordEvent();
        CombatStateEvent(true);

        while(timeOnCombat >0)
        {
            timeOnCombat -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        isInCombat = false;
        SaveSwordEvent();
        CombatStateEvent(false);
    }

    IEnumerator DodgeMovement(float timer, Vector3 dir)
    {
        onDodge = true;

        while(timer >0)
        {
            timer -= Time.deltaTime;
            _rb.MovePosition(transform.position + dir * dodgeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        onDodge = false;
    }

    private void Awake()
    {
        _viewer = GetComponent<Viewer_Player>();
        _camera = FindObjectOfType<PlayerCamera>();
        _controller = new Controller_Player(this,_viewer);
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        _controller.ControllerUpdate();
    }

    public void Movement(Vector3 d)
    {
        if (!onDodge)
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
        if (!onDodge)
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
        if(direction == DogeDirecctions.Roll)
        {
            StartCoroutine(DodgeMovement(0.5f, transform.forward));
        }

        if (direction == DogeDirecctions.Back)
        {
            StartCoroutine(DodgeMovement(0.5f, -transform.forward));
        }

        if (direction == DogeDirecctions.Right)
        {
            StartCoroutine(DodgeMovement(0.5f, transform.right));
        }

        if (direction == DogeDirecctions.Left)
        {
            StartCoroutine(DodgeMovement(0.5f, -transform.right));
        }
    }

    public void SwordAttack()
    {
        CombatStateUp();
    }

    public void CombatStateUp()
    {
        timeOnCombat = maxTimeOnCombat;

        if (!isInCombat) StartCoroutine(SetTimerCombat());
        
    }
}
