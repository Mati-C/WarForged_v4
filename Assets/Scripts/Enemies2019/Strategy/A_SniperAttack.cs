using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_SniperAttack : i_EnemyActions
{
    ModelE_Sniper _e;

   public void Actions()
   {

        _e.IdleEvent();
        _e.target.CombatState();
        _e.target.saveSword = true;

        _e.timeToMove -= Time.deltaTime;

        if(!_e.changeMovementOnAttack && _e.timeToMove<=0)
        {
            int r = Random.Range(1, 3);

            _e.dirToMoveOnAttack = r;

            _e.timeToMove = Random.Range(3, 5);
        }

        if (_e.changeMovementOnAttack && _e.timeToMove <= 0)
        {

            _e.dirToMoveOnAttack = 0;

            _e.changeMovementOnAttack = false;

            _e.timeToMove = Random.Range(3, 5);
        }

        if(_e.dirToMoveOnAttack == 0  && _e.timeToShoot > 0 && _e.animClipName != _e.view.animDictionary[ViewerE_Sniper.EnemyWizzardAnims.Attack])
        {
            _e.IdleEvent();         
        }
        
        if(_e.dirToMoveOnAttack == 1 && _e.timeToShoot > 0 && _e.animClipName != _e.view.animDictionary[ViewerE_Sniper.EnemyWizzardAnims.Attack])
        {

            _e.FlyRightEvent();
            Quaternion targetRotation;
            var dir = (_e.target.transform.position - _e.transform.position).normalized;
            dir.y = 0;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 3 * Time.deltaTime);
            _e.rb.MovePosition(_e.transform.position + _e.transform.right * Time.deltaTime);
        }

        if (_e.dirToMoveOnAttack >= 2 && _e.timeToShoot > 0 && _e.animClipName != _e.view.animDictionary[ViewerE_Sniper.EnemyWizzardAnims.Attack])
        {

            _e.FlyLeftEvent();
            Quaternion targetRotation;
            var dir = (_e.target.transform.position - _e.transform.position).normalized;
            dir.y = 0;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 3 * Time.deltaTime);
            _e.rb.MovePosition(_e.transform.position - _e.transform.right * Time.deltaTime);
        }

        var obstacles = _e.ObstacleAvoidance();

        if (obstacles != Vector3.zero)
        {

            Quaternion targetRotation;
            var dir = (_e.target.transform.position - _e.transform.position).normalized;
            dir.y = 0;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 3 * Time.deltaTime);
            _e.rb.MovePosition(_e.rb.position + _e.transform.forward * Time.deltaTime);
        }


        if (_e.timeToShoot <=0 && !_e.onDamage && !_e.target.onCounterAttack)
        {

            Quaternion targetRotation;
            var _dir = (_e.target.transform.position - _e.transform.position).normalized;
            _dir.y = 0;
            targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 2 * Time.deltaTime);
            
            var _dirToTarget = (_e.target.transform.position - _e.transform.position).normalized;

            var _distanceToTarget = Vector3.Distance(_e.transform.position, _e.target.transform.position);

            RaycastHit hit;
        
            bool onSight = false;

            if (Physics.Raycast(_e.transform.position + new Vector3(0,1,0), _e.transform.forward, out hit, _distanceToTarget, _e.layerPlayer))
            {
                if (hit.transform.name == _e.target.name) onSight = true;
            }

            if (onSight &&  !_e.onDamage)
            {
                _e.AttackEvent();
                
            }

        }


    }

   public A_SniperAttack(ModelE_Sniper e)
   {
      _e = e;
   }
}
