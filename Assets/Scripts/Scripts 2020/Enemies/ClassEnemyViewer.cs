using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClassEnemyViewer : MonoBehaviour
{
    [Header("Enemy Camera and UI:")]
    public Camera cam;
    public PopText prefabTextDamage;
    public PopExpText prefabExpText;
    public GameObject levelUI;
    public EnemyScreenSpace ess;

    [Header("Enemy Particles:")]
    public ParticleSystem bloodParticle;
    public ParticleSystem burnParticle;

    [Header("Enemy Animator:")]
    public Animator anim;
    public GameObject ragdollPrefab;

    [Header("Enemy Meshes:")]
    public List<MeshRenderer> myMeshes = new List<MeshRenderer>();
    public List<SkinnedMeshRenderer> mySkinedMeshes = new List<SkinnedMeshRenderer>();

    Viewer_Player _player;
    FireSword _sowrd;


    public void AwakeViewer()
    {
        _player = FindObjectOfType<Viewer_Player>();
        _sowrd = FindObjectOfType<FireSword>();

    }

    public void GetMeshes()
    {
        myMeshes.AddRange(GetComponentsInChildren<MeshRenderer>());
        mySkinedMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
    }

    public void ReturnToIdleAnim()
    {
        foreach (var item in myMeshes) item.enabled = true;
        foreach (var item in mySkinedMeshes) item.enabled = true;
        var capsuleCol = GetComponent<CapsuleCollider>();
        capsuleCol.isTrigger = false;
        anim.SetBool("Die", false);
        anim.SetBool("true", true);
    }


    public void AnimRagdollActivate()
    {
        Instantiate(ragdollPrefab, transform.position, transform.rotation);
        foreach (var item in myMeshes) item.enabled = false;
        foreach (var item in mySkinedMeshes) item.enabled = false;
        var rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        var capsuleCol = GetComponent<CapsuleCollider>();
        capsuleCol.isTrigger = true;
    }

    public void CreatePopText(float damage)
    {
        PopText text = Instantiate(prefabTextDamage);
        StartCoroutine(FollowEnemy(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetDamage(damage);
    }

    public void CreateExpPopText(float exp)
    {
        StartCoroutine(ChargeExpFireText(exp));
        PopExpText text = Instantiate(prefabExpText);
        StartCoroutine(FollowEnemyExp(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetExp(exp);
    }

    IEnumerator ChargeExpFireText(float exp)
    {
        float t = 0.5f;
        float newExp = _sowrd.currentExp;
        _player.timertAlphaSwordExp = 2;
        while(t>0)
        {
            t -= Time.deltaTime;
            newExp += Time.deltaTime * (exp / 0.5f);
            int n = (int)newExp;
            _player.swordExp.text = n + "Exp";
            yield return new WaitForEndOfFrame();
        }
    }

    public void PushKnockedAnim()
    {
        StartCoroutine(PushCorrutine());
    }

    IEnumerator PushCorrutine()
    {
        anim.SetBool("Knocked", true);
        yield return new WaitForSeconds(3f);
        anim.SetBool("Knocked", false);
    }

    public void BurnOn_Off(bool b)
    {
        if (b) burnParticle.Play();

        else burnParticle.Stop();
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

    IEnumerator FollowEnemyExp(PopExpText text)
    {
        while (text != null)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(transform.position + (Vector3.up * 2));
            text.transform.position = screenPos;
            yield return new WaitForEndOfFrame();
        }
    }

    public void UpdateLifeBar(float val)
    {
        StartCoroutine(ess.UpdateLifeBar(val));
    }
}

   
