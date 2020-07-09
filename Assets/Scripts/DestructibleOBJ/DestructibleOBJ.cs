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
    public bool destroyed;
    Model_Player _player;
    Viewer_Player _playerView;

    Vector3 startpos;
    public float xp;

    public void Break()
    {
        if (!destroyed)
        {
            destroyed = true;
            destructibleMesh.SetActive(true);
            anim.SetBool("IsHit", true);
            myBox.isTrigger = true;
            _player.fireSword.SwordExp(xp);
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
        destroyed = false;
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
        _player = FindObjectOfType<Model_Player>();
        _playerView = FindObjectOfType<Viewer_Player>();
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
