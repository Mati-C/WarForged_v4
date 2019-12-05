using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewB_Cyclops : MonoBehaviour
{
    public Animator anim;
    ModelB_Cyclops _model;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _model = GetComponent<ModelB_Cyclops>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AwakeAnim()
    {
        anim.SetBool("Awake", true);
        
    }

    public void AwakeAnimFalse()
    {
        anim.SetBool("Awake", false);
        _model.AwakeStateActivate();
    }

    public void IdleAnimation()
    {
        anim.SetBool("Idle", true);
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

    public void Attack1OnPlaceAnimationFalse()
    {
        anim.SetBool("Attack1OnPlace", false);
        _model.AnimationAttackFalse();
    }

    public void DieAnim()
    {
        anim.SetBool("Die", true);
    }
}
