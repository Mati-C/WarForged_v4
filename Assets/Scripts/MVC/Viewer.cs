﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Viewer : MonoBehaviour
{
    public Model model;
    public Controller controller;
    public Animator anim;
    bool turn;
    bool melleCombo1;
    bool melleCombo2;
    bool melleCombo3;
    public bool attacking;
    public bool onDamageAnim;
    public bool movementAnims;
    public bool dodgeAnims;
    public CamShake camShake;
    public Transform head;
    Quaternion headBaseRot;
    public ParticleSystem blood;
    public ParticleSystem healParticles;
    public ParticleSystem antisipationRing;
    public GameObject rotateImage;
    public GameObject coverImage;
    public GameObject fire1Image;
    public GameObject fire2Image;

    public Image power1;
    public Image power2;
    public Image power3;
    public Image power4;
    public Image defenceColdwon;

    public GameObject youWin;
    public GameObject phParticles;
    public GameObject trail;

    public Image lifeBar;
    public Image tempLifeBar;
    public Image manaBar;
    public Image armor;

    public Image[] potions;
    public Text potionTimer;

    public List<GameObject> particlesSowrd;

    public int currentAttackAnimation;

    public float preAttacktime;
    public string animClipName;

    float timeToSaveSword;

    public CamController cam;

    public GameObject pauseMenu;
    public GameObject maxDamageDisplay;

    bool slowSpeed;

    public ParticleSystem sparks;

    public enum AnimPlayerNames {Dead, Attack1_Pre, Attack1_Damage, Attack1_End, Attack2_Pre, Attack2_Damage, Attack2_End, Attack3_Pre, Attack3_Damage, Attack3_End, Attack4_Pre, Attack4_Damage, Attack4_End,
                                 W_Attack1_Pre, W_Attack1_Damage, W_Attack1_End, W_Attack2_Pre, W_Attack2_Damage, W_Attack2_End, W_Attack3_Pre, W_Attack3_Damage, W_Attack3_End, W_Attack4_Pre, W_Attack4_Damage, W_Attack4_End,
                                 Parry1, Parry2, Parry3, Parry4, BackAttack2, TurnAttack_Pre, TurnAttack_Damage, TurnAttack_End, TakeDamage1, TakeDamage2, TakeDamage3, Defence, Blocked, FailDefence, Kick, IdleCombat, WalkW, WalkS,
                                 WalkD, WalkA, RunCombat, W_RunCombat, Run, Walk, Idle, Roll, RollAttack, RollEstocada_Damage, Dodge_Left, Dodge_Back, Dodge_Right, Dodge_Left2, Dodge_Back2, Dodge_Right2, WalkW2, WalkS2, WalkD2, WalkA2, IdleCombat_Wizzard, CastSpell};

    public Dictionary<AnimPlayerNames, string> AnimDictionary = new Dictionary<AnimPlayerNames, string>();

    public RawImage startFade;
    [Header("Time of the initial fade from black:")]
    public float fadeTime;

    public GameObject smashParticle;

    public Image shieldIcon;
    public Image whirlIcon;

    public Transform orbPH;
    public GameObject orbPrefab;
    public float orbSpeed;
    public ButtonManager bm;

    public List<SkinnedMeshRenderer> gauntlets;
    List<Material> gauntletMaterials = new List<Material>();
    public List<GameObject> handEffects;

    public GameObject lockParticle;

    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetInteger("TakeDamage", 0);
    }

    public IEnumerator DestroyParticles(GameObject p)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(p);
    }

    public IEnumerator SaveSwordAnim()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetLayerWeight(1, 0);
    }

    public IEnumerator TakeSwordAnim()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetLayerWeight(1, 0);
    }

    public IEnumerator SmashParticleEvent()
    {
        smashParticle.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        trail.SetActive(false);

        if (animClipName == AnimDictionary[AnimPlayerNames.Attack4_Damage] || animClipName == AnimDictionary[AnimPlayerNames.Attack4_Pre] 
            || animClipName == AnimDictionary[AnimPlayerNames.Attack4_End] || animClipName == AnimDictionary[AnimPlayerNames.Attack3_End])
        {
            
            smashParticle.SetActive(true);
            trail.SetActive(false);
            ShakeCameraDamage(1, 1, 0.3f);
        }
    }

    /* public IEnumerator SlowAnimSpeed()
     {
         if (!slowSpeed)
         {
             slowSpeed = true;
             anim.speed = 0;
             yield return new WaitForSeconds(0.1f);
             anim.speed = 1;
             slowSpeed = false;
         }
     }
     */
    public void Start()
    {
        var clips = anim.runtimeAnimatorController.animationClips.ToList();

        // Iterate over the clips and gather their information
        /*int aux = 0;
        foreach (var animClip in clips)
        {
            Debug.Log(animClip.name + ": " + aux++);
        }
        */

        AnimDictionary.Add(AnimPlayerNames.Dead, clips[0].name);
        AnimDictionary.Add(AnimPlayerNames.Attack1_Pre, clips[1].name);
        AnimDictionary.Add(AnimPlayerNames.Attack1_Damage, clips[2].name);
        AnimDictionary.Add(AnimPlayerNames.Attack1_End, clips[3].name);
        AnimDictionary.Add(AnimPlayerNames.Attack2_Damage, clips[4].name);
        AnimDictionary.Add(AnimPlayerNames.Attack2_End, clips[5].name);
        AnimDictionary.Add(AnimPlayerNames.Attack2_Pre, clips[6].name);
        AnimDictionary.Add(AnimPlayerNames.Attack3_Damage, clips[7].name);
        AnimDictionary.Add(AnimPlayerNames.Attack3_End, clips[8].name);
        AnimDictionary.Add(AnimPlayerNames.Attack3_Pre, clips[9].name);
        AnimDictionary.Add(AnimPlayerNames.Attack4_Damage, clips[10].name);
        AnimDictionary.Add(AnimPlayerNames.Attack4_End, clips[11].name);
        AnimDictionary.Add(AnimPlayerNames.Attack4_Pre, clips[12].name);
        AnimDictionary.Add(AnimPlayerNames.Parry1, clips[13].name);
        AnimDictionary.Add(AnimPlayerNames.Parry2, clips[14].name);
        AnimDictionary.Add(AnimPlayerNames.Parry3, clips[15].name);
        AnimDictionary.Add(AnimPlayerNames.Parry4, clips[16].name);
        AnimDictionary.Add(AnimPlayerNames.BackAttack2, clips[17].name);
        AnimDictionary.Add(AnimPlayerNames.TurnAttack_Pre, clips[19].name);
        AnimDictionary.Add(AnimPlayerNames.TurnAttack_Damage, clips[20].name);
        AnimDictionary.Add(AnimPlayerNames.TurnAttack_End, clips[21].name);
        AnimDictionary.Add(AnimPlayerNames.TakeDamage2, clips[22].name);
        AnimDictionary.Add(AnimPlayerNames.TakeDamage3, clips[23].name);
        AnimDictionary.Add(AnimPlayerNames.TakeDamage1, clips[24].name);
        AnimDictionary.Add(AnimPlayerNames.Defence, clips[29].name);
        AnimDictionary.Add(AnimPlayerNames.Blocked, clips[30].name);
        AnimDictionary.Add(AnimPlayerNames.FailDefence, clips[31].name);
        AnimDictionary.Add(AnimPlayerNames.Kick, clips[32].name);
        AnimDictionary.Add(AnimPlayerNames.IdleCombat, clips[43].name);
        AnimDictionary.Add(AnimPlayerNames.WalkW, clips[40].name);
        AnimDictionary.Add(AnimPlayerNames.WalkS, clips[39].name);
        AnimDictionary.Add(AnimPlayerNames.WalkD, clips[42].name);
        AnimDictionary.Add(AnimPlayerNames.WalkA, clips[41].name);
        AnimDictionary.Add(AnimPlayerNames.RunCombat, clips[46].name);
        AnimDictionary.Add(AnimPlayerNames.W_RunCombat, clips[47].name);
        AnimDictionary.Add(AnimPlayerNames.Run, clips[35].name);
        AnimDictionary.Add(AnimPlayerNames.Walk, clips[36].name);
        AnimDictionary.Add(AnimPlayerNames.Idle, clips[37].name);
        AnimDictionary.Add(AnimPlayerNames.Roll, clips[51].name);
        AnimDictionary.Add(AnimPlayerNames.RollAttack, clips[52].name);
        AnimDictionary.Add(AnimPlayerNames.RollEstocada_Damage, clips[53].name);
        AnimDictionary.Add(AnimPlayerNames.Dodge_Back, clips[55].name);
        AnimDictionary.Add(AnimPlayerNames.Dodge_Left, clips[56].name);
        AnimDictionary.Add(AnimPlayerNames.Dodge_Right, clips[57].name);
        AnimDictionary.Add(AnimPlayerNames.Dodge_Back2, clips[58].name);
        AnimDictionary.Add(AnimPlayerNames.Dodge_Left2, clips[59].name);
        AnimDictionary.Add(AnimPlayerNames.Dodge_Right2, clips[60].name);
        AnimDictionary.Add(AnimPlayerNames.WalkA2, clips[45].name);
        AnimDictionary.Add(AnimPlayerNames.WalkD2, clips[44].name);
        AnimDictionary.Add(AnimPlayerNames.WalkS2, clips[46].name);
        AnimDictionary.Add(AnimPlayerNames.WalkW2, clips[47].name);
        AnimDictionary.Add(AnimPlayerNames.IdleCombat_Wizzard, clips[43].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack1_Pre, clips[65].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack1_Damage, clips[63].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack1_End, clips[64].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack2_Damage, clips[66].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack2_End, clips[67].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack2_Pre, clips[68].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack3_Damage, clips[69].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack3_End, clips[70].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack3_Pre, clips[71].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack4_Damage, clips[72].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack4_End, clips[73].name);
        AnimDictionary.Add(AnimPlayerNames.W_Attack4_Pre, clips[74].name);
        AnimDictionary.Add(AnimPlayerNames.CastSpell, clips[18].name);

        shieldIcon.color = new Color(shieldIcon.color.r, shieldIcon.color.g, shieldIcon.color.b, 0.5f);
        whirlIcon.color = new Color(whirlIcon.color.r, whirlIcon.color.g, shieldIcon.color.b, 0.5f);
        foreach (var item in gauntlets)
            gauntletMaterials.Add(item.material);
    }

  
    public void Update()
    {
        if (model.life <=0)
        {
            anim.SetBool("IdleCombat", false);
            anim.SetBool("IsInCombat", false);
            anim.SetBool("Idle", false);
        }

        if (animClipName == AnimDictionary[AnimPlayerNames.Dodge_Back] || animClipName == AnimDictionary[AnimPlayerNames.Dodge_Left] || animClipName == AnimDictionary[AnimPlayerNames.Dodge_Right]
          || animClipName == AnimDictionary[AnimPlayerNames.Dodge_Back2] || animClipName == AnimDictionary[AnimPlayerNames.Dodge_Left2] || animClipName == AnimDictionary[AnimPlayerNames.Dodge_Right2]) dodgeAnims = true;

        else dodgeAnims = false;

        if (animClipName == AnimDictionary[AnimPlayerNames.Walk] || animClipName == AnimDictionary[AnimPlayerNames.WalkA] || animClipName == AnimDictionary[AnimPlayerNames.WalkA2] 
            || animClipName == AnimDictionary[AnimPlayerNames.WalkD] || animClipName == AnimDictionary[AnimPlayerNames.WalkD2] || animClipName == AnimDictionary[AnimPlayerNames.WalkS] 
            || animClipName == AnimDictionary[AnimPlayerNames.WalkS2] || animClipName == AnimDictionary[AnimPlayerNames.WalkW] || animClipName == AnimDictionary[AnimPlayerNames.WalkW2] 
            || animClipName == AnimDictionary[AnimPlayerNames.Run] || animClipName == AnimDictionary[AnimPlayerNames.RunCombat] || animClipName == AnimDictionary[AnimPlayerNames.IdleCombat]
            || animClipName == AnimDictionary[AnimPlayerNames.Idle] || animClipName == AnimDictionary[AnimPlayerNames.IdleCombat_Wizzard]) movementAnims = true;

        else movementAnims = false;

        if (model.classType == Model.PlayerClass.Warrior)
        {
            anim.SetBool("P_Warrior", true);
            anim.SetBool("P_Wizzard", false);
        }

        else
        {
            anim.SetBool("P_Warrior", false);
            anim.SetBool("P_Wizzard", true);
        }

        if (animClipName == AnimDictionary[AnimPlayerNames.TakeDamage1] || animClipName == AnimDictionary[AnimPlayerNames.TakeDamage2] || animClipName == AnimDictionary[AnimPlayerNames.TakeDamage3]) onDamageAnim = true;
        else onDamageAnim = false;

        animClipName = model.animClipName;

         if (animClipName == AnimDictionary[AnimPlayerNames.Attack1_Damage] || animClipName == AnimDictionary[AnimPlayerNames.Attack1_End] || animClipName == AnimDictionary[AnimPlayerNames.Attack1_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.Attack2_Damage] || animClipName == AnimDictionary[AnimPlayerNames.Attack2_End] || animClipName == AnimDictionary[AnimPlayerNames.Attack2_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.Attack3_Damage] || animClipName == AnimDictionary[AnimPlayerNames.Attack3_End] || animClipName == AnimDictionary[AnimPlayerNames.Attack3_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.Attack4_Damage] || animClipName == AnimDictionary[AnimPlayerNames.Attack4_End] || animClipName == AnimDictionary[AnimPlayerNames.Attack4_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.W_Attack1_Damage] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack1_End] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack1_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.W_Attack2_Damage] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack2_End] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack2_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.W_Attack3_Damage] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack3_End] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack3_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.W_Attack4_Damage] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack4_End] || animClipName == AnimDictionary[AnimPlayerNames.W_Attack4_Pre]
           || animClipName == AnimDictionary[AnimPlayerNames.CastSpell] || animClipName == AnimDictionary[AnimPlayerNames.TurnAttack_Damage]
           || animClipName == AnimDictionary[AnimPlayerNames.TurnAttack_End] || animClipName == AnimDictionary[AnimPlayerNames.TurnAttack_Pre]) attacking = true;

          

         else attacking = false;

        if (animClipName == AnimDictionary[AnimPlayerNames.TakeDamage1] || animClipName == AnimDictionary[AnimPlayerNames.TakeDamage2] || animClipName == AnimDictionary[AnimPlayerNames.TakeDamage3]) model.onDamage = true;

        else model.onDamage = false;

        currentAttackAnimation = model.countAnimAttack;

        anim.SetBool("OnDash", model.onDash);

        if (animClipName == AnimDictionary[AnimPlayerNames.IdleCombat] || animClipName == AnimDictionary[AnimPlayerNames.BackAttack2]) currentAttackAnimation = 0;

        if (animClipName == "TakeSword.V3")
        {
            timeToSaveSword += Time.deltaTime;
            if (timeToSaveSword >= 0.1f)
            {
                BackTakeSword2();
                timeToSaveSword = 0;
            }
        }
        else timeToSaveSword =0;

        if (anim.GetBool("TakeSword2") && anim.GetBool("SaveSword2"))
        {
            anim.SetBool("TakeSword2", false);
            anim.SetBool("SaveSword2", false);
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(0, 1);
        }

        if (anim.GetBool("RollAttack")) anim.SetInteger("TakeDamage", 0);

        var velocityX = Input.GetAxis("Vertical");
        var velocityZ = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) velocityZ = 0;
  
        if (velocityX > 1) velocityX = 1;
        if (velocityZ > 1) velocityZ = 1;

        if (velocityX > 0 || velocityZ > 0) anim.SetBool("Idle", false);

        if (velocityX <= 0 && velocityZ <= 0 && model.life>0) anim.SetBool("Idle", true);

        if (model.timeOnCombat>0)
        {
            anim.SetBool("Idle", false);
            if (velocityX == 0 && velocityZ == 0) anim.SetBool("IdleCombat", true);
            else anim.SetBool("IdleCombat", false);
        }

      //  if(model.timeOnCombat <=0 && model.life>0) anim.SetBool("Idle", true);

        anim.SetFloat("VelX", velocityX);
        anim.SetFloat("VelZ", velocityZ);

        if(model.isDead) anim.SetLayerWeight(1, 0);

        if (model.targetLocked)
            lockParticle.SetActive(true);
        else
            lockParticle.SetActive(false);
    }

    IEnumerator CastMagicMissileFalse()
    {
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("CastMagic", false);
    }

    public void CastMagicMissileAnim()
    {
        anim.SetBool("CastMagic", true);
        StartCoroutine(CastMagicMissileFalse());
    }

   

    public void RollAttackAntisipation()
    {
        antisipationRing.Clear();
        antisipationRing.Play();
    }

    public void DodgeLeftAnim()
    {
        anim.SetBool("DodgeLeft", true);
    }

    public void DogeRightAnim()
    {
        anim.SetBool("DodgeRight", true);
    }

    public void DogeBackAnim()
    {
        anim.SetBool("DodgeBack", true);
    }

    public void EndDodge()
    {
        anim.SetBool("DodgeBack", false);
        anim.SetBool("DodgeRight", false);
        anim.SetBool("DodgeLeft", false);
    }

    public void  CanRollAttack()
    {
        anim.SetBool("CanRollAttack", true);
    }

    public void RollAttackAnim()
    {
        anim.SetBool("RollAttack", true);
    }

    public void Sparks()
    {
        sparks.Play();
    }

    public void RollAttackAnimFalse()
    {
        anim.SetBool("RollAttack", false);
        anim.SetBool("CanRollAttack", false);
    }

    public void ParryAnim()
    {
        StartCoroutine(DelayAnimActive("Parry", 0.5f));
    }

    IEnumerator DelayAnimActive(string animName, float t)
    {
        anim.SetBool(animName, true);
        yield return new WaitForSeconds(t);
        anim.SetBool(animName, false);
        currentAttackAnimation = 0;
        model.countAnimAttack = 0;
        anim.SetInteger("AttackAnim", currentAttackAnimation);
    }

    public void ParryAnimFalse()
    {
        anim.SetBool("Parry", false);
    }

    public void Streak()
    {
        anim.SetBool("Streak", true);
        model.onPowerState = true;
    }

    public void StreakFalse()
    {
        anim.SetBool("Streak", false);
        model.onPowerState = false;
    }

    public void Awake()
    {
   
        trail.SetActive(false);
        anim.SetLayerWeight(1, 0);
        cam = FindObjectOfType<CamController>();
        headBaseRot = head.transform.rotation;

        startFade.enabled = true;
        startFade.CrossFadeAlpha(0, fadeTime, false);
    }

    public void SaveSwordAnim2()
    {
        anim.SetLayerWeight(1, 1);
        anim.SetBool("SaveSword2", true);
        anim.SetBool("Defence", false);
    }

    public void BackSaveSword()
    {
        
        StartCoroutine(SaveSwordAnim());       
        anim.SetBool("Idle", true);
        anim.SetBool("SaveSword2", false);
        anim.SetBool("Defence", false);
        anim.SetBool("IdleCombat", false);
    }

    public void TakeSword2()
    {
        anim.SetLayerWeight(1, 1);
        anim.SetBool("TakeSword2", true);
        anim.SetBool("Defence", false);
    }

    public void BackTakeSword2()
    {
        StartCoroutine(TakeSwordAnim());
        anim.SetBool("IdleCombat", true);
        anim.SetBool("Idle", false);
        anim.SetBool("TakeSword2", false);
        anim.SetBool("Defence", false);
    }

    public void RollAnim()
    {
        anim.SetBool("Roll", true);
    }

    public void BackRollAnim()
    {
        anim.SetBool("Roll", false);
        anim.SetBool("CanRollAttack", false);
        model.onRoll = false;
    }

    public void Blocked()
    {
        anim.Play("Blocked");
    }

    public void BlockedFail()
    {
        anim.Play("P_Warrior_FailDefence");
    }

    public void AwakeTrail()
    {
        trail.SetActive(true);
    }

    public void SleepTrail()
    {
        trail.SetActive(false);
    }

    public void RunSword()
    {
        anim.SetBool("runSword", true);
    }

    public void TrotAnim()
    {
        anim.SetBool("trotAnim", true);
    }

    public void FalseTrotAnim()
    {
        anim.SetBool("trotAnim", false);
    }

    public void RunAnim()
    {
        anim.SetBool("runAnim", true);
        anim.SetBool("WalkW", false);
        anim.SetBool("WalkS", false);
        anim.SetBool("WalkD", false);
        anim.SetBool("WalkA", false);
    }

    public void FalseRunAnim()
    {

        anim.SetBool("runAnim", false);
    }

    public void FalseAnimRunSword()
    {
        anim.SetBool("runSword", false);
    }


    public void FalseAnimWalk()
    {
        anim.SetBool("runSword", false);
        anim.SetBool("WalkW", false);
        anim.SetBool("WalkS", false);
        anim.SetBool("WalkD", false);
        anim.SetBool("WalkA", false);

    }

    public void UpdateLifeBar(float val)
    {
        StartCoroutine(UpdateHealth(val));
    }

    public void UpdateManaBar(float val)
    {
        StartCoroutine(BarSmooth(val, manaBar));
    }

    public void ToggleMaxDamage(bool state)
    {
        maxDamageDisplay.SetActive(state);
    }

    public void CounterAttackAnim()
    {
        anim.SetBool("CounterAttack",true);
    }

    public void CounterAttackFalse()
    {
        anim.SetBool("CounterAttack", false);
    }

    public void ToggleHandEffect(bool turnOn)
    {
        if ((gauntletMaterials[0].GetFloat("_BURN") == 1 && turnOn) || (gauntletMaterials[0].GetFloat("_BURN") == -1 && !turnOn))
            return;
        StartCoroutine(HandEffect(turnOn));
    }

    IEnumerator HandEffect(bool turnOn)
    {
        foreach (var item in handEffects)
            item.SetActive(turnOn);

        float t = -1;
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            float fill = turnOn ? t : -t;
            foreach (var item in gauntletMaterials)
                item.SetFloat("_BURN", fill);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator UpdateHealth(float target)
    {
        if (lifeBar.fillAmount > target)
        {
            lifeBar.fillAmount = target;
            yield return new WaitForSeconds(0.2f);
            while (tempLifeBar.fillAmount > target)
            {
                tempLifeBar.fillAmount -= Time.deltaTime / 10;
                yield return new WaitForEndOfFrame();
            }
            tempLifeBar.fillAmount = target;
        }
        else
        {
            while (lifeBar.fillAmount < target)
            {
                lifeBar.fillAmount += Time.deltaTime / 2;
                yield return new WaitForEndOfFrame();
            }
            tempLifeBar.fillAmount = target;
        }
    }

    public IEnumerator BarSmooth(float target, Image barToAffect)
    {
        bool timerRunning = true;
        float smoothTimer = 0;

        float current = barToAffect.fillAmount;

        if (current - target <= 0.025f)
            barToAffect.fillAmount = target;

        while (timerRunning)
        {
            smoothTimer += Time.deltaTime * 1.5f;
            barToAffect.fillAmount = Mathf.Lerp(current, target, smoothTimer);
            if (smoothTimer > 1)
                timerRunning = false;
            yield return new WaitForEndOfFrame();
        }
    }

    public void UpdatePotions(int i)
    {
        //potions[i].fillAmount = (float)model.potions[i] / 3;

    }

    public void UpdateTimer(string val = "")
    {
        potionTimer.text = val.ToString();
    }

    public void TogglePause()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            cam.blockMouse = true;
            model.rb.velocity = Vector3.zero;
            AudioSource[] audios = FindObjectsOfType<AudioSource>();
            foreach (var item in audios) item.UnPause();
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            cam.blockMouse = false;
            model.rb.velocity = Vector3.zero;
            AudioSource[] audios = FindObjectsOfType<AudioSource>();
            foreach (var item in audios) item.Pause();
        }
    }

    public void LookAtEnemy()
    {
        if (model.closestEnemy != null)
            head.LookAt(model.closestEnemy);
        else
            head.rotation = headBaseRot;
    }

    public IEnumerator SlowSpeed()
    {
        anim.speed = 0f;
        yield return new WaitForSeconds(0.025f);
        anim.speed = 1;
    }

    public void UpdatePowerCD(int id, float fa)
    {
        switch (id)
        {
            case 1:
                power1.fillAmount = fa;
                if (fa >= 1)
                    StartCoroutine(PowerEffect(id, true));
                break;
            case 2:
                power2.fillAmount = fa;
                if (fa >= 1)
                    StartCoroutine(PowerEffect(id, true));
                break;
            case 3:
                power3.fillAmount = (fa / 100 * 18) + 0.34f;
                break;
            case 4:
                power4.fillAmount = (fa / 100 * 18) + 0.34f;
                break;
        }
    }

    public IEnumerator PowerEffect(int id, bool activate)
    {
        float t = 0;

        Color shieldColor = shieldIcon.color;
        Color whirlColor = whirlIcon.color;

        if (activate)
        {
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                if (id == 1)
                    shieldIcon.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, t);
                else
                    whirlIcon.color = new Color(whirlColor.r, whirlColor.g, whirlColor.b, t);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            t = 0.5f;
            while (t > 0)
            {
                t -= Time.deltaTime;
                if (id == 1)
                    shieldIcon.color = new Color(shieldColor.r, shieldColor.g, shieldColor.b, t);
                else
                    whirlIcon.color = new Color(whirlColor.r, whirlColor.g, whirlColor.b, t);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void DesactivateLayer()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetBool("SaveSword", false);
        anim.SetBool("TakeSword", false);
    }

    public void BrokenDefence(float time)
    {
        defenceColdwon.gameObject.SetActive(true);
        defenceColdwon.fillAmount = -time + 1;
    }

    public void Defence()
    {
        anim.SetBool("Defence", true);
        //defenceActive.gameObject.SetActive(true);
    }

    public void NoDefence()
    {
        anim.SetBool("Defence", false);
        //defenceActive.gameObject.SetActive(false);
    }

    public void ReciveDamage()
    {

        blood.Play();
        if (!model.onPowerState && model.life>0)
        {
            StartCoroutine(DamageDelay());
            ShakeCameraDamage(1, 1.5f, 0.5f);
            var random = Random.Range(1, 4);
            anim.SetInteger("TakeDamage", random);
        }
    }

    public void ShakeCameraDamage(float frequency, float amplitude, float time )
    {
        cam.CamShake(frequency, amplitude, time);
    }

    public void NoReciveDamage()
    {
        anim.SetInteger("TakeDamage", 0);
    }

    public void BasicAttack()
    {
        cam.AttackCameraEffect();

        if (currentAttackAnimation == 3)
        {
            preAttacktime = 0.001f;
            StartCoroutine(SmashParticleEvent());
        }
        currentAttackAnimation++;
        anim.SetInteger("AttackAnim", currentAttackAnimation);
    }
 


    public void Dead()
    {
        anim.SetBool("IsDead", true);
    }

    public IEnumerator YouDied()
    {
        cam.cinemaCam.Follow = null;
        yield return new WaitForSeconds(1.5f);
        GameObject orb = Instantiate(orbPrefab);
        orb.transform.position = orbPH.position + (Vector3.down * 0.25f);
        yield return new WaitForEndOfFrame();
        model.rb.isKinematic = true;

        var t = 0f;
        while (t < 4)
        {
            t += Time.deltaTime;
            orb.transform.position += Vector3.up * Time.deltaTime * orbSpeed;

            if (t > 3)
            {
                startFade.enabled = true;
                startFade.CrossFadeAlpha(1, 0.5f, false);
            }
            yield return new WaitForEndOfFrame();
        }

        cam.cinemaCam.LookAt = transform;
        cam.cinemaCam.Follow = transform;
        Destroy(orb.gameObject);
        bm.RespawnScene();
       
    }

    public IEnumerator YouWin()
    {
        youWin.gameObject.SetActive(true);
        var tempColor = youWin.GetComponent<Image>().color;
        var alpha = 0f;
        while (alpha <= 1)
        {
            alpha += 0.75f * Time.deltaTime;
            if (alpha > 1) alpha = 1;
            tempColor.a = alpha;
            youWin.GetComponent<Image>().color = tempColor;
            if (alpha >= 1)
            {
                cam.blockMouse = false;
                for (int i = 0; i < youWin.transform.childCount; i++)
                    youWin.transform.GetChild(i).gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator NextLevel()
    {
        startFade.enabled = true;
        startFade.CrossFadeAlpha(1, 1, false);
        yield return new WaitForSeconds(1);
        LoadingScreen.instance.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
