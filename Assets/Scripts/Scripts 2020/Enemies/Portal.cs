using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int ID;
    public Transform phPortal;
    public MeshRenderer portalMR;
    Material portalShader;
    bool canSpawn;
    public Transform phParticles;

    void Start()
    {
        canSpawn = true;
        portalShader = portalMR.material;
    }

    public void TurnOff()
    {
        canSpawn = false;
        StartCoroutine(TurnOffEffect());
    }

    IEnumerator TurnOffEffect()
    {
        float t = 0.1f;
        while (t > 0)
        {
            t -= Time.deltaTime / 10;
            portalShader.SetFloat("_Opacity", t);
            yield return new WaitForEndOfFrame();
        }
    }
}
