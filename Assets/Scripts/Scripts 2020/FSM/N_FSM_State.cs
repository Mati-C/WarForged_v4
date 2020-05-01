using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class N_FSM_State 
{
    public event Action OnUpdate = delegate { };
    public event Action OnFixedUpdate = delegate { };
    public event Action OnLateUpdate = delegate { };
    public event Action OnEnter = delegate { };
    public event Action OnExit = delegate { };

    public string Name { get { return _name; } }

    string _name;

    public N_FSM_State(string name)
    {
        _name = name;
    }

    public void Update()
    {
        OnUpdate();
    }

    public void LateUpdate()
    {
        OnLateUpdate();
    }

    public void FixedUpdate()
    {
        OnFixedUpdate();
    }

    public void Enter()
    {
        OnEnter();
    }

    public void Exit()
    {
        OnExit();
    }
}
