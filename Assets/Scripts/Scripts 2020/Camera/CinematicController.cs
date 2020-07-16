using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.PostProcessing;
using Sound;

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
    public CinemachineBasicMultiChannelPerlin noiseCamTutorial;

    [Header("Level-1 Boss Variables:")]
    public Model_B_Ogre1 boss;
    public bool cinematicLevel1;
    public bool cinematicScapeLevel1;
    public Animator barsAnimator;
    public CinemachineVirtualCamera cinematicBarCam;
    public CinemachineVirtualCamera cinematicBossLevel1Cam;
    public GameObject winTrigger;
    public GameObject portal;

    [Header("Level-1 Rocks Right Variables:")]
    public bool cinematicRocksLevel1Right;
    public Transform PH_Rocks1;
    public Transform PH_Rocks2;
    public Animator rocksLevel1Animator;
    public CinemachineVirtualCamera cinematicRocksCamera;

    [Header("Level-1 Rocks Left Variables:")]
    public bool cinematicRocksLevel1Left;
    public bool rocksOff;

    [Header("Level-2 Variables:")]
    public bool cinematicLevel2;
    public Animator barsAnimators;
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
            noiseCamTutorial = cinematicTutorialCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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

        if (cinematicRocksLevel1Right && c.GetComponent<Model_Player>())
        {
            cinematicRocksLevel1Right = false;
     
            if (!rocksOff)StartCoroutine(Level_1RocksRightCinematic());

            var cc = new List<CinematicController>();
            cc.AddRange(FindObjectsOfType<CinematicController>());
            foreach (var item in cc) item.rocksOff = true;
        }

        if (cinematicRocksLevel1Left && c.GetComponent<Model_Player>())
        {
            cinematicRocksLevel1Left = false;
            if (!rocksOff) StartCoroutine(Level_1RocksLeftCinematic());

            var cc = new List<CinematicController>();
            cc.AddRange(FindObjectsOfType<CinematicController>());
            foreach (var item in cc) item.rocksOff = true;
        }

        if (cinematicLevel2 && c.GetComponent<Model_Player>())
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
        SoundManager.instance.Play(Objects.IRON_BARS, barsAnimator.bodyPosition, true, 1);

        yield return new WaitForSeconds(2);

        cinematicBarCam1.Priority = 0;
        cinematicBossCam.Priority = 1;
        boss.portalOrder = true;
        boss.dontMove = false;
        SoundManager.instance.BossMusic(true);
        yield return new WaitForSeconds(5);

        cinematicBossCam.Priority = 0;
        mainCamera.Priority = 1;
        postProcess.vignette.enabled = false;
        playerHolder.gameObject.SetActive(true);
        _player.onCinematic = false;
        onCinematic = false;
        
        while (boss.life >0)
        {
            yield return new WaitForEndOfFrame();
        }

        bool enemiesAlive = false;
        var enemies = new List<ClassEnemy>();
        enemies.AddRange(boss.wave1);
        enemies.AddRange(boss.wave2);

        while(!enemiesAlive)
        {
            int count =0;
            foreach (var item in enemies)
            {
                if (item.life <= 0) count++;
            }
            if (count >= enemies.Count) enemiesAlive = true;
            yield return new WaitForEndOfFrame();
        }

        _viewer.anim.SetBool("Walk", false);
        _viewer.anim.SetBool("Run", false);
        _viewer.anim.SetBool("Idle", true);
        _player.run = false;
        postProcess.vignette.enabled = true;
        playerHolder.gameObject.SetActive(false);
        _player.onCinematic = true;
        onCinematic = true;
        cinematicBarCam2.Priority = 1;
        mainCamera.Priority = 0;
        barsAnimators.SetBool("Activate", true);
        SoundManager.instance.Play(Objects.IRON_BARS, _player.transform.position, true, 3);

        yield return new WaitForSeconds(3);

        cinematicBarCam2.Priority = 0;
        mainCamera.Priority = 1;
        postProcess.vignette.enabled = false;
        playerHolder.gameObject.SetActive(true);
        _player.onCinematic = false;
        onCinematic = false;
        
    }

    IEnumerator Level_1RocksLeftCinematic()
    {
        rocksLevel1Animator.SetBool("RockActivate", true);
        SoundManager.instance.Play(Objects.FALLING_ROCKS, transform.position, true);
        yield return new WaitForSeconds(1);
        particlesDirt.Play();
    }

    IEnumerator Level_1RocksRightCinematic()
    {
        _viewer.anim.SetBool("Walk", false);
        _viewer.anim.SetBool("Run", true);
        _viewer.anim.SetBool("Idle", false);
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
				SoundManager.instance.Play(Objects.FALLING_ROCKS, rocksLevel1Animator.bodyPosition, true);
            } 

            yield return new WaitForEndOfFrame();
        }
        particlesDirt.Play();
        _player.GetHitHeavyEvent(false);
        yield return new WaitForSeconds(1.5f);
        _viewer.anim.SetBool("Cinematic", false);
        _viewer.anim.SetBool("Idle", true);
        _viewer.anim.SetBool("Walk", false);
        _viewer.anim.SetBool("Run", false);
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
        boss.GetComponent<Viewerl_B_Ogre1>().healthBar.SetActive(false);
        SoundManager.instance.BossMusic(false);
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
        portal.SetActive(false);
        boss.GetComponent<Viewerl_B_Ogre1>().CreateExpPopTextScapeBoss(boss.exp);
        boss.playerFireSowrd.SwordExp(boss.exp);
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

        bool auxAudio = false;
        while(t>0)
        {
            if (t <= 2.5f)
            {
                barsAnimator.SetBool("Activate", true);
                if (!auxAudio)
                {
                    SoundManager.instance.Play(Objects.IRON_BARS, _player.transform.position, true, 3);
                    auxAudio = true;
                }
            }

            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cinematicBarCam.Priority = 0;
        cinematicBossLevel1Cam.Priority = 1;

        t = 5;
        while (t > 0)
        {
            if (t <= 3f)
            {
                boss.portalOrder = true;
                boss.dontMove = false;
            }

            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cinematicBossLevel1Cam.Priority = 0;
        mainCamera.Priority = 1;
        postProcess.vignette.enabled = false;
        playerHolder.gameObject.SetActive(true);
        _player.onCinematic = false;
        onCinematic = false;
        SoundManager.instance.BossMusic(true);
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

        bool auxAudio = false;
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

            if (t <= 2.3f && !auxAudio)
            {
                noiseCamTutorial.m_AmplitudeGain = 1;
                noiseCamTutorial.m_FrequencyGain = 1;
                auxAudio = true;
                rocks.SetBool("RockActivate", true);
                SoundManager.instance.Play(Objects.FALLING_ROCKS, rocks.bodyPosition, true);
            }

            if (t <= 0.5f && !auxDirt)
            {
                
                auxDirt = true;
                particlesDirt.Play();
                foreach (var item in anim_Npcs) item.SetBool("Die", true);

                leaderAnim.SetInteger("GetHitHeavy", 1);
                SoundManager.instance.Play(Entity.WILHELM_SCREAM, leaderRB.position, false, 1);
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);
        noiseCamTutorial.m_AmplitudeGain = 0;
        noiseCamTutorial.m_FrequencyGain = 0;
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
