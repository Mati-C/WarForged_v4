using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    public float healAmount;

    void Start()
    {
        StartCoroutine(Effect(FindObjectOfType<Model_Player>()));
    }

    IEnumerator Effect(Model_Player player)
    {
        Vector3 initialPos = transform.position;

        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, player.transform.position + (Vector3.up * 1), t);
            yield return new WaitForEndOfFrame();
        }
        player.UpdateLife(healAmount);
        Destroy(gameObject);
    }
}
