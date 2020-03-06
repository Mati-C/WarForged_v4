using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer_Player : MonoBehaviour
{
    public Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
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
}
