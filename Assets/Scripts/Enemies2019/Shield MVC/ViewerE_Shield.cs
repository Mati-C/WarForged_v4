using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ViewerE_Shield : MonoBehaviour
{
    public Animator anim;
    ModelE_Shield _model;
    public List<SkinnedMeshRenderer> myMeshes = new List<SkinnedMeshRenderer>();
    public List<Material> myMats = new List<Material>();
    string animClipName;
    EnemyScreenSpace ess;
    GameObject levelUI;
    public PopText prefabTextDamage;
    public Camera cam;
    public ParticleSystem sparks;
    public ParticleSystem blood;
    public float timeShaderDamage;
    public bool damaged;
    public GameObject lockParticle;
    public Transform positionLockOn;
    public Transform lockParticlePosition;
    Model _player;
    public GameObject ragdollPrefab;
    public ParticleSystem heavyHitParticle;

    public enum EnemyMeleeAnim
    {
        TakeDamage, Dead, Attack1, WalkL, WalkR, WalkForward, IdleCombat, Patrol, Retreat, Stuned, Knocked, AttackBlocked, Blocked, CounterAttack, IdleDefence, Idle,
        Chat1, Chat2, Chat3, Point, RunAttack, Charge
    };

    public Dictionary<EnemyMeleeAnim, string> animDictionary = new Dictionary<EnemyMeleeAnim, string>();

    private void Awake()
    {
        _player = FindObjectOfType<Model>();
        anim = GetComponent<Animator>();
        var clips = anim.runtimeAnimatorController.animationClips.ToList();
        ess = GetComponent<EnemyScreenSpace>();
        levelUI = GameObject.Find("LEVEL UI");
        myMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());

        foreach (var item in myMeshes)
        {
            myMats.Add(item.materials[0]);
            item.materials[0].SetFloat("_Intensity", 0);
        }

        //Iterate over the clips and gather their information
        /* int aux = 0;
         foreach (var animClip in clips)
         {
             Debug.Log(animClip.name + ": " + aux++);
         }
         */
    
          animDictionary.Add(EnemyMeleeAnim.WalkL, clips[5].name);
          animDictionary.Add(EnemyMeleeAnim.WalkR, clips[8].name);
          animDictionary.Add(EnemyMeleeAnim.IdleCombat, clips[10].name);
          animDictionary.Add(EnemyMeleeAnim.Retreat, clips[9].name);
          animDictionary.Add(EnemyMeleeAnim.RunAttack, clips[6].name);
          animDictionary.Add(EnemyMeleeAnim.Attack1, clips[0].name);
          animDictionary.Add(EnemyMeleeAnim.Chat1, clips[13].name);
          animDictionary.Add(EnemyMeleeAnim.Chat2, clips[12].name);
          animDictionary.Add(EnemyMeleeAnim.Chat3, clips[11].name);
          animDictionary.Add(EnemyMeleeAnim.Blocked, clips[2].name);
          animDictionary.Add(EnemyMeleeAnim.WalkForward, clips[7].name);
          animDictionary.Add(EnemyMeleeAnim.TakeDamage, clips[1].name);
          animDictionary.Add(EnemyMeleeAnim.Knocked, clips[14].name);
          animDictionary.Add(EnemyMeleeAnim.Charge, clips[3].name);
         
    }

    void Start()
    {
        _model = GetComponent<ModelE_Shield>();
       
    }

   
    void Update()
    {
        if (_player.targetLocked && !_model.isDead && _player.targetLocked.name == transform.name)
            lockParticle.transform.position = new Vector2(cam.WorldToScreenPoint(positionLockOn.position).x, cam.WorldToScreenPoint(positionLockOn.position).y);

        DamageShader();

        animClipName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if (animClipName == animDictionary[EnemyMeleeAnim.WalkL] || animClipName == animDictionary[EnemyMeleeAnim.WalkR] || animClipName == animDictionary[EnemyMeleeAnim.IdleCombat]) _model.strafeAnim = true;
        else _model.strafeAnim = false;
    }

    public void CreatePopText(float damage)
    {
        if (!_model.isDead)
        {
            PopText text = Instantiate(prefabTextDamage);
            StartCoroutine(FollowEnemy(text));
            text.transform.SetParent(levelUI.transform, false);
            text.SetDamage(damage);
        }
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

    public void OnChargeAnimation()
    {
        heavyHitParticle.Stop();
        heavyHitParticle.Play();
        anim.SetBool("OnCharge", true);
        anim.SetBool("IdleCombat", false);
        anim.SetBool("RunAttack", false);
    }

    public void DamageShader()
    {
        if (damaged && !_model.isDead)
        {
            timeShaderDamage -= Time.deltaTime * 2;

            foreach (var item in myMats) item.SetFloat("_Intensity", timeShaderDamage);

            if (timeShaderDamage <= 0) damaged = false;

        }
    }

    public void FailAttackAnim()
    {
        sparks.Play();
        anim.SetBool("FailAttack", true);
    }

    public void FailAttackAnimFalse()
    {
        anim.SetBool("FailAttack", false);
    }

    public void BlockAnim()
    {
        sparks.Play();
        anim.SetBool("Block", true);
    }

    public void BlockAnimFalse()
    {
        anim.SetBool("Block", false);
    }

    public void LifeBar(float val)
    {
        StartCoroutine(ess.UpdateLifeBar(val));
    }

    public void KnockAnim()
    {
        anim.SetBool("Knock", true);
    }

    public void KnockAnimFalse()
    {
        anim.SetBool("Knock", false);
    }

    public void IdleAnim()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("IdleCombat", true);
        anim.SetBool("WalkForward", false);
    }

    public void ChangeChatAnimation()
    {
        int r = Random.Range(0, 2);

        if (r == 0)
        {
            anim.SetBool("Chat1", true);
            anim.SetBool("Chat2", false);
            anim.SetBool("Chat3", false);
        }

        if (r == 1)
        {
            anim.SetBool("Chat1", false);
            anim.SetBool("Chat2", true);
            anim.SetBool("Chat3", false);
        }

        if (r == 2)
        {
            anim.SetBool("Chat1", false);
            anim.SetBool("Chat2", false);
            anim.SetBool("Chat3", true);
        }
    }

    public void TakeDamageAnim()
    {
        blood.Play();
        anim.SetBool("TakeDamage", true);
    }

    public void TakeDamageAnimFalse()
    {
        anim.SetBool("TakeDamage", false);
    }

    public void WalkLeftAnim()
    {
        anim.SetBool("WalkL", true);
        anim.SetBool("WalkR", false);
        anim.SetBool("IdleCombat", false);
    }

    public void WalkRightAnim()
    {
        anim.SetBool("WalkL", false);
        anim.SetBool("WalkR", true);
        anim.SetBool("IdleCombat", false);

    }

    public void PointAnimation()
    {
        anim.SetBool("Point", true);
    }

    public void PointAnimationFalse()
    {
        anim.SetBool("Point", false);
    }

    public void CombatWalkAnim()
    {
        anim.SetBool("WalkForward", true);
        anim.SetBool("IdleCombat", false);
        anim.SetBool("WalkL", false);
        anim.SetBool("WalkR", false);
        anim.SetBool("Idle", false);
    }

    public void WalckBackAnim()
    {
        anim.SetBool("WalkBack", true);
        anim.SetBool("WalkForward", false);
        anim.SetBool("IdleCombat", false);
        anim.SetBool("Idle", false);
        anim.SetBool("WalkL", false);
        anim.SetBool("WalkR", false);
    }

    public void CombatIdleAnim()
    {
      
        anim.SetBool("WalkForward", false);
        anim.SetBool("IdleCombat", true);
        anim.SetBool("WalkL", false);
        anim.SetBool("WalkR", false);
        anim.SetBool("Idle", false);
    }

 

    public void AttackAnim()
    {
  
        anim.SetBool("Attack", true);
        anim.SetBool("RunAttack", false);
    }

    public void RunAttackAnim()
    {
        anim.SetBool("RunAttack", true);
    }

    public void StunedAnim()
    {
        anim.SetBool("Stuned", true);
    }

    public void StunedAnimFalse()
    {
        anim.SetBool("Stuned", false);
    }

    public void PerfectBlockedAnim()
    {
     
        anim.SetBool("PerfectBlocked", true);
    }

    public void PerfectBlockedFalse()
    {
        anim.SetBool("PerfectBlocked", false);
    }

    public void AttackFalse()
    {
        anim.SetBool("Attack", false);
    }

    public void DeadAnim()
    {
        StartCoroutine(Die());

    }

    IEnumerator Die()
    {
        yield return new WaitForEndOfFrame();
        var ragdoll = Instantiate(ragdollPrefab);
        ragdoll.transform.position = transform.position;
        ragdoll.transform.rotation = transform.rotation;
        gameObject.SetActive(false);
        _model.healthBar.SetActive(false);
        var dc = FindObjectOfType<DamageControll>();
        dc.SetMeDead(_model);
    }
}
