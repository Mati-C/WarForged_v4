using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActivator : MonoBehaviour
{
    public bool isActive;
    public CamController cam;
    Camera myCamera;
    GameObject interactiveKey;
    public GameObject prefabInteractiveKey;
    GameObject lockKey;
    public GameObject prefabLockKey;
    public GameObject ph;
    public GameObject lever;
    public Animator leverAnimator;
    public ParticleSystem DirtParticle;

    Model player;

    public float positionFix;
    public float timeToMove;

    public bool RockIsActivate;

    DepthUI interactiveKeyDepth;
    DepthUI lockKeyDepth;

    Canvas canvas;

    public int NumberClipAnimation;

    public Animator chains;

    IEnumerator AnimationAdjustment()
    {
        lever.GetComponent<BoxCollider>().isTrigger= true;

        player.GetComponent<Controller>().ShutDownControlls(timeToMove + 0.2f + 1);

        float actualTime = 0;

        var playerStartPos = player.transform.position;

        while (actualTime < timeToMove)
        {
            Quaternion targetRotation;

            ph.transform.position = new Vector3(ph.transform.position.x, player.transform.position.y, ph.transform.position.z);
            var dir = ph.transform.position - player.transform.position;
            dir.y = 0;

            targetRotation = Quaternion.LookRotation(dir, Vector3.up);

            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 7 * Time.deltaTime);

            player.view.anim.SetBool("trotAnim", true);
            actualTime += Time.deltaTime;
            player.transform.position = Vector3.Lerp(playerStartPos, ph.transform.position, actualTime / timeToMove);

            yield return new WaitForEndOfFrame();

        }

        actualTime = 0;

        while (actualTime < 0.1f)
        {
            Quaternion targetRotation;

            targetRotation = Quaternion.LookRotation(ph.transform.forward, Vector3.up);

            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 20 * Time.deltaTime);

            player.view.anim.SetBool("trotAnim", true);
            actualTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        player.view.anim.SetBool("trotAnim", false);
        player.view.anim.SetBool("Idle", true);
        player.view.anim.SetBool("InteractLevel", true);
      
        
        yield return new WaitForSeconds(1);

        leverAnimator.SetBool("open", true);
        player.view.anim.SetBool("InteractLevel", false);
        player.view.anim.SetBool("Idle", true);
        lever.GetComponent<BoxCollider>().isTrigger = false;
        cam.StartCoroutine(cam.Cinematic03());
    }

    public void Start()
    {
        myCamera = cam.GetComponent<Camera>();
        canvas = FindObjectOfType<Canvas>();
        interactiveKey = Instantiate(prefabInteractiveKey);
        interactiveKey.transform.SetParent(canvas.transform, false);
        interactiveKey.SetActive(false);
        lockKey = Instantiate(prefabLockKey);
        lockKey.transform.SetParent(canvas.transform, false);
        lockKey.SetActive(false);
        interactiveKeyDepth = interactiveKey.GetComponent<DepthUI>();
        lockKeyDepth = lockKey.GetComponent<DepthUI>();
        player = FindObjectOfType<Model>();
    }

    private void Update()
    {
        Vector3 worldPos = transform.position + Vector3.up * positionFix ;
        Vector3 screenPos = myCamera.WorldToScreenPoint(worldPos);
        interactiveKey.transform.position = screenPos;
        lockKey.transform.position = myCamera.WorldToScreenPoint(transform.position + (Vector3.down * 0.7f));

        float distance = Vector3.Distance(worldPos, cam.transform.position);
        interactiveKeyDepth.depth = -distance;
        lockKeyDepth.depth = -distance;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && !isActive)
        {
            if(NumberClipAnimation != 3 && NumberClipAnimation != 0) isActive = true;

            if (NumberClipAnimation == 1)
            {             
                NumberClipAnimation = 0;
                RockIsActivate = true;
                cam.StartCoroutine(cam.Cinematic01());
            }
            if (NumberClipAnimation == 2)
            {
                NumberClipAnimation = 0;
                cam.StartCoroutine(cam.Cinematic02());
            }    
                      
        }
    }

    public void OnTriggerStay(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && !isActive)
        {

            if (NumberClipAnimation != 0 && NumberClipAnimation == 3)
            {
                if (player.hasKey)
                    interactiveKey.SetActive(true);
                else
                    lockKey.SetActive(true);
            }

            if (NumberClipAnimation == 3 && Input.GetKeyDown(KeyCode.F) && player.hasKey)
            {
                BreakChains();
                player.hasKey = false;
                cam.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void BreakChains()
    {
        NumberClipAnimation = 0;
        interactiveKey.SetActive(false);
        chains.enabled = true;
        StartCoroutine(AnimationAdjustment());
        interactiveKey.SetActive(false);
        isActive = true;
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            interactiveKey.SetActive(false);
            lockKey.SetActive(false);
            isActive = false;
        }
    }
}
