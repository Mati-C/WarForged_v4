using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer_E_Lancer : ClassEnemyViewer
{
    public Animator anim;
    public Model_E_Lancer myModel;
    public ParticleSystem heavyHitParticle;
    public ParticleSystem bloodParticle;

    public IEnumerator DelayAnimActive(string animName, float t)
    {
        anim.SetBool(animName, true);
        yield return new WaitForSeconds(t);
        anim.SetBool(animName, false);
    }

    IEnumerator DamageTimerAnim()
    {
        while (true)
        {
            if (myModel.onDamageTime <= 0) anim.SetInteger("GetHit", 0);

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator KnockedAnimTimer()
    {
        anim.SetBool("Blocked", true);
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        yield return new WaitForSeconds(0.6f);
        anim.SetBool("Blocked", false);
        anim.SetBool("Knocked", true);
        yield return new WaitForSeconds(2.5f);
        anim.SetBool("Knocked", false);
    }

    private void Start()
    {
        myModel = GetComponent<Model_E_Lancer>();
        StartCoroutine(DamageTimerAnim());
    }

    public void HeavyHitAntisipation()
    {
        heavyHitParticle.Clear();
        heavyHitParticle.Play();
    }

    public void AnimDie()
    {
        anim.SetBool("Die", true);
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
        anim.SetBool("Parry", false);
    }

    public void BlockedAnim()
    {
        StartCoroutine(DelayAnimActive("Blocked", 0.7f));
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Parry", false);
    }

    public void KnockedAnim()
    {
        StartCoroutine(KnockedAnimTimer());
    }

    public void AnimWalkCombat()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Parry", false);
    }


    public void AnimRunCombat()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", true);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Parry", false);
    }

    public void AnimIdleCombat()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Parry", false);
    }

    public void AnimWalkRight()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", true);
        anim.SetBool("Parry", false);
    }

    public void AnimWalkLeft()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", true);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Parry", false);
    }

    public void AnimComboAttack()
    {
        StartCoroutine(DelayAnimActive("AttackCombo", 2));
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Parry", false);
    }

    public void AnimRetreat()
    {
        anim.SetBool("Parry", false);
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", true);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("AttackCombo", false);
    }

    public void AnimGetHit()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Parry", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        bloodParticle.Clear();
        bloodParticle.Play();

        switch (anim.GetInteger("GetHit"))
        {
            case 0:
                anim.SetInteger("GetHit", 1);
                break;

            case 1:
                anim.SetInteger("GetHit", 2);
                break;

            case 2:
                anim.SetInteger("GetHit", 1);
                break;
        }

    }

    public void AnimParry()
    {
        anim.SetBool("Parry", true);
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);

    }  
}
