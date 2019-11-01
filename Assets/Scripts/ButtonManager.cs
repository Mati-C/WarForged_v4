using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour, ICheckObserver
{
    public Transform CheckTransform;
    public Transform firstCheck;
    public Model player;
    public CamController cam;
    public GameObject pauseMenu;
    public GameObject buttonRespawn;
    bool _GameOver;
    public bool Starting;
    public bool startRespawn;
    public bool pause;
    public bool startFirstRespawn;
    public RawImage startFade;
    public GameObject diedMenu;

    public Image controls;
    Material controlsMat;
    public GameObject controlButtons;
    public float dissolveTime;
    public GameObject orb;
    public Transform orbTarget;

    List<EnemyEntity> enemies = new List<EnemyEntity>();

    public void Awake()
    {
        cam = FindObjectOfType<CamController>();

        enemies.AddRange(FindObjectsOfType<EnemyEntity>());

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            controlsMat = controls.material;
        }
        else
        {
            cam = FindObjectOfType<CamController>();
        }
    }
    public void Update()
    {
      /*  if (player.life <= 0 && !startRespawn) StartCoroutine(Respawn());
        if (startRespawn) RespawnScene();
        if (startFirstRespawn) RespawnFirstCheck();
        */
    }
    public void OnNotify(Transform check)
    {
        CheckTransform = check;
    }

    public void LoadLevel1()
    {
        Time.timeScale = 1;
        LoadingScreen.instance.LoadLevel(1);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        LoadingScreen.instance.LoadLevel(0);
    }

    public void Resume()
    {
        pause = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        cam.blockMouse = true;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        LoadingScreen.instance.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleWindow (GameObject window)
    {
        window.SetActive(!window.activeSelf);
        if (window.activeSelf)
            StartCoroutine(ControlsEffect());
    }

    public IEnumerator ControlsEffect()
    {
        controlsMat.SetFloat("_DissolveAmount", 0);
        controlButtons.SetActive(false);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / dissolveTime;
            controlsMat.SetFloat("_DissolveAmount", t);
            yield return new WaitForEndOfFrame();
        }
        controlButtons.SetActive(true);
    }

    public void ToggleDescription(GameObject d)
    {
        d.SetActive(!d.activeSelf);
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.5f);
        if (!_GameOver)
        {
            _GameOver = true;
        }
        buttonRespawn.SetActive(true);

    }

    public void RespawnFirstCheck()
    {
        buttonRespawn.SetActive(false);
        pauseMenu.SetActive(false);

        Starting = true;    

        if (Starting)
        {
            player.transform.position = firstCheck.transform.position;
            player.transform.rotation = firstCheck.transform.rotation;
            StartCoroutine(ResetCamera());
            player.isIdle = true;
            player.life = 100;
            player.mana = 100;
            player.maxLife = 100;
            player.maxMana = 100;
            player.isDead = false;
        }
    }

    public IEnumerator Fade()
    {
        yield return new WaitForSeconds(0.5f);
        //if (CheckRune) StartCoroutine(CheckRune.AttractOrb(true));
        StartCoroutine(AttractOrb());
        startFade.enabled = true;
        startFade.CrossFadeAlpha(0, 3.5f, false);
    }

    public IEnumerator AttractOrb()
    {
        GameObject o = Instantiate(orb);
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / 2;
            o.transform.position = Vector3.LerpUnclamped(orbTarget.transform.position, orbTarget.transform.position + (Vector3.up * 2), t);
            o.transform.localScale = new Vector3(t * 0.3f, t * 0.3f, t * 0.3f);
            yield return new WaitForEndOfFrame();
        }
        Destroy(o);
        yield return new WaitForEndOfFrame();
    }

    public void RespawnScene()
    {
        Time.timeScale = 1;
        pause = false;   
        pauseMenu.SetActive(false);
        player.transform.position = CheckTransform.position;
        player.transform.rotation = CheckTransform.rotation;
        StartCoroutine(ResetCamera());
        player.view.anim.SetBool("IsDead", false);
        player.view.anim.SetBool("IsIdle", true);
        player.life = player.maxLife;
        player.UpdateLife(player.maxLife);
        player.mana = player.maxMana;       
        player.isIdle = true;
        player.isDead = false;
        player.rb.isKinematic = false;
        startRespawn = false;
        StartCoroutine(Fade());
        diedMenu.SetActive(false);
        cam.blockMouse = true;
        player.fadeTimer = 0;

        foreach (var item in enemies)
        {
            if (!item.cantRespawn)
            {
                if (item.isDead) item.gameObject.SetActive(true);
                else item.Respawn();
            }
        }

        var combatlocks = FindObjectsOfType<CombatArea>();

        foreach (var item in combatlocks) item.firstPass = false;

        player.view.anim.SetBool("Idle", true);
        player.view.anim.SetBool("IdleCombat", false);
        player.view.anim.SetBool("IsInCombat", false);
        player.view.anim.SetBool("runAnim", false);
        player.view.anim.SetBool("trotAnim", false);
        player.view.anim.SetLayerWeight(1, 0);
        player.view.anim.SetLayerWeight(0, 1);
        player.view.anim.SetBool("IsDead", false);
        
    }

    IEnumerator ResetCamera()
    {
        CamController cam = FindObjectOfType<CamController>();
        cam.cinemaCam.m_RecenterToTargetHeading.m_enabled = true;
        yield return new WaitForSeconds(0.015f);
        cam.cinemaCam.m_RecenterToTargetHeading.m_enabled = false;
    }

    public void StartRespawn()
    {
        Time.timeScale = 1;
        pause = false;
        startRespawn = true;
    }

    public void StartFirstRespawn()
    {
        Time.timeScale = 1;
        pause = false;
        startFirstRespawn = true;
    }
 
}
