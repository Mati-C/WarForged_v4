using System.Collections;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
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
        Destroy(gameObject);
    }
}
