using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

public class DestructibleOBJ : MonoBehaviour
{
    public GameObject mainMesh;
    public GameObject destructibleMesh;
    BoxCollider myBox;
    Animator anim;
    bool first;

    Vector3 startpos;

    public void Break()
    {
        if (!first)
        {
            first = true;
            destructibleMesh.SetActive(true);
            anim.SetBool("IsHit", true);
            myBox.isTrigger = true;

            if (gameObject.name.Contains("Barrel") || gameObject.name.Contains("Jugs"))
            {
                mainMesh.SetActive(false);
                if (gameObject.name.Contains("Barrel"))
                    SoundManager.instance.Play(Objects.BARREL_BREAK, transform.position, true);
                else
                    SoundManager.instance.Play(Objects.JUGS_BREAK, transform.position, true);
            }
            else
                SoundManager.instance.Play(Objects.JUGS_BREAK, transform.position, true);
    }
    }

    public void RecoverDes()
    {
        transform.position = startpos;
        first = false;
        if (gameObject.name.Contains("Barrel") || gameObject.name.Contains("Jugs"))
        {
            mainMesh.SetActive(true);
            destructibleMesh.SetActive(false);
        }
        anim.SetBool("IsHit", false);
        myBox.isTrigger = false;       
    }

    public void Start()
    {
        startpos = transform.position;
        anim = destructibleMesh.GetComponent<Animator>();
        myBox = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision c)
    {
        if(!gameObject.name.Contains("Barrel"))
            if (c.gameObject.GetComponent<Model_Player>())
                if (c.gameObject.GetComponent<Model_Player>().run)
                    Break();
    }
}
