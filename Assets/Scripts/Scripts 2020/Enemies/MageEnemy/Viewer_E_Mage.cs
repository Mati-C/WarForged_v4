﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

public class Viewer_E_Mage : ClassEnemyViewer
{

    public Model_E_Mage myModel;
    public ParticleSystem heavyHitParticle;

    public IEnumerator DelayAnimActive(string animName, float t)
    {
        anim.SetBool(animName, true);
        yield return new WaitForSeconds(t);
        anim.SetBool(animName, false);
    }

    public IEnumerator DamageTimerAnim()
    {
        while (true)
        {
            if (myModel.onDamageTime <= 0) anim.SetInteger("GetHit", 0);

            yield return new WaitForEndOfFrame();
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cam = FindObjectOfType<PlayerCamera>().GetComponent<Camera>();
        levelUI = GameObject.Find("LEVEL UI");
        ess = GetComponent<EnemyScreenSpace>();
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
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
        AnimRagdollActivate();

        SoundManager.instance.PlayRandom(SoundManager.instance.deathVoice, transform.position, false, 0.9f, 0.5f);
        SoundManager.instance.Play(Hit.SOFT, transform.position, true);
    }

    public void AnimWalkCombat()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("Idle", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
    }


    public void AnimIdleCombat()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", true);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Attack", false);
    }

    public void AnimWalkRight()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", true);
        anim.SetBool("Attack", false);
    }

    public void AnimWalkLeft()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
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
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("AttackCombo", false);
    }

    public void AnimGetHit()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
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

        SoundManager.instance.PlayRandom(SoundManager.instance.damageVoice, transform.position, false, 0.9f, 0.5f);
        SoundManager.instance.Play(Hit.SOFT, transform.position, true);
    }

}
