using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sound;

public class Viewer_Player : MonoBehaviour
{
    [Header("Player Animator:")]
    public Animator anim;
    Model_Player _player;
    FireSword _fireSword;

    [Header("Player Particles:")]
    public ParticleSystem bloodParticle;
    public ParticleSystem sparksParticle;
    public ParticleSystem woodParticle;
    public ParticleSystem burlapParticle;
    Camera _mainCam;

    [Header("Player Sprites:")]

    public Image lockImage;
    public Image powerBar;
    public Image chargeAttackBar;
    public Image lifeBar;
    public Image tempLifeBar;
    public Text swordLevel;
    public Text swordExp;
    public float timertAlphaSwordExp;

    [Header("Player Bones:")]

    public Transform spineBone;
    public Transform headBone;
    public Transform defenceFroward;
    public Transform headFroward;

    [Header("Player Swords:")]

    public GameObject swordHand;
    public GameObject swordBack;
    public ParticleSystem swordFire;
    public GameObject normalTrail;
    public GameObject fireTrail;

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

    IEnumerator PlayerAlphaExp()
    {
        while(true)
        {
            if(timertAlphaSwordExp>0)
            {
                var tempColor = swordExp.color;
                tempColor.a = 1;
                swordExp.color = tempColor;
                timertAlphaSwordExp -= Time.deltaTime;
            }

            if(timertAlphaSwordExp <=0)
            {
                var tempColor = swordExp.color;
                tempColor.a -= Time.deltaTime * 3;
                if (tempColor.a < 0) tempColor.a = 0;
                swordExp.color = tempColor;
            }
            yield return new WaitForEndOfFrame();
        }
    }

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
        powerBar = GameObject.Find("PowerBar").GetComponent<Image>();
        chargeAttackBar = GameObject.Find("Stamina").GetComponent<Image>();
        lifeBar = GameObject.Find("LifeBar").GetComponent<Image>();
        tempLifeBar = GameObject.Find("TempLifeBar").GetComponent<Image>();       
        normalTrail = GameObject.Find("Vfx_SwordTrail");       
        fireTrail.gameObject.SetActive(false);
        swordFire.Stop();
        powerBar.fillAmount = 0;
        DesactivateSword();
        lifeBar.material.SetFloat("_InnerGlow", 1);
        lifeBar.material.SetFloat("_BeatRate", 0);
        lifeBar.material.SetFloat("_BeatColorIntensity", 0);
        powerBar.material.SetFloat("_InsideGlowOpacity", 0);
    }

    void Start()
    {
        swordLevel = GameObject.Find("Sword Level").GetComponent<Text>();
        swordExp = GameObject.Find("Sword Exp").GetComponent<Text>();

        var tempColor = swordExp.color;
        tempColor.a = 0;
        swordExp.color = tempColor;

        int l = _player.fireSword.fireSwordLevel + 1;
        swordLevel.text = "Level-" + l;
        StartCoroutine(DelayActivateLayers());
        StartCoroutine(DamageTimerAnim());
        _mainCam = _player.GetPlayerCam().GetComponent<Camera>();
        StartCoroutine(PlayerAlphaExp());
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

        if(!_player.chargeAttackCasted) anim.SetFloat("ChargeAttack", _player.chargeAttackAmount);

        else anim.SetFloat("ChargeAttack", 0);

        if (anim.GetBool("Roll") || anim.GetBool("DodgeRight") || anim.GetBool("DodgeLeft") || anim.GetBool("DodgeBack")) anim.SetBool("Dodge", true);

        if (!anim.GetBool("Roll") && !anim.GetBool("DodgeRight") && !anim.GetBool("DodgeLeft") && !anim.GetBool("DodgeBack")) anim.SetBool("Dodge", false);

        if (anim.GetInteger("AttackCombo") > 0 || anim.GetFloat("ChargeAttack") >= 0.2f || anim.GetBool("FailAttack")) anim.SetBool("OnAttack", true);

        if (anim.GetInteger("AttackCombo") <= 0 && anim.GetFloat("ChargeAttack") < 0.2f && !anim.GetBool("FailAttack")) anim.SetBool("OnAttack", false);

        normalTrail.SetActive(_player.onAttackAnimation && !_player.flamesOn);

        fireTrail.SetActive(_player.onAttackAnimation && _player.flamesOn);

        if (_player.timeOnCombat < _player.maxTimeOnCombat * 0.5f)
            SoundManager.instance.CombatMusic(false);
    }


    public void LateUpdate()
    {
        if (anim.GetBool("Defence") && !_boneMove && !anim.GetBool("Blocked") && !anim.GetBool("Kick"))
        {            
            headBone.transform.forward = headFroward.forward;
            spineBone.transform.forward = defenceFroward.forward;
        }
    }

    public void UpdateLife(float target)
    {
        if (target == 1)
            lifeBar.material.SetFloat("_InnerGlow", 1);
        else
        {
            lifeBar.material.SetFloat("_InnerGlow", 0);
            if (target <= 0.3f)
            {
                float val = 1 - (target / 3) * 10;
                lifeBar.material.SetFloat("_BeatRate", Mathf.Lerp(3, 10, val));
                lifeBar.material.SetFloat("_BeatColorIntensity", Mathf.Lerp(0.2f, 1f, val));
            }
            else
            {
                lifeBar.material.SetFloat("_BeatRate", 0);
                lifeBar.material.SetFloat("_BeatColorIntensity", 0);
            }
        }

        StartCoroutine(UpdateLifeBar(target));
    }

    public IEnumerator UpdateLifeBar(float target)
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

    public void PowerSwordActivated()
    {
        SoundManager.instance.Play(Player.FIRE_SWORD, transform.position, false, 0.5f);
        swordFire.Play();
        StartCoroutine(DelayAnimationActivate("FireSword", true, 1));
        StartCoroutine(DecreesPowerBar());
        normalTrail.SetActive(false);
    }

    public void PowerSwordDesactivated()
    {
        swordFire.Stop();
        normalTrail.SetActive(true);
    }

    public void ChargeAttackAnim()
    {
        SoundManager.instance.PlayRandom(SoundManager.instance.swing, transform.position, true);
        SoundManager.instance.Play(Player.SHORT_YELL, transform.position, true);
        StartCoroutine(DelayAnimationActivate("ChargeAttackB", true, 0.2f));
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

    public void OnHit(float val)
    {        
        StartCoroutine(FillPowerBar(_player.fireEnergy / _fireSword.energyToUseFireSword));
    }
    
    IEnumerator FillPowerBar(float target)
    {
        while (powerBar.fillAmount < target && powerBar.fillAmount < 1)
        {
            powerBar.fillAmount += Time.deltaTime / 10;
            yield return new WaitForEndOfFrame();
        }
        powerBar.fillAmount = target;

        if (powerBar.fillAmount >= 1)
        {
            powerBar.fillAmount = 1;
            powerBar.material.SetFloat("_InsideGlowOpacity", 1);
        }
    }

    IEnumerator DecreesPowerBar()
    {
        while(_player.fireSwordCurrentTime < _fireSword.fireSwordTime)
        {
            powerBar.fillAmount -= Time.deltaTime / _fireSword.fireSwordTime;
            yield return new WaitForEndOfFrame();
        }
        powerBar.fillAmount = 0;
        powerBar.material.SetFloat("_InsideGlowOpacity", 0);
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

    public void FailAttackAnim(string objName)
    {
        StartCoroutine(DelayAnimationActivate("FailAttack", true, 1f));

        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);

        if (objName.StartsWith("Grain"))
        {
            burlapParticle.Play();
            SoundManager.instance.Play(Hit.SOFT, transform.position + transform.forward.normalized, true);
        }
        else if (objName.StartsWith("Crate"))
        {
            woodParticle.Play();
            SoundManager.instance.Play(Hit.WOOD, transform.position + transform.forward.normalized, true);
        }
        else
        {
            sparksParticle.Play();
            SoundManager.instance.Play(Hit.HARD, transform.position + transform.forward.normalized, true);
        }
    }

    public void BlockAnim(bool b)
    {
        if (b == false)
        {
            if (!anim.GetBool("Blocked"))
            {
                StartCoroutine(DelayAnimationActivate("Blocked", true, 0.3f));
                SoundManager.instance.Play(Hit.HARD, transform.position + transform.forward.normalized, true);
            }
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
        SoundManager.instance.Play(Hit.HARD, transform.position, true);
        sparksParticle.Play();
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("Blocked", false);
        anim.SetBool("Kick", true);
        SoundManager.instance.Play(Hit.SOFT, transform.position, true, 0.4f);
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
        SoundManager.instance.Play(Player.TAKE_SWORD, transform.position, true);
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
        SoundManager.instance.Play(Player.SAVE_SWORD, transform.position, true);
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

        SoundManager.instance.PlayRandom(SoundManager.instance.damageVoice, transform.position, false, 1, 0.7f);
        SoundManager.instance.Play(Hit.SOFT, transform.position, true);
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
        SoundManager.instance.PlayRandom(SoundManager.instance.damageVoice, transform.position, false);
        SoundManager.instance.Play(Hit.SOFT, transform.position, true);
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
