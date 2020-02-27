using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewB_Cyclops : MonoBehaviour
{
    public Animator anim;
    public Image healthBar;
    Material healthBarMat;
    ModelB_Cyclops _model;
    public GameObject lockParticle;
    public Transform lockParticlePosition;
    public Camera cam;
    public bool auxAwake;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _model = GetComponent<ModelB_Cyclops>();
        healthBarMat = healthBar.material;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBarMat.SetFloat("_BossLifePercentage", _model.life / _model.totalLife * 200);
        healthBarMat.SetFloat("_ArrowBeatRatePercentage", _model.life / _model.totalLife * 100);

        if (_model.target.targetLocked && !_model.isDead && _model.target.targetLocked.name == transform.name)
            lockParticle.transform.position = new Vector2(cam.WorldToScreenPoint(lockParticlePosition.position).x, cam.WorldToScreenPoint(lockParticlePosition.position).y);
    }

    public void AwakeAnim()
    {
        anim.SetBool("Awake", true);

        var dir = Vector3.zero;

        dir = (_model.target.transform.position - transform.position).normalized;

        dir.y = 0;

        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        if (!auxAwake)
        {
            SoundManager.instance.Play(Sound.EntitySound.ROAR, transform.position, true, 1);
            auxAwake = true;
        }
        SoundManager.instance.BossMusic(true);
    }

    public void AwakeAnimFalse()
    {
        anim.SetBool("Awake", false);
        _model.AwakeStateActivate();
        healthBar.gameObject.SetActive(true);
    }

    public void IdleAnimation()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("Move", false);
    }

    public void MoveAnimation()
    {
        anim.SetBool("Move", true);
        anim.SetBool("Idle", false);
        anim.SetBool("Awake", false);
    }

    public void Attack1OnPlaceAnimation()
    {
        anim.SetBool("Attack1OnPlace", true);
        anim.SetBool("Move", false);
    }

    public void Attack2OnPlaceAnimation()
    {
        anim.SetBool("Smash", true);
        anim.SetBool("Move", false);
    }

    public void Attack3Combo()
    {
        anim.SetBool("Combo", true);
        anim.SetBool("Move", false);
    }

    public void Attack1OnPlaceAnimationFalse()
    {
        anim.SetBool("Attack1OnPlace", false);
        _model.AnimationAttackFalse();
    }


    public void Attack2OnPlaceAnimationFalse()
    {
        anim.SetBool("Smash", false);
        _model.AnimationAttackFalse();
    }

    public void Attack3ComboFalse()
    {
        anim.SetBool("Combo", false);
        _model.AnimationAttackFalse();
    }

    public void DieAnim()
    {
        anim.SetBool("Die", true);
        healthBar.gameObject.SetActive(false);
    }
}
