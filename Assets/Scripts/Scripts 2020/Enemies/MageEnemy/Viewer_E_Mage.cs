using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer_E_Mage : ClassEnemyViewer
{

    public Animator anim;
    public Model_E_Mage myModel;
    public ParticleSystem heavyHitParticle;

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

    private void Start()
    {
        myModel = GetComponent<Model_E_Mage>();
        StartCoroutine(DamageTimerAnim());
    }

    public void HeavyHitAntisipation()
    {
        heavyHitParticle.Clear();
        heavyHitParticle.Play();
    }

    public void AnimDie()
    {       
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
        AnimRagdollActivate(anim);
    }

    public void AnimWalkCombat()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("Idle", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
    }


    public void AnimIdleCombat()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", true);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
    }

    public void AnimWalkRight()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", true);
        anim.SetBool("Attack", false);
    }

    public void AnimWalkLeft()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Retreat", false);
        anim.SetBool("WalkLeft", true);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
    }

    public void AnimShootMagicAttack()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        StartCoroutine(DelayAnimActive("Attack", 1.2f));       
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

    public void AnimGetHit()
    {
        anim.SetBool("Walk", false);
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
}
