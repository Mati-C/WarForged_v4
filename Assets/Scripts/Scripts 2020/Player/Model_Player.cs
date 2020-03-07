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

    [Header("Player States:")]
    public bool run;
    public bool isInCombat;

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

    public void CombatMovement(Vector3 d, bool turnDir, bool opposite)
    {
        Quaternion targetRotation;

        d.y = 0;

        if ((turnDir || run) && !opposite)
        {
          
            targetRotation = Quaternion.LookRotation(d, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
        }

        if ((!turnDir || !run) && !opposite)
        {
            targetRotation = Quaternion.LookRotation(_camera.transform.forward, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
        }

        if(opposite)
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
