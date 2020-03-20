using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public CinemachineFreeLook cinemaCam;

    CinemachineComposer middleRig;
    CinemachineComposer topRig;
    CinemachineComposer bottonRig;

    [Header("Camera Distances:")]
    float actualCamDistance;
    public float distanceCombat;
    public float distanceNormal;

    [Header("Camera Smooth:")]
    public float smoothDistance;

    void Start()
    {
        actualCamDistance = distanceNormal;
        middleRig = cinemaCam.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        topRig = cinemaCam.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
        bottonRig = cinemaCam.GetRig(2).GetCinemachineComponent<CinemachineComposer>();
    }

    
    void Update()
    {
        
    }

    public void SetCameraState(bool onCombat)
    {
        if (onCombat) StartCoroutine(CombatCameraView());

        else StartCoroutine(NormalCameraView());
    }

    IEnumerator CombatCameraView()
    {

        while (actualCamDistance < distanceCombat)
        {
            actualCamDistance += Time.deltaTime * smoothDistance;

            cinemaCam.m_Orbits = new CinemachineFreeLook.Orbit[3]
            {
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(0.4f, 1.3f)
            };

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator NormalCameraView()
    {

        while (actualCamDistance > distanceNormal)
        {
            actualCamDistance -= Time.deltaTime * smoothDistance;

            cinemaCam.m_Orbits = new CinemachineFreeLook.Orbit[3]
            {
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(2.5f, actualCamDistance),
                    new CinemachineFreeLook.Orbit(0.4f, 1.3f)
            };

            yield return new WaitForEndOfFrame();
        }
    }
}
