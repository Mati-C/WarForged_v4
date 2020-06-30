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
    public RuntimeAnimatorController defaultAnimator;
    public RuntimeAnimatorController getUpAnimator;
//     public Transform ragdollBones;
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
//         anim.enabled = false;
//         ragdollBones.parent = null;
//         ragdollBones.GetComponent<Rigidbody>().AddForce(-transform.forward * 40, ForceMode.Impulse);
    }

    public void CreatePopText(float damage)
    {

        PopText text = Instantiate(prefabTextDamage);
        StartCoroutine(FollowEnemy(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetDamage(damage);

    }

//     public void DesactivateAnimator()
//     {
//         StartCoroutine(AnimatorDesactivateCorrutine());
//     }
// 
//     IEnumerator AnimatorDesactivateCorrutine()
//     {
//         anim.runtimeAnimatorController = getUpAnimator;
//         anim.enabled = false;
//         ragdollBones.parent = null;
//         transform.SetParent(ragdollBones);
//         ragdollBones.GetComponent<Rigidbody>().AddForce(-transform.forward * 90, ForceMode.Impulse);
//         yield return new WaitForSeconds(2);
//         transform.parent = null;
//         ragdollBones.SetParent(transform);
//         anim.enabled = true;
//         GetUpAnim();
//     }


    public abstract void GetUpAnim(); 

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

   
