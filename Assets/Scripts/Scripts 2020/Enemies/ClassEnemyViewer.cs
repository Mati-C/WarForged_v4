using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClassEnemyViewer : MonoBehaviour
{
    [Header("Enemy Camera and UI:")]
    public Camera cam;
    public PopText prefabTextDamage;
    public GameObject levelUI;

    [Header("Enemy Particles:")]
    public ParticleSystem bloodParticle;
    public ParticleSystem burnParticle;

    [Header("Enemy Animator:")]
    public Animator anim;
    public GameObject ragdollPrefab;

    private void Awake()
    {
        
    }

    void Start()
    {

    }


    void Update()
    {

    }

    public void AnimRagdollActivate()
    {
        Instantiate(ragdollPrefab, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    public void CreatePopText(float damage)
    {

        PopText text = Instantiate(prefabTextDamage);
        StartCoroutine(FollowEnemy(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetDamage(damage);

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
}

   
