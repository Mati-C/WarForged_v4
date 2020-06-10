﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewer_Player : MonoBehaviour
{
    public Animator anim;
    Model_Player _player;
    Camera _mainCam;

    [Header("Player Sprites:")]

    public Image lockImage;
    public Image powerBar;
    public Image chargeAttackBar;

    [Header("Player Bones:")]

    public Transform spineBone;
    public Transform headBone;
    public Transform defenceFroward;
    public Transform headFroward;

    [Header("Player Swords:")]

    public GameObject swordHand;
    public GameObject swordBack;
    public ParticleSystem swordFire;

    [Header("Layer Up Active:")]
    public bool layerUpActive;

    bool _lockParticleUp;
    bool _changeSwordBoneParent;
    bool _slowedTime;
    bool _boneMove;
    Quaternion _swordBackSaveRotation;

    [Header("Player AxisValues:")]

    public float axisX;

    IEnumerator DelayAnimationActivate(string animName, bool r, float time)
    {
        anim.SetBool(animName, r);
        yield return new WaitForSeconds(time);       
        anim.SetBool(animName, !r);
    }

    public IEnumerator DelayActivateLayers(float time)
    {

        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 1);
        layerUpActive = true;
        yield return new WaitForSeconds(time);
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
        layerUpActive = false;

    }

    IEnumerator LockOnParticlePosition()
    {
        while(_lockParticleUp)
        {
            Vector2 screenPos = _mainCam.WorldToScreenPoint(_player.targetEnemy.transform.position + Vector3.up);
            lockImage.transform.position = screenPos;
            yield return new WaitForEndOfFrame();
        }
      
    }

    IEnumerator DelayBone()
    {
        _boneMove = true;
        yield return new WaitForSeconds(0.5f);
        _boneMove = false;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _player = GetComponent<Model_Player>();
        powerBar.fillAmount = 0;
        DesactivateSword();
    }

    void Start()
    {
        _mainCam = _player.GetPlayerCam().GetComponent<Camera>();
    }

    public void ChangeLayer(bool b)
    {
        if(b)
        {
            anim.SetLayerWeight(0, 0);
            anim.SetLayerWeight(1, 1);
            layerUpActive = true;
        }
        else
        {
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 0);
            layerUpActive = false;
        }
    }
 
    void Update()
    {

        anim.SetInteger("AttackCombo", _player.attackCombo);

        var velocityX = Input.GetAxis("Vertical");
        var velocityZ = Input.GetAxis("Horizontal");
        axisX += Input.GetAxis("Mouse X Defence");

        if (axisX > 0.1) axisX -= Time.deltaTime * 2;

        if (axisX < -0.1) axisX += Time.deltaTime * 2;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) velocityZ = 0;

        if (velocityX > 1) velocityX = 1;
        if (velocityZ > 1) velocityZ = 1;
        if (velocityZ < -1) velocityZ = -1;
        if (velocityX < -1) velocityX = -1;
        if (axisX > 1) axisX = 1;
        if (axisX < -1) axisX = -1;

        anim.SetFloat("VelX", velocityX);
        anim.SetFloat("VelZ", velocityZ);
        anim.SetFloat("Mouse X", axisX);

        anim.SetFloat("ChargeAttack", _player.chargeAttackAmount);
       
    }


    public void LateUpdate()
    {
        if (anim.GetBool("Defence") && !_boneMove && !anim.GetBool("Blocked") && !anim.GetBool("Kick"))
        {            
            headBone.transform.forward = headFroward.forward;
            spineBone.transform.forward = defenceFroward.forward;
        }
    }

    public void PowerSwordActivated()
    {
        swordFire.Play();
        StartCoroutine(DecreesPowerBar());
    }

    public void PowerSwordDesactivated()
    {
        swordFire.Stop();
    }

    public void ChargeAttackAnim()
    {        
        StartCoroutine(DecreesChargeAttackBar());
    }

    IEnumerator DecreesChargeAttackBar()
    {
        chargeAttackBar.fillAmount = 0;
        while (_player.chargeAttackColdown < _player.chargeAttackColdownMax)
        {
            chargeAttackBar.fillAmount += Time.deltaTime / _player.chargeAttackColdownMax;
            yield return new WaitForEndOfFrame();
        }
        if (chargeAttackBar.fillAmount > 1) chargeAttackBar.fillAmount = 1;
    }

    public void OnHit(float target)
    {
        powerBar.fillAmount += target;
        if(powerBar.fillAmount > 1) powerBar.fillAmount = 1;
    }

    IEnumerator DecreesPowerBar()
    {
        while(_player.powerCurrentTime < _player.powerCurrentMaxTime)
        {
            powerBar.fillAmount -= Time.deltaTime / _player.powerCurrentMaxTime;
            yield return new WaitForEndOfFrame();
        }
        powerBar.fillAmount = 0;
    }

    public void ActivateSword()
    {
        swordBack.SetActive(false);
        swordHand.SetActive(true);

    }

    public void DesactivateSword()
    {
        swordBack.SetActive(true);
        swordHand.SetActive(false);
    }

    public void BlockAnim(bool b)
    {
        if (b == false)
        {
            if (!anim.GetBool("Blocked")) StartCoroutine(DelayAnimationActivate("Blocked", true, 0.3f));
        }
        else
        {
            if (!anim.GetBool("Blocked") && !anim.GetBool("Kick")) StartCoroutine(KickAnimnCorrutine());
        }
    }

    IEnumerator KickAnimnCorrutine()
    {
        anim.SetBool("Blocked", true);
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("Blocked", false);
        anim.SetBool("Kick", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Kick", false);
    }

    public void WalkAnim()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
    }

    public void IdleAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
    }

    public void RunAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", true);
    }

    public void TakeSwordAnim()
    {
        StartCoroutine(DelayActivateLayers(0.8f));
        StartCoroutine(DelayAnimationActivate("TakeSword", true, 0.6f));
    }

    public void SaveSwordAnim()
    {
        StartCoroutine(DelayActivateLayers(0.8f));
        StartCoroutine(DelayAnimationActivate("SaveSword", true, 0.6f));
    }

    public void CombatStateAnimator(bool r)
    {
        anim.SetBool("CombatState", r);
    }

    public void DefenceAnim(bool d)
    {
        anim.SetBool("Defence", d);
    }

    public void DodgeAnims(Model_Player.DogeDirecctions dir)
    {
        StartCoroutine(DelayBone());

        if (dir == Model_Player.DogeDirecctions.Roll) StartCoroutine(DelayAnimationActivate("Roll", true, 0.3f)); 

        if (dir == Model_Player.DogeDirecctions.Back) StartCoroutine(DelayAnimationActivate("DodgeBack", true, 0.3f)); 

        if (dir == Model_Player.DogeDirecctions.Left) StartCoroutine(DelayAnimationActivate("DodgeLeft", true, 0.5f)); 

        if (dir == Model_Player.DogeDirecctions.Right) StartCoroutine(DelayAnimationActivate("DodgeRight", true, 0.5f)); 
    }

    public void SetLockOnParticle()
    {
        _lockParticleUp = true;
        lockImage.gameObject.SetActive(true);
        StartCoroutine(LockOnParticlePosition());
    }

    public void SetOffLockOnParticle()
    {
        _lockParticleUp = false;
        lockImage.gameObject.SetActive(false);
    }

    public void SlowTinme()
    {
       if(!_slowedTime)StartCoroutine(SlowTimeCorrutine());
    }

    IEnumerator SlowTimeCorrutine()
    {
        Time.timeScale = 0.1f;
        _slowedTime = true;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1;
        _slowedTime = false;
    }
}
