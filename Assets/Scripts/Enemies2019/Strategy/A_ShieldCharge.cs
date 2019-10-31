using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_ShieldCharge : i_EnemyActions
{
    ModelE_Shield _m;

   public void Actions()
   {
        Debug.Log("1");
       if(_m.animClipName != _m.view.animDictionary[ViewerE_Shield.EnemyMeleeAnim.Knocked]) _m.rb.MovePosition(_m.transform.position + _m.transform.forward * _m.speed * 2 * Time.deltaTime);
   }

   public A_ShieldCharge(ModelE_Shield m)
   {        
        _m = m;
        _m.onCharge = true;
   }
}
