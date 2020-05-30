using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Viewer_E_Melee : ClassEnemyViewer
{

    public Animator anim;

    public IEnumerator DelayAnimActive(string animName, float t)
    {
        anim.SetBool(animName, true);
        yield return new WaitForSeconds(t);
        anim.SetBool(animName, false);
    }

    public void AnimWalkCombat()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
    }


    public void AnimRunCombat()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", true);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
    }

    public void AnimIdleCombat()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
    }

    public void AnimWalkRight()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", true);
    }

    public void AnimWalkLeft()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", true);
        anim.SetBool("WalkRight", false);
    }

    public void AnimComboAttack()
    {
        StartCoroutine(DelayAnimActive("AttackCombo", 2));       
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
    }

    public void AnimHeavyAttack()
    {
        StartCoroutine(DelayAnimActive("HeavyAttack", 3));
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
    }

    public void AnimRetreat()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", true);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("AttackCombo", false);
    }
}
