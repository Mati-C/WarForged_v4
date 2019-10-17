﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_SniperMeleeAttack : i_EnemyActions
{
    ModelE_Sniper _e;

    public void Actions()
    {
        _e.target.CombatState();
        _e.target.saveSword = true;

        if (_e.timeToRetreat <= 0)
        {
            _e.onRetreat = true;
        }

        Quaternion targetRotation;
        var _dir = (_e.target.transform.position - _e.transform.position).normalized;
        _dir.y = 0;
        targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
        _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 10 * Time.deltaTime);

        if (_e.timeToMeleeAttack <= 0)
        {
            _e.AttackMeleeEvent();
            _e.timeToMeleeAttack = Random.Range(_e.minTimeDelayMeleeAttack, _e.maxTimeDelayMeleeAttack);
        }


    }
    public A_SniperMeleeAttack( ModelE_Sniper e)
    {
        _e = e;
    }
}
