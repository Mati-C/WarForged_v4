using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewer_Player : MonoBehaviour
{
    public Animator anim;
    Model_Player _player;
    FireSword _fireSword;
    public ParticleSystem bloodParticle;
    public ParticleSystem sparksParticle;
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
    public float timeLayerUp;
    bool _lockParticleUp;
    bool _changeSwordBoneParent;
    bool _slowedTime;
    bool _boneMove;
    Quaternion _swordBackSaveRotation;

    [Header("Player AxisValues:")]

    public float axisX;

    IEnumerator DamageTimerAnim()
    {
        while (true)
        {            

            if (_player.onDamageTime <= 0)
            {
                anim.SetInteger("GetHit", 0);
                anim.SetInteger("GetHitHeavy", 0);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DelayAnimationActivate(string animName, bool r, float time)
    {
        anim.SetBool(animName, r);
        yield return new WaitForSeconds(time);       
        anim.SetBool(animName, !r);
    }

    public IEnumerator DelayActivateLayers()
    {
        while(true)
        {
            timeLayerUp -= Time.deltaTime;

            if(timeLayerUp >0)
            {
                anim.SetLayerWeight(0, 1);
                anim.SetLayerWeight(1, 1);
                layerUpActive = true;
            }

            else
            {
                anim.SetLayerWeight(0, 1);
                anim.SetLayerWeight(1, 0);
                layerUpActive = false;
            }

            yield return new WaitForEndOfFrame();
        }
        

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
        _fireSword = FindObjectOfType<FireSword>();
        powerBar.fillAmount = 0;
        DesactivateSword();
    }

    void Start()
    {
        StartCoroutine(DelayActivateLayers());
        StartCoroutine(DamageTimerAnim());
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

        if (anim.GetBool("Roll") || anim.GetBool("DodgeRight") || anim.GetBool("DodgeLeft") || anim.GetBool("DodgeBack")) anim.SetBool("Dodge", true);

        if (!anim.GetBool("Roll") && !anim.GetBool("DodgeRight") && !anim.GetBool("DodgeLeft") && !anim.GetBool("DodgeBack")) anim.SetBool("Dodge", false);

        if (anim.GetInteger("AttackCombo") > 0 || anim.GetFloat("ChargeAttack") >= 0.2f || anim.GetBool("FailAttack")) anim.SetBool("OnAttack", true);

        if (anim.GetInteger("AttackCombo") <= 0 && anim.GetFloat("ChargeAttack") < 0.2f && !anim.GetBool("FailAttack")) anim.SetBool("OnAttack", false);

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
        StartCoroutine(DelayAnimationActivate("FireSword", true, 1));
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
        while(_player.fireSwordCurrentTime < _fireSword.fireSwordTime)
        {
            powerBar.fillAmount -= Time.deltaTime / _fireSword.fireSwordTime;
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

    public void FailAttackAnim()
    {
        StartCoroutine(DelayAnimationActivate("FailAttack", true, 1f));
        sparksParticle.Play();
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);

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
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Blocked", true);
        sparksParticle.Play();
        yield return new WaitForSeconds(0.4f);
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
        timeLayerUp = 0.8f;
        StartCoroutine(TakeSword());
        StartCoroutine(DelayAnimationActivate("TakeSword", true, 0.6f));
    }

    IEnumerator TakeSword()
    {
        yield return new WaitForSeconds(0.42f);
        ActivateSword();
    }

    public void SaveSwordAnim()
    {
        timeLayerUp = 0.8f;
        StartCoroutine(SaveSword());
        StartCoroutine(DelayAnimationActivate("SaveSword", true, 0.6f));
    }

    IEnumerator SaveSword()
    {
        yield return new WaitForSeconds(0.42f);
        DesactivateSword();
    }

  
    public void CombatStateAnimator(bool r)
    {
        anim.SetBool("CombatState", r);
    }

    public void DefenceAnim(bool d)
    {
        anim.SetBool("Defence", d);
    }

    public void AnimGetHit()
    {
        if (_player.chargeAttackAmount <0.2f)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Run", false);
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

    public void AnimGetHitHeavy(bool d)
    {        
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        bloodParticle.Play();

        switch (d)
        {
            case true:
                anim.SetInteger("GetHitHeavy", 1);
                break;

            case false:
                anim.SetInteger("GetHitHeavy", 2);
                break;
        }

    }

    public void SwordPowerAnim()
    {

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
