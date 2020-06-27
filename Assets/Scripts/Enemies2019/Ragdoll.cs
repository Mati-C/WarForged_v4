using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody forcePosition;
    public float forceAmount;
    Rigidbody[] allRigidbodies;

    Model_Player player;

    void Start()
    {
        player = FindObjectOfType<Model_Player>();
        Vector3 dir = -(player.transform.position - transform.position).normalized;
        dir.y = 0;
        forcePosition.AddForce(dir * forceAmount);
        allRigidbodies = GetComponentsInChildren<Rigidbody>();
        StartCoroutine(Bury());
    }

    IEnumerator Bury()
    {
        yield return new WaitForSeconds(10);
        foreach (var rb in allRigidbodies)
            rb.isKinematic = true;

        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;
    }
}
