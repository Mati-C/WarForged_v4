using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.PostProcessing;

public class CinematicController : MonoBehaviour
{
    
    public bool onCinematic;
    public CinemachineFreeLook mainCamera;
    FadeLevel _fade;
    Model_Player _player;
    Viewer_Player _viewer;
    MeshRenderer _meshRenderPlayer;
    List<SkinnedMeshRenderer> _SkinMeshRenderPlayer = new List<SkinnedMeshRenderer>();
    public GameObject playerHolder;
    
    [Header("Tutorial Variables:")]

    public bool startTutoCinematic;
    public Animator rocks;
    public ParticleSystem particlesDirt;
    public CinemachineVirtualCamera cinematicTutorialCam;    
    public List<Transform> npcsTuto = new List<Transform>();
    public GameObject arrow;
    public PostProcessingProfile postProcess;

    [Header("Level-1 Boss Variables:")]
    public Model_B_Ogre1 boss;
    public bool cinematicLevel1;
    public bool cinematicScapeLevel1;
    public Animator barsAnimator;
    public CinemachineVirtualCamera cinematicBarCam;
    public CinemachineVirtualCamera cinematicBossLevel1Cam;
    public GameObject winTrigger;

    [Header("Level-1 Rocks Variables:")]
    public bool cinematicRocksLevel1;
    public Transform PH_Rocks1;
    public Transform PH_Rocks2;
    public Animator rocksLevel1Animator;
    public CinemachineVirtualCamera cinematicRocksCamera;

    [Header("Level-2 Variables:")]
    public bool cinematicLevel2;
    public CinemachineVirtualCamera cinematicBossCam;
    public CinemachineVirtualCamera cinematicBarCam1;
    public CinemachineVirtualCamera cinematicBarCam2;

    private void Awake()
    {
        _player = FindObjectOfType<Model_Player>();
        _viewer = FindObjectOfType<Viewer_Player>();
        _SkinMeshRenderPlayer.AddRange(_player.GetComponentsInChildren<SkinnedMeshRenderer>());
        _meshRenderPlayer = _player.GetComponentInChildren<MeshRenderer>();
        mainCamera = GameObject.Find("CM Main View").GetComponent<CinemachineFreeLook>();
        playerHolder = GameObject.Find("Player Holder");
        _fade = FindObjectOfType<FadeLevel>();

        if (startTutoCinematic)
        {
            rocks = GameObject.Find("RockAnimation").GetComponent<Animator>();
            StartCoroutine(TutorialCinematic());
        }

        if (cinematicLevel1)
        {
            StartCoroutine(BossScapeLevel_1());
            winTrigger.SetActive(false);
        }
    }

    void Start()
    {
        if (startTutoCinematic) arrow.SetActive(false);
    }


    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider c)
    {
        if(cinematicLevel1 && c.GetComponent<Model_Player>())
        {
            cinematicLevel1 = false;
            StartCoroutine(Level_1BossCinematic());
        }

        if (cinematicRocksLevel1 && c.GetComponent<Model_Player>())
        {
            cinematicRocksLevel1 = false;
            StartCoroutine(Level_1RocksCinematic());
        }

        if(cinematicLevel2 && c.GetComponent<Model_Player>())
        {
            cinematicLevel2 = false;
            StartCoroutine(Level2_BossCinematic());
        }
    }

    IEnumerator Level2_BossCinematic()
    {
        _viewer.anim.SetBool("Walk", false);
        _viewer.anim.SetBool("Run", false);
        _viewer.anim.SetBool("Idle", true);
        _player.run = false;
        postProcess.vignette.enabled = true;
        playerHolder.gameObject.SetActive(false);
        _player.onCinematic = true;
        onCinematic = true;
        cinematicBarCam1.Priority = 1;
        mainCamera.Priority = 0;

        yield return new WaitForSeconds(1);
        barsAnimator.SetBool("Activate", true);

        yield return new WaitForSeconds(2);

        cinematicBarCam1.Priority = 0;
        cinematicBossCam.Priority = 1;
        boss.portalOrder = true;

        yield return new WaitForSeconds(5);

        cinematicBossCam.Priority = 0;
        mainCamera.Priority = 1;
        postProcess.vignette.enabled = false;
        playerHolder.gameObject.SetActive(true);
        _player.onCinematic = false;
        onCinematic = false;
    }

    IEnumerator Level_1RocksCinematic()
    {
        _viewer.anim.SetBool("Walk", false);
        _viewer.anim.SetBool("Run", false);
        _viewer.anim.SetBool("Idle", true);
        _viewer.anim.SetBool("Cinematic", true);
        _player.run = false;
        postProcess.vignette.enabled = true;
        playerHolder.gameObject.SetActive(false);
        _player.onCinematic = true;
        onCinematic = true;
        cinematicRocksCamera.Priority = 1;
        mainCamera.Priority = 0;
        var d = Vector3.Distance(_player.transform.position, PH_Rocks1.position);

        while(d>1.2f)
        {
            d = Vector3.Distance(_player.transform.position, PH_Rocks1.position);
            _player.WalkEvent();

            Quaternion targetRotation;

            var dir = (PH_Rocks1.position - _player.transform.position).normalized;
            dir.y = 0;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, 5 * Time.deltaTime);
            _player.rb.MovePosition(_player.transform.position + _player.transform.forward * _player.speed * Time.deltaTime);

           
            yield return new WaitForEndOfFrame();
        }

        var d2 = Vector3.Distance(_player.transform.position, PH_Rocks2.position);
        bool auxRocks = false;

        while (d2 > 1.2f)
        {
            d2 = Vector3.Distance(_player.transform.position, PH_Rocks2.position);
            _player.WalkEvent();

            Quaternion targetRotation;

            var dir = (PH_Rocks2.position - _player.transform.position).normalized;
            dir.y = 0;
            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, 5 * Time.deltaTime);
            _player.rb.MovePosition(_player.transform.position + _player.transform.forward * _player.speed * Time.deltaTime);

            if(d2 <=6 && !auxRocks)
            {
                auxRocks = true;
                rocksLevel1Animator.SetBool("RockActivate", true);
            } 

            yield return new WaitForEndOfFrame();
        }
        particlesDirt.Play();
        _player.GetHitHeavyEvent(false);
        yield return new WaitForSeconds(1.5f);
        _viewer.anim.SetBool("Cinematic", false);
        _viewer.anim.SetBool("Idle", true);
        cinematicRocksCamera.Priority = 0;
        mainCamera.Priority = 1;
        postProcess.vignette.enabled = false;
        playerHolder.gameObject.SetActive(true);
        _player.onCinematic = false;
        onCinematic = false;

    }



    IEnumerator BossScapeLevel_1()
    {
        while (true)
        {
            if (boss.life <= 30 && cinematicScapeLevel1) StartCoroutine(StartScapeBossLevel_1());
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartScapeBossLevel_1()
    {
        winTrigger.SetActive(true);
        boss.canScape = true;
        cinematicScapeLevel1 = false;
        _viewer.anim.SetBool("Walk", false);
        _viewer.anim.SetBool("Run", false);
        _viewer.anim.SetBool("Idle", true);
        _viewer.anim.SetBool("Cinematic", true);
        _player.run = false;
        _player.UpdateLife(100);
        postProcess.vignette.enabled = true;
        playerHolder.gameObject.SetActive(false);
        _player.onCinematic = true;
        onCinematic = true;
        cinematicBossLevel1Cam.Priority = 1;
        mainCamera.Priority = 0;
        float d = Vector3.Distance(boss.transform.position, boss.phScape);

        while (d > 1.2f)
        {
            d = Vector3.Distance(boss.transform.position, boss.phScape);
            yield return new WaitForEndOfFrame();
        }

        _viewer.anim.SetBool("Cinematic", false);
        cinematicBossLevel1Cam.Priority = 0;
        mainCamera.Priority = 1;
        postProcess.vignette.enabled = false;
        playerHolder.gameObject.SetActive(true);
        _player.onCinematic = false;
        onCinematic = false;
    }

    IEnumerator Level_1BossCinematic()
    {
        _viewer.anim.SetBool("Walk", false);
        _viewer.anim.SetBool("Run", false);
        _viewer.anim.SetBool("Idle", true);
        _player.run = false;
        postProcess.vignette.enabled = true;
        playerHolder.gameObject.SetActive(false);
        _player.onCinematic = true;
        onCinematic = true;
        float t = 4;       
        cinematicBarCam.Priority = 1;
        mainCamera.Priority = 0;

        while(t>0)
        {
            if (t <= 2.5f) barsAnimator.SetBool("Activate", true);

            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cinematicBarCam.Priority = 0;
        cinematicBossLevel1Cam.Priority = 1;

        t = 5;
        while (t > 0)
        {
            if (t <= 3f) boss.portalOrder = true;

            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cinematicBossLevel1Cam.Priority = 0;
        mainCamera.Priority = 1;
        postProcess.vignette.enabled = false;
        playerHolder.gameObject.SetActive(true);
        _player.onCinematic = false;
        onCinematic = false;
    }

    IEnumerator TutorialCinematic()
    {
        postProcess.vignette.enabled = true;
        playerHolder.gameObject.SetActive(false);
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
        postProcess.vignette.enabled = false;
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
        playerHolder.gameObject.SetActive(true);
        onCinematic = false;
    }
}
