using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CamController : MonoBehaviour {

   
    public bool blockMouse;

    Model model;
    public Transform middleTargets;
    CinematicActivator CinematicActive;
    public CinemachineFreeLook cinemaCam;
    public CinemachineFreeLook cinemaCam2;
    public CinemachineFreeLook cinemaCam_KickCam;
    public CinemachineFreeLook CinemaCam_Cinematic01;
    public CinemachineFreeLook CinemaCam_Cinematic02;
    public CinemachineFreeLook CinemaCam_Cinematic03;
    public CinemachineFreeLook CinemaCam_AimCam;
    public CinemachineFreeLook Boss_AnimCam;
    public Transform currentTarget;
    public ModelE_Sniper mage;
    public float distanceIdle;
    public float distanceCombat;
    public float actualCamDistance;
    public float positionIdleCamOnX;
    public float positionCombatCamOnX;
    public float smoothDistance;
    public float smoothPosition;
    public float smoothAttacks;
    public float smoothMovement;
    public float smoothWalk;
    public float smoothRun;
    bool onAttack;
    bool onMovement;


    public bool aimCam;

    public bool PlayerCanMove;

    public float ShakeDuration = 0.3f;
    public float ShakeAmplitude = 1.2f;
    public float ShakeFrequency = 2.0f;

    private float ShakeElapsedTime = 0f;

    CinemachineBasicMultiChannelPerlin cameraNoise;

    CinemachineComposer middleRig;
    CinemachineComposer topRig;
    CinemachineComposer bottonRig;

    AnimationTimeLineScenes AnimationsCutScenes;

    Transform lockedTarget;
    float xMaxSpeed;
    float yMaxSpeed;
    bool corroutineRunning;

    public IEnumerator AttackTiltCamera()
    {
        onAttack = true;
        yield return new WaitForSeconds(0.2f);
        onAttack = false;
    }

    public IEnumerator KickCameraChange()
    {
        cinemaCam_KickCam.Priority = 1;
        cinemaCam.Priority = 0;
        cinemaCam2.Priority = 0;
        yield return new WaitForSeconds(1);
        cinemaCam_KickCam.Priority = 0;
        if (model.targetLockedOn)
        {
            cinemaCam2.Priority = 1;
            cinemaCam.Priority = 0;
        }
        else
        {
            cinemaCam2.Priority = 0;
            cinemaCam.Priority = 1;
        }
    }
    public IEnumerator Cinematic01()
    {
        CinemaCam_Cinematic01.Priority = 1;
        cinemaCam.Priority = 0;
        cinemaCam2.Priority = 0;

       
        AnimationsCutScenes.StartCoroutine(AnimationsCutScenes.CutSceneRocks());

        var controller = FindObjectOfType<Controller>();
        controller.ShutDownControlls(3);
        yield return new WaitForSeconds(3);
        CinemaCam_Cinematic01.Priority = 0;
        cinemaCam2.Priority = 0;
        cinemaCam_KickCam.Priority = 0;
        cinemaCam.Priority = 1;

        PlayerCanMove = false;
    }
    public IEnumerator Cinematic02()
    {

        CinemaCam_Cinematic02.Priority = 1;
        CinemaCam_Cinematic01.Priority = 0;
        cinemaCam.Priority = 0;
        cinemaCam2.Priority = 0;
        PlayerCanMove = true;

        var controller = FindObjectOfType<Controller>();
        controller.ShutDownControlls(4);
        yield return new WaitForSeconds(4);
        CinemaCam_Cinematic01.Priority = 0;
        CinemaCam_Cinematic02.Priority = 0;
        cinemaCam2.Priority = 0;
        cinemaCam_KickCam.Priority = 0;
        cinemaCam.Priority = 1;
        yield return new WaitForSeconds(2);
        mage.angleToPersuit = 360;
        mage.viewDistancePersuit = 100;
        mage.angleToPersuit = 180;
        mage.viewDistancePersuit = 100;
        PlayerCanMove = false;

    }

    public IEnumerator Cinematic03()
    {

        AnimationsCutScenes.StartCoroutine(AnimationsCutScenes.CutSceneLever());

        yield return new WaitForSeconds(3.5f);

        CinemaCam_Cinematic03.Priority = 1;
        CinemaCam_Cinematic02.Priority = 0;
        CinemaCam_Cinematic01.Priority = 0;
        cinemaCam.Priority = 0;
        cinemaCam2.Priority = 0;
        PlayerCanMove = true;


        yield return new WaitForSeconds(4);

        CinemaCam_Cinematic01.Priority = 0;
        CinemaCam_Cinematic02.Priority = 0;
        cinemaCam2.Priority = 0;
        cinemaCam_KickCam.Priority = 0;
        CinemaCam_Cinematic03.Priority = 0;
        cinemaCam.Priority = 1;

        PlayerCanMove = false;

    }

    public IEnumerator Cinematic04()
    {
        Boss_AnimCam.Priority = 1;        
        cinemaCam.Priority = 0;
        cinemaCam2.Priority = 0;
        PlayerCanMove = true;

        yield return new WaitForSeconds(2f);

        Boss_AnimCam.Priority = 0;
        cinemaCam.Priority = 1;
        cinemaCam2.Priority = 0;
        PlayerCanMove = false;

    }

    void Start()
    {
        model = FindObjectOfType<Model>();
        blockMouse = true;
        actualCamDistance = distanceIdle;
        cameraNoise = cinemaCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        middleRig = cinemaCam.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        topRig = cinemaCam.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
        bottonRig = cinemaCam.GetRig(2).GetCinemachineComponent<CinemachineComposer>();
        cinemaCam2.LookAt = middleTargets;

        AnimationsCutScenes = FindObjectOfType<AnimationTimeLineScenes>();

        xMaxSpeed = cinemaCam.m_XAxis.m_MaxSpeed;
        yMaxSpeed = cinemaCam.m_YAxis.m_MaxSpeed;
    }

    void LateUpdate()
    {
        if(cinemaCam2.Priority>=1)
        {
            currentTarget = model.targetLocked.transform;

            if (cinemaCam_KickCam.Priority != 1) cinemaCam2.m_XAxis.Value = 352f;

            var center = Vector3.Lerp(model.transform.position, currentTarget.position, 0.5f);

            middleTargets.position = center;
        }

        if (lockedTarget != null)
        {
            cinemaCam.m_XAxis.m_MaxSpeed = 0;
            cinemaCam.m_YAxis.m_MaxSpeed = 0;
            if (lockedTarget.GetComponent<EnemyEntity>().isDead)
                StartCoroutine(CameraFocus(false));
        }
        else
        {
            cinemaCam.m_XAxis.m_MaxSpeed = xMaxSpeed;
            cinemaCam.m_YAxis.m_MaxSpeed = yMaxSpeed;
        }
    }

    void Update()
    {
        if (model.animClipName == "P_WARRIOR_WALK" || model.animClipName == "P_WARRIOR_RUN")
        {

            if (!onMovement)
            {
                if (model.animClipName == "P_WARRIOR_WALK") smoothMovement = smoothWalk;
                if (model.animClipName == "P_WARRIOR_RUN") smoothMovement = smoothRun;

                middleRig.m_TrackedObjectOffset.y -= Time.deltaTime * smoothMovement;
                topRig.m_TrackedObjectOffset.y -= Time.deltaTime * smoothMovement;
                bottonRig.m_TrackedObjectOffset.y -= Time.deltaTime * smoothMovement;

                if (middleRig.m_TrackedObjectOffset.y <= 1.25f)
                {
                    middleRig.m_TrackedObjectOffset.y = 1.25f;
                    topRig.m_TrackedObjectOffset.y = 1.25f;
                    bottonRig.m_TrackedObjectOffset.y = 1.25f;
                    onMovement = true;
                }
            }

            if (onMovement)
            {

                if (model.animClipName == "P_WARRIOR_WALK") smoothMovement = smoothWalk;
                if (model.animClipName == "P_WARRIOR_RUN") smoothMovement = smoothRun;

                middleRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothMovement;
                topRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothMovement;
                bottonRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothMovement;

                if (middleRig.m_TrackedObjectOffset.y >= 1.28f)
                {
                    middleRig.m_TrackedObjectOffset.y = 1.28f;
                    topRig.m_TrackedObjectOffset.y = 1.28f;
                    bottonRig.m_TrackedObjectOffset.y = 1.28f;
                    onMovement = false;
                }
            }
        }

        if (model.animClipName == "NewIdel2.0")
        {
            middleRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothMovement;
            topRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothMovement;
            bottonRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothMovement;

            if (middleRig.m_TrackedObjectOffset.y >= 1.28f)
            {
                middleRig.m_TrackedObjectOffset.y = 1.28f;
                topRig.m_TrackedObjectOffset.y = 1.28f;
                bottonRig.m_TrackedObjectOffset.y = 1.28f;
            }
        }

        if (onAttack)
        {
            middleRig.m_TrackedObjectOffset.y -= Time.deltaTime * smoothAttacks;
            topRig.m_TrackedObjectOffset.y -= Time.deltaTime * smoothAttacks;
            bottonRig.m_TrackedObjectOffset.y -= Time.deltaTime * smoothAttacks;

            if (middleRig.m_TrackedObjectOffset.y <= 1.15f)
            {
                middleRig.m_TrackedObjectOffset.y = 1.15f;
                topRig.m_TrackedObjectOffset.y = 1.15f;
                bottonRig.m_TrackedObjectOffset.y = 1.15f;
                onAttack = false;
            }
        }

        if (!onAttack && model.animClipName != "NewIdel2.0" && model.animClipName != "P_WARRIOR_WALK" && model.animClipName != "P_WARRIOR_RUN")
        {
            middleRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothAttacks;
            topRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothAttacks;
            bottonRig.m_TrackedObjectOffset.y += Time.deltaTime * smoothAttacks;

            if (middleRig.m_TrackedObjectOffset.y >= 1.28f)
            {
                middleRig.m_TrackedObjectOffset.y = 1.28f;
                topRig.m_TrackedObjectOffset.y = 1.28f;
                bottonRig.m_TrackedObjectOffset.y = 1.28f;
            }
        }


        if (blockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (model.isInCombat)
        {
            if (actualCamDistance < distanceCombat)
            {

                actualCamDistance += Time.deltaTime * smoothDistance;

                cinemaCam.m_Orbits = new CinemachineFreeLook.Orbit[3]
                {
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(0.4f, 1.3f)
                };
            }

            if (middleRig.m_ScreenX < positionCombatCamOnX)
            {
                middleRig.m_ScreenX += Time.deltaTime * smoothPosition;
                topRig.m_ScreenX += Time.deltaTime * smoothPosition;
                bottonRig.m_ScreenX += Time.deltaTime * smoothPosition;

                if (middleRig.m_ScreenX > positionCombatCamOnX) middleRig.m_ScreenX = positionCombatCamOnX;
                if (topRig.m_ScreenX > positionCombatCamOnX) topRig.m_ScreenX = positionCombatCamOnX;
                if (bottonRig.m_ScreenX > positionCombatCamOnX) bottonRig.m_ScreenX = positionCombatCamOnX;
            }

        }

        else
        {
            if (actualCamDistance > distanceIdle)
            {

                actualCamDistance -= Time.deltaTime * smoothDistance;

                cinemaCam.m_Orbits = new CinemachineFreeLook.Orbit[3]
                {
                    new CinemachineFreeLook.Orbit(2.5f, 2.15f),
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(0.4f, 1.3f)
                };
            }

            if (middleRig.m_ScreenX > positionIdleCamOnX)
            {
                middleRig.m_ScreenX -= Time.deltaTime * smoothPosition;
                topRig.m_ScreenX -= Time.deltaTime * smoothPosition;
                bottonRig.m_ScreenX -= Time.deltaTime * smoothPosition;

                if (middleRig.m_ScreenX < positionIdleCamOnX) middleRig.m_ScreenX = positionIdleCamOnX;
                if (topRig.m_ScreenX < positionIdleCamOnX) topRig.m_ScreenX = positionIdleCamOnX;
                if (bottonRig.m_ScreenX < positionIdleCamOnX) bottonRig.m_ScreenX = positionIdleCamOnX;
            }


        }

        if (ShakeElapsedTime > 0)
        {
            cameraNoise.m_AmplitudeGain = ShakeAmplitude;
            cameraNoise.m_FrequencyGain = ShakeFrequency;

            ShakeElapsedTime -= Time.deltaTime;
        }
        else
        {
            cameraNoise.m_AmplitudeGain = 0f;
            ShakeElapsedTime = 0f;
        }



    }

    public void AttackCameraEffect()
    {
        StartCoroutine(AttackTiltCamera());
    }

    public void CamShake(float frequency, float amplitude, float timeShake)
    {
        ShakeAmplitude = amplitude;
        ShakeFrequency = frequency;

        ShakeElapsedTime = timeShake;
    }

    public void ChangeTarget(EnemyEntity e)
    {
        cinemaCam.m_Priority = 0;
        cinemaCam2.m_Priority = 1;

        currentTarget = e.transform;

        var bounds = new Bounds(model.transform.position, currentTarget.transform.position);

        middleTargets.position = bounds.center;    

    }

    public void ChangeTargetAlt(EnemyEntity e)
    {
        lockedTarget = e.transform;
        StartCoroutine(CameraFocus(true));
    }

    public IEnumerator CameraFocus(bool enemy)
    {
        if (corroutineRunning) yield break;

        corroutineRunning = true;
        Transform start = enemy ? model.transform : lockedTarget;
        Transform end = enemy ? lockedTarget : model.transform;
        GameObject o = Instantiate(new GameObject());
        float t = 0;

        while(t < 2)
        {
            t += Time.fixedDeltaTime * 2;
            o.transform.position = Vector3.Lerp(start.position, end.position, t);
            cinemaCam.LookAt = o.transform;
            yield return new WaitForFixedUpdate();
        }
        cinemaCam.LookAt = end;
        corroutineRunning = false;
        Destroy(o);
        if (!enemy)
            lockedTarget = null;
    }

    public void StopLockedTargetAlt()
    {
        StartCoroutine(CameraFocus(false));
    }

    public void StopLockedTarget()
    {
        cinemaCam.m_Priority = 1;
        cinemaCam2.m_Priority = 0;
    }

    public void AimMageCam()
    {
        switch(aimCam)
        {
            case true:
                {
                    cinemaCam.m_Priority = 1;
                    CinemaCam_AimCam.m_Priority = 0;
                    aimCam = false;
                    break;
                }

            case false:
                {
                    cinemaCam.m_Priority = 0;
                    CinemaCam_AimCam.m_Priority = 1;
                    aimCam = true;
                    break;
                }
        }
    }
}
