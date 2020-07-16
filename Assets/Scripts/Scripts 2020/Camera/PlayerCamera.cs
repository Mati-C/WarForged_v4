using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    Model_Player _player;

    [Header("Cameras:")]
    public CinemachineFreeLook mainCamera;
    public CinemachineFreeLook lockOnCamera;
    CinemachineBasicMultiChannelPerlin _noise;

    CinemachineComposer middleRig;
    CinemachineComposer topRig;
    CinemachineComposer bottonRig;

    [Header("Camera Distances:")]
    float actualCamDistance;
    public float distanceCombat;
    public float distanceNormal;

    [Header("Camera Smooth:")]
    public float smoothDistance;

    [Header("Camera Lock:")]

    public Transform middleTarget;
    public bool onLockCamera;

    IEnumerator OnLockCorrutine()
    {
        while(onLockCamera)
        {
            var center = Vector3.Lerp(_player.transform.position, _player.targetEnemy.transform.position, 0.5f);

            middleTarget.position = center;

            yield return new WaitForEndOfFrame();
        }
    }

    void Start()
    {
        _player = FindObjectOfType<Model_Player>();
        actualCamDistance = distanceNormal;
        middleRig = mainCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        topRig = mainCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
        bottonRig = mainCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>();
        _noise = mainCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
  
    void Update()
    {

    }

    public void SetCameraState(bool onCombat)
    {
        if (onCombat) StartCoroutine(CombatCameraView());

        else StartCoroutine(NormalCameraView());
    }

    public void CameraShake(float amplitude, float frequency)
    {
        _noise.m_AmplitudeGain = amplitude;
        _noise.m_FrequencyGain = frequency;
    }

    public void CameraShakeSmooth(float amplitude, float frequency, float duration)
    {
        StartCoroutine(CamShakeSmooth(amplitude, frequency, duration));
    }

    IEnumerator CamShakeSmooth(float amplitude, float frequency, float duration)
    {
        CameraShake(amplitude, frequency);
        yield return new WaitForSeconds(duration / 2);
        float t = duration / 2;
        while (t > 0)
        {
            t -= Time.deltaTime;
            CameraShake(Mathf.Lerp(0, amplitude, t), Mathf.Lerp(0, frequency, t));
            yield return new WaitForEndOfFrame();
        }
    }

    public void LockOnCam()
    {
        mainCamera.Priority = 0;
        lockOnCamera.Priority = 1;
        onLockCamera = true;
        StartCoroutine(OnLockCorrutine());
    }

    public void LockOffCam()
    {
        mainCamera.Priority = 1;
        lockOnCamera.Priority = 0;
        onLockCamera = false;
    }


    IEnumerator CombatCameraView()
    {

        /*  while (actualCamDistance < distanceCombat)
          {
              actualCamDistance += Time.deltaTime * smoothDistance;

              mainCamera.m_Orbits = new CinemachineFreeLook.Orbit[3]
              {
                      new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                      new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                      new CinemachineFreeLook.Orbit(0.4f, 1.3f)
              };

              yield return new WaitForEndOfFrame();
          }
          */
        yield return new WaitForEndOfFrame();
    }

    IEnumerator NormalCameraView()
    {

        while (actualCamDistance > distanceNormal)
        {
            actualCamDistance -= Time.deltaTime * smoothDistance;

            mainCamera.m_Orbits = new CinemachineFreeLook.Orbit[3]
            {
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(0.4f, 1.3f)
            };

            yield return new WaitForEndOfFrame();
        }
    }

 
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(middleTarget.position, new Vector3(0.2f, 0.2f, 0.2f));
    }
}
