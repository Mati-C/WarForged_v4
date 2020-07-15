using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewerl_B_Ogre1 : ClassEnemyViewer
{
    public Model_B_Ogre1 myModel;

    public IEnumerator DelayAnimActive(string animName, float t)
    {
        anim.SetBool(animName, true);
        yield return new WaitForSeconds(t);
        anim.SetBool(animName, false);
    }

    private void Awake()
    {
        AwakeViewer();
        anim = GetComponent<Animator>();
        myModel = GetComponent<Model_B_Ogre1>();
        levelUI = GameObject.Find("LEVEL UI");
        ess = GetComponent<EnemyScreenSpace>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
      
    }

    public void AnimLightAttack()
    {
        StartCoroutine(DelayAnimActive("LightAttack", 1.2f));
        anim.SetBool("WalkLeft", false);       
        anim.SetBool("WalkRight", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }

    public void AnimHeavyAttack()
    {
        StartCoroutine(DelayAnimActive("HeavyAttack", 1.3f));
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }

    public void AnimComboAttack()
    {
        StartCoroutine(DelayAnimActive("ComboAttack", 2.3f));
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }

    public void AnimIdle()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("Walk", false);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
    }

    public void AnimWalk()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", true);
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
    }

    public void animWalkLeft()
    {
        anim.SetBool("WalkLeft", true);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }

    public void animWalkRight()
    {
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", true);
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }

    public void AnimGetHit()
    {
        bloodParticle.Clear();
        bloodParticle.Play();
    }

    public void AnimTaunt()
    {
        StartCoroutine(DelayAnimActive("Taunt", 2.3f));
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }
}
