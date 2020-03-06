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

    public Action idleEvent;
    public Action WalkEvent;
    public Action RunEvent;

    public PlayerCamera GetPlayerCam() { return _camera; }

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
}
