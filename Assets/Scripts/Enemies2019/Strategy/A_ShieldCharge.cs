using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_ShieldCharge : i_EnemyActions
{
    ModelE_Shield _m;
    float timer = 3f;

   public void Actions()
   {
        timer -= Time.deltaTime;
        if(_m.animClipName == _m.view.animDictionary[ViewerE_Shield.EnemyMeleeAnim.Charge]) _m.rb.MovePosition(_m.transform.position + _m.transform.forward * _m.speed * 2 * Time.deltaTime);

        if (timer <= 0)
        {
            _m.view.anim.SetBool("OnCharge", false);
            _m.view.anim.SetBool("IdleCombat", true);
            _m.onCharge = false;
            _m.view.anim.SetBool("WalkForward", false);
            _m.StopRetreat();
            _m.SendInputToFSM(ModelE_Shield.EnemyInputs.WAIT);
        }
   }

   public A_ShieldCharge(ModelE_Shield m)
   {        
        _m = m;
        _m.onCharge = true;
   }
}
