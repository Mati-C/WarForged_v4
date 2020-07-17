using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sound;

public class Viewerl_B_Ogre1 : ClassEnemyViewer
{
    public Model_B_Ogre1 myModel;
    Material _healthBarMat;
    public GameObject healthBar;
    public ParticleSystem smashParticles;
    PlayerCamera _cam;
    Model_Player _target;
    public bool onSmashAttack;

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
        healthBar = GameObject.Find("BossLifeBar");
        _healthBarMat = healthBar.GetComponent<Image>().material;
        healthBar.gameObject.SetActive(false);
        _cam = FindObjectOfType<PlayerCamera>();
        _target = FindObjectOfType<Model_Player>();
    }

    void Start()
    {
        
    }


    void Update()
    {
        if(healthBar.activeSelf) _healthBarMat.SetFloat("_BossLifePercentage", myModel.life / myModel.maxLife * 200);
        if(healthBar.activeSelf) _healthBarMat.SetFloat("_ArrowBeatRatePercentage", myModel.life / myModel.maxLife * 100);
    }

    private void LateUpdate()
    {
      
    }

    public void AnimLightAttack()
    {
        SoundManager.instance.PlayRandom(SoundManager.instance.bossAttack, transform.position, true, 1, 3);
        StartCoroutine(DelayAnimActive("LightAttack", 1.2f));
        anim.SetBool("WalkLeft", false);       
        anim.SetBool("WalkRight", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }

    public void AnimHeavyAttack()
    {
        SoundManager.instance.Play(Boss.ROAR, transform.position, true, 2);
        smashParticles.Clear();
        smashParticles.Play();
        smashParticles.transform.position = transform.position + transform.forward;
        StartCoroutine(SmashParticles());
        StartCoroutine(SmashShake());
        StartCoroutine(DelayAnimActive("HeavyAttack", 1.3f));
        anim.SetBool("WalkLeft", false);
        anim.SetBool("WalkRight", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
    }

    public void AnimDie()
    {
        anim.SetBool("Die", true);
        StartCoroutine(DieShake());
        SoundManager.instance.BossMusic(false);
    }

    public void CreateExpPopTextScapeBoss(float exp)
    {
        StartCoroutine(ChargeExpFireText(exp));
        PopExpText text = Instantiate(prefabExpText);
        StartCoroutine(FollowEnemyExpBoss(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetExp(exp);
    }

    public IEnumerator FollowEnemyExpBoss(PopExpText text)
    {
        while (text != null)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(_target.transform.position + (Vector3.up * 2));
            text.transform.position = screenPos;
            yield return new WaitForEndOfFrame();
        }
    }

    public void LastComboEffect()
    {
        StartCoroutine(LastComboAttack());
    }

    public void Dirt()
    {
        smashParticles.Clear();
        smashParticles.Play();
        smashParticles.transform.position = transform.position + transform.forward;
    }

    public IEnumerator LastComboAttack()
    {
        
        yield return new WaitForSeconds(0.1f);
        float t = 1;
        onSmashAttack = true;
        SoundManager.instance.Play(Boss.SMASH, transform.position, true, 3);
        while (t >0)
        {
            _cam.CameraShake(1.5f, 1.5f);
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        onSmashAttack = false;
        _cam.CameraShake(0,0);
    }

    IEnumerator DieShake()
    {

        yield return new WaitForSeconds(1.4f);
        smashParticles.Clear();
        smashParticles.Play();
        smashParticles.transform.position = transform.position;
        yield return new WaitForSeconds(0.1f);

        onSmashAttack = true;
        float t = 1;
        bool f = false;
        while (t > 0)
        {
            _cam.CameraShake(3, 3);
            t -= Time.deltaTime;

            if (t <= 1 && !f)
            {
                f = true;
                SoundManager.instance.Play(Boss.SMASH, transform.position, true, 3);
            }

            yield return new WaitForEndOfFrame();
        }
        _cam.CameraShake(0,0);
        onSmashAttack = false;
        
    }

    IEnumerator SmashShake()
    {
        onSmashAttack = true;
        _cam.CameraShake(1, 1);
        yield return new WaitForSeconds(1f);
        _cam.CameraShake(0, 0);
        onSmashAttack = false;
    }

    IEnumerator RoarShake()
    {
        _cam.CameraShake(4, 10);
        yield return new WaitForSeconds(1);
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            _cam.CameraShake(Mathf.Lerp(0, 4, t), Mathf.Lerp(0, 10, t));
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator SmashParticles()
    {
        yield return new WaitForSeconds(0.9f);        
        SoundManager.instance.Play(Boss.SMASH, transform.position, true, 3);
       
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
        SoundManager.instance.Play(Hit.SOFT, transform.position, true);
    }

    public void AnimTaunt()
    {
        SoundManager.instance.Play(Boss.ROAR, transform.position, true, 3);
        healthBar.gameObject.SetActive(true);
        StartCoroutine(DelayAnimActive("Taunt", 2.3f));
        anim.SetBool("Idle", false);
        anim.SetBool("Walk", false);
        _cam.CameraShakeSmooth(4, 10, 2);
    }
}
