﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class A_ShieldRetreat : i_EnemyActions
{
    ModelE_Shield _e;

    public void Actions()
    {


        _e.target.CombatState();
        _e.target.saveSword = true;

        var d = Vector3.Distance(_e.transform.position, _e.target.transform.position);

        float maxD = 0;

        if (_e.aggressiveLevel == 1) maxD = 2.5f;
        if (_e.aggressiveLevel == 2) maxD = 5f;

        Quaternion targetRotation;

        var dir = (_e.target.transform.position - _e.transform.position).normalized;
        dir.y = 0;
        targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 2 * Time.deltaTime);

        if (_e.onRetreat && _e.timeToRetreat > 0 && d < maxD && _e.animClipName == _e.view.animDictionary[ViewerE_Shield.EnemyMeleeAnim.Retreat])
        {


            _e.rb.MovePosition(_e.rb.position - _e.transform.forward * _e.speed * Time.deltaTime);

            _e.timeToAttack = false;
            _e.WalkBackEvent();

            RaycastHit hit;

            if (Physics.Raycast(_e.transform.position, -_e.transform.forward, out hit, 0.5f, _e.layerObst))
            {

                _e.timeToRetreat = 0;
            }
        }

        if (_e.onRetreat && _e.animClipName != _e.view.animDictionary[ViewerE_Shield.EnemyMeleeAnim.Attack1] && _e.animClipName != _e.view.animDictionary[ViewerE_Shield.EnemyMeleeAnim.WalkForward] && _e.animClipName != _e.view.animDictionary[ViewerE_Shield.EnemyMeleeAnim.Blocked] && _e.timeToRetreat > 0 && d > maxD)
        {


            var obs = Physics.OverlapSphere(_e.transform.position, 0.5f, _e.layerEntites).Where(x => x.GetComponent<ModelE_Shield>()).Select(x => x.GetComponent<ModelE_Shield>()).Where(x => x != _e).ToList();
            obs.Remove(_e);

            if (obs.Count > 0)
            {
                _e.view.anim.SetBool("WalkBack", false);
                var dirAvoid = (_e.transform.position - obs[0].transform.position).normalized;
                dirAvoid.y = 0;
                _e.rb.MovePosition(_e.rb.position + dirAvoid * _e.speed * Time.deltaTime);

                bool left = false;
                bool right = false;

                var relativePoint = _e.transform.InverseTransformPoint(obs[0].transform.position);

                if (relativePoint.x < 0.0) left = true;

                if (relativePoint.x > 0.0) right = true;


                if (left && !right) _e.WalkLeftEvent();

                if (!left && right) _e.WalkRightEvent();
            }

            if (obs.Count <= 0)
            {
                _e.view.anim.SetBool("WalkBack", false);
                _e.CombatIdleEvent();
                _e.timeToAttack = false;
            }

            RaycastHit hit;

            if (Physics.Raycast(_e.transform.position, -_e.transform.forward, out hit, 0.5f, _e.layerObst))
            {
                _e.timeToRetreat = 0;
            }

        }

    }

    public A_ShieldRetreat(ModelE_Shield e)
    {
        _e = e;
    }
}
