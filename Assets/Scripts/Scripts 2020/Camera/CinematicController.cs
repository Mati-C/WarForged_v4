using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using System.Linq;

public class CinematicController : MonoBehaviour
{
    
    public bool onCinematic;
    public CinemachineFreeLook mainCamera;
    FadeLevel _fade;
    Model_Player _player;
    Viewer_Player _viewer;
    MeshRenderer _meshRenderPlayer;
    List<SkinnedMeshRenderer> _SkinMeshRenderPlayer = new List<SkinnedMeshRenderer>();

    [Header("Tutorial Variables:")]

    public bool startTutoCinematic;
    public Animator rocks;
    public ParticleSystem particlesDirt;
    public CinemachineVirtualCamera cinematicTutorialCam;    
    public List<Transform> npcsTuto = new List<Transform>();
    public GameObject arrow;

    private void Awake()
    {
        _player = FindObjectOfType<Model_Player>();
        _viewer = FindObjectOfType<Viewer_Player>();
        _SkinMeshRenderPlayer.AddRange(_player.GetComponentsInChildren<SkinnedMeshRenderer>());
        _meshRenderPlayer = _player.GetComponentInChildren<MeshRenderer>();
        mainCamera = GameObject.Find("CM Main View").GetComponent<CinemachineFreeLook>();
        _fade = FindObjectOfType<FadeLevel>();

        if (startTutoCinematic)
        {
            rocks = GameObject.Find("RockAnimation").GetComponent<Animator>();
            StartCoroutine(TutorialCinematic());
        }
    }

    void Start()
    {
        if (startTutoCinematic) arrow.SetActive(false);
    }


    void Update()
    {
        
    }

    IEnumerator TutorialCinematic()
    {
       
        foreach (var item in _SkinMeshRenderPlayer)
        {
            item.enabled = false;
        }
        _meshRenderPlayer.enabled = false;

        _player.onCinematic = true;
        onCinematic = true;
        cinematicTutorialCam.Priority = 10;
        List<Rigidbody> rb_Npcs = new List<Rigidbody>();
        List<Animator> anim_Npcs = new List<Animator>();

        rb_Npcs.AddRange(npcsTuto.Select(x=> x.GetComponent<Rigidbody>()).Where(x => x.name != "NPC Leader"));
        anim_Npcs.AddRange(npcsTuto.Select(x => x.GetComponent<Animator>()).Where(x => x.name != "NPC Leader"));

        var leaderRB = npcsTuto.Where(x => x.name == "NPC Leader").Select(x=> x.GetComponent<Rigidbody>()).First();
        var leaderAnim = leaderRB.GetComponent<Animator>();
        leaderAnim.SetBool("TutoKnocked", true);

        bool auxDirt = false;
        float t = 3;
        while (t >0)
        {
            t -= Time.deltaTime;

            leaderRB.MovePosition(leaderRB.position + leaderRB.transform.forward * 3 * Time.deltaTime);
            leaderAnim.SetBool("Run", true);

            foreach (var item in rb_Npcs)
            {
                item.MovePosition(item.position + item.transform.forward * 3 * Time.deltaTime);
            }

            foreach (var item in anim_Npcs)
            {
                item.SetBool("Run", true);
            }

            if(t<=2.3f) rocks.SetBool("RockActivate", true);

            if(t<=0.5f && !auxDirt)
            {
                auxDirt = true;
                particlesDirt.Play();
                foreach (var item in anim_Npcs) item.SetBool("Die", true);
                
                leaderAnim.SetInteger("GetHitHeavy", 1);
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);
        _fade.FadeIn(false);
        yield return new WaitForSeconds(4);
        _fade.FadeOut(false);
        leaderRB.gameObject.SetActive(false);

        foreach (var item in _SkinMeshRenderPlayer)
        {
            item.enabled = true;
        }
        _meshRenderPlayer.enabled = true;

        cinematicTutorialCam.Priority = 0;
        _player.onCinematic = false;
        _viewer.anim.SetBool("TutoKnocked", false);
        _viewer.anim.SetBool("Idle", true);
        arrow.SetActive(true);
        onCinematic = false;
    }
}
