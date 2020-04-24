using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewer_Player : MonoBehaviour
{
    public Animator anim;
    Model_Player _player;
    Camera _mainCam;

    [Header("Lock Particle:")]

    public Image lockImage;

    [Header("Player Bones:")]

    public Transform swordBone;
    public Transform swordHandBone;
    public Transform swordHandPositionPh;
    public Transform swordBackBone;
    public Transform swordBackPositionPh;

    [Header("Player Swords:")]

    public GameObject swordHand;
    public GameObject swordBack;

    [Header("Layer Up Active:")]
    public bool layerUpActive;

    bool _lockParticleUp;
    bool _changeSwordBoneParent;
    Quaternion _swordBackSaveRotation;


    IEnumerator DelayAnimationActivate(string animName, bool r, float time)
    {
        anim.SetBool(animName, r);
        yield return new WaitForSeconds(time);       
        anim.SetBool(animName, !r);
    }

    IEnumerator DelayActivateLayers(float time)
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

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _player = GetComponent<Model_Player>();
        DesactivateSword();
    }

    void Start()
    {
        _mainCam = _player.GetPlayerCam().GetComponent<Camera>();
    }

    
    void Update()
    {

        anim.SetInteger("AttackCombo", _player.attackCombo);

        var velocityX = Input.GetAxis("Vertical");
        var velocityZ = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) velocityZ = 0;

        if (velocityX > 1) velocityX = 1;
        if (velocityZ > 1) velocityZ = 1;
        if (velocityZ < -1) velocityZ = -1;
        if (velocityX < -1) velocityX = -1;
    
        anim.SetFloat("VelX", velocityX);
        anim.SetFloat("VelZ", velocityZ);

    }

    IEnumerator DelayPositionSword(bool handOrBack)
    {
        var timer = 0.3f;

        while(timer >0 && !handOrBack)
        {
            timer -= Time.deltaTime;
            swordBone.position = swordHandPositionPh.position;
            swordBone.rotation = swordHandPositionPh.rotation;
            yield return new WaitForEndOfFrame();
        }

        while (timer > 0 && handOrBack)
        {
            timer -= Time.deltaTime;
            swordBone.position = swordBackPositionPh.position;
            swordBone.rotation = swordBackPositionPh.rotation;
            yield return new WaitForEndOfFrame();
        }
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
        if (dir == Model_Player.DogeDirecctions.Roll) StartCoroutine(DelayAnimationActivate("Roll", true, 0.3f)); 

        if (dir == Model_Player.DogeDirecctions.Back) StartCoroutine(DelayAnimationActivate("DodgeBack", true, 0.3f)); 

        if (dir == Model_Player.DogeDirecctions.Left) StartCoroutine(DelayAnimationActivate("DodgeLeft", true, 0.3f)); 

        if (dir == Model_Player.DogeDirecctions.Right) StartCoroutine(DelayAnimationActivate("DodgeRight", true, 0.3f)); 
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

}
