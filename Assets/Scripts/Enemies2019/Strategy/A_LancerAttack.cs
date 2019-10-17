using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class A_LancerAttack : i_EnemyActions
{
    ModelE_Lancer _e;

    public void Actions()
    {
        _e.target.CombatState();
        _e.target.saveSword = true;


        if (!_e.onDamage)
        {
            if (!_e.onAttackArea && !_e.onDamage)
            {
                Quaternion targetRotation;
                var dir = (_e.target.transform.position - _e.transform.position).normalized;
                dir.y = 0;
                var avoidObs = _e.ObstacleAvoidance();
                avoidObs.y = 0;
                var avoidEntites = _e.EntitiesAvoidance();
                avoidEntites.y = 0;
                targetRotation = Quaternion.LookRotation(dir + avoidEntites + avoidObs, Vector3.up);
                _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 7 * Time.deltaTime);
                _e.rb.MovePosition(_e.rb.position + _e.transform.forward * _e.speed * 2 * Time.deltaTime);
                _e.AttackRunEvent();
            }

            if (_e.target.onCounterAttack)
            {
                Quaternion targetRotation;
                var dir = (_e.target.transform.position - _e.transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                _e.CombatIdleEvent();
            }

            else if (_e.onAttackArea && !_e.onRetreat && !_e.target.onCounterAttack)
            {

                if (!_e.isPersuit && !_e.isWaitArea) _e.FollowState();

                _e.transform.LookAt(_e.target.transform.position);

                var player = Physics.OverlapSphere(_e.attackPivot.position, _e.radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();

                if (player != null)
                {
                    _e.AttackEvent();
                    _e.view.anim.SetBool("WalkForward", false);
                    _e.onRetreat = true;

                }

            }
        }
    }

    public A_LancerAttack(ModelE_Lancer e)
    {
        _e = e;
    }


}
