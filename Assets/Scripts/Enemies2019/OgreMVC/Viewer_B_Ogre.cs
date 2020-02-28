using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewer_B_Ogre : MonoBehaviour
{
    
    public Animator anim;
    public Image healthBar;
    Material healthBarMat;
    Model_B_Ogre _model;
    public GameObject lockParticle;
    public Transform lockParticlePosition;
    public Camera cam;
    public bool auxAwake;
    public ParticleSystem heavyParticles;
    public ParticleSystem bloodParticles;
    GameObject levelUI; 
    public PopText prefabTextDamage;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _model = GetComponent<Model_B_Ogre>();
        healthBarMat = healthBar.material;
        levelUI = GameObject.Find("LEVEL UI");
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

    public void AwakeAnimSum()
    {
        anim.SetBool("Awake", true);
    }

    public void ActiveBloodParticles()
    {
        bloodParticles.Play();
    }

    public void HeavyParticlesActive()
    {
        heavyParticles.Play();
    }

    public void HeavyParticlesActiveFalse()
    {
        heavyParticles.Stop();
        heavyParticles.Simulate(0, false, true, true);
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

    public void CreatePopText(float damage)
    {
        bloodParticles.Play();
        PopText text = Instantiate(prefabTextDamage);
        StartCoroutine(FollowEnemy(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetDamage(damage);
    }

    IEnumerator FollowEnemy(PopText text)
    {
        while (text != null)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(transform.position + (Vector3.up * 2));
            text.transform.position = screenPos;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Attack1Anim()
    {
        int r = Random.Range(0, 3);

        if (r == 0) SoundManager.instance.Play(Sound.EntitySound.MONSTER_ATTACK1, transform.position, true, 1);

        if (r == 1) SoundManager.instance.Play(Sound.EntitySound.MONSTER_ATTACK2, transform.position, true, 1);

        if (r == 2) SoundManager.instance.Play(Sound.EntitySound.MONSTER_ATTACK3, transform.position, true, 1);

        anim.SetBool("Attack1", true);
        anim.SetBool("Move", false);
    }

    public void Attack2Anim()
    {
        int r = Random.Range(0, 3);

        if (r == 0) SoundManager.instance.Play(Sound.EntitySound.MONSTER_ATTACK1, transform.position, true, 1);

        if (r == 1) SoundManager.instance.Play(Sound.EntitySound.MONSTER_ATTACK2, transform.position, true, 1);

        if (r == 2) SoundManager.instance.Play(Sound.EntitySound.MONSTER_ATTACK3, transform.position, true, 1);

        anim.SetBool("Attack2", true);
        anim.SetBool("Move", false);
    }


    public void Attack1AnimFalse()
    {
        anim.SetBool("Attack1", false);
        _model.AnimationAttackFalse();
    }


    public void Attack2AnimFalse()
    {
        anim.SetBool("Attack2", false);
        _model.AnimationAttackFalse();
    }

    public void DieAnim()
    {
        anim.SetBool("Die", true);
        healthBar.gameObject.SetActive(false);
    }
}