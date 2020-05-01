using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_FSM_EventMachine
{
    N_FSM_State _currentState;

    public N_FSM_EventMachine(N_FSM_State state)
    {
        _currentState = state;
    }

    public void Update()
    {
        _currentState.Update();
    }

    public void LateUpdate()
    {
        _currentState.LateUpdate();
    }

    public void FixedUpdate()
    {
        _currentState.FixedUpdate();
    }


    public void ChangeState(N_FSM_State state)
    {
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }
}
