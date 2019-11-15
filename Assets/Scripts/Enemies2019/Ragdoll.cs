﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody forcePosition;
    public float forceAmount;
    Rigidbody[] allRigidbodies;

    Model player;
    public List<SkinnedMeshRenderer> myMeshes = new List<SkinnedMeshRenderer>();
    public List<Material> myMats = new List<Material>();

    public GameObject bloodPool;
    Material matPool;
    float timerExpandPool;
    float timerVanishPool;
    public LayerMask lm;

    void Start()
    {
        player = FindObjectOfType<Model>();
        forcePosition.AddForce(player.transform.forward * forceAmount);
        allRigidbodies = GetComponentsInChildren<Rigidbody>();
        StartCoroutine(Bury());

        myMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());

        foreach (var item in myMeshes)
        {
            myMats.Add(item.materials[0]);
            item.materials[0].SetFloat("_Intensity", 0);
        }
    }

    IEnumerator Bury()
    {
        yield return new WaitForSeconds(0.75f);
        RaycastHit hit;
        Physics.Raycast(forcePosition.position, Vector3.down, out hit, 5, lm);

        var pool = Instantiate(bloodPool);
        matPool = pool.GetComponent<MeshRenderer>().material;
        pool.transform.forward = transform.forward;
        pool.transform.position = hit.point;
        StartCoroutine(BloodPoolAnim());

        yield return new WaitForSeconds(10);
        foreach (var rb in allRigidbodies)
            rb.isKinematic = true;

        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;
    }

    public IEnumerator BloodPoolAnim()
    {
        while (timerExpandPool < 1)
        {
            MatPoolExpand();
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2.5f);

        while (timerVanishPool < 1)
        {
            MatPoolVanish();
            yield return new WaitForEndOfFrame();
        }

    }

    public void MatPoolExpand()
    {
        timerExpandPool += Time.deltaTime * 0.5f;
        if (timerExpandPool >= 1) timerExpandPool = 1;
        matPool.SetFloat("_FillAmount", timerExpandPool);
    }

    public void MatPoolVanish()
    {
        timerVanishPool += Time.deltaTime / 2.2f;
        if (timerVanishPool >= 1) timerVanishPool = 1;
        matPool.SetFloat("_Vanish", timerVanishPool);
    }
}
