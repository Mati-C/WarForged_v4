using System.Collections;
using UnityEngine;

public class Portal_Rune : MonoBehaviour
{
    public float hitsToBreak;
    float damage;
    public Portal portal;
    MeshRenderer normalMesh;
    Material runeShader;
    public GameObject destructibleMesh;
    public GameObject orb;
    public ParticleSystem particles;
    public bool portalOff;

    void Start()
    {
        particles.gameObject.GetComponent<particleAttractorLinear>().target = portal.phParticles;
        damage = 0;
        normalMesh = GetComponent<MeshRenderer>();
        runeShader = normalMesh.material;
        runeShader.SetFloat("_EmissionIntensity", 0);
    }

    public void Damage()
    {
        damage++;

        StartCoroutine(IncreaseEmission((damage / hitsToBreak) * 3));

        if (damage >= hitsToBreak)
        {
            normalMesh.enabled = false;
            orb.SetActive(false);
            destructibleMesh.SetActive(true);
            particles.Stop();
            portal.TurnOff();
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DisableCollision());
            portalOff = true;
        }
    }

    IEnumerator IncreaseEmission(float target)
    {
        float t = runeShader.GetFloat("_EmissionIntensity");
        while (t < target)
        {
            t += Time.deltaTime * 2;
            runeShader.SetFloat("_EmissionIntensity", t);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DisableCollision()
    {
        yield return new WaitForSeconds(3);
        Rigidbody[] rb = destructibleMesh.transform.GetComponentsInChildren<Rigidbody>();
        foreach (var i in rb)
            i.isKinematic = true;

        Collider[] col = destructibleMesh.transform.GetComponentsInChildren<Collider>();
        foreach (var i in col)
            i.enabled = false;
    }
}
