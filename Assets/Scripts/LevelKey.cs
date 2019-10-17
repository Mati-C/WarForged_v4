using System.Collections;
using UnityEngine;

public class LevelKey : MonoBehaviour
{
    GameObject lightObject;
    SpriteRenderer lightEffect;
    public float flickerSpeed;
    Transform cam;

    void Start()
    {
        lightObject = transform.GetChild(0).gameObject;
        lightEffect = lightObject.GetComponent<SpriteRenderer>();
        StartCoroutine(LightFlicker(true));
        cam = FindObjectOfType<CamController>().transform;
    }

    void Update()
    {
        lightObject.transform.LookAt(cam);
    }

    void OnTriggerEnter(Collider c)
    {
        Model player = c.GetComponent<Model>();
        if (player)
        {
            player.hasKey = true;
            Destroy(gameObject);
        }
    }

    IEnumerator LightFlicker (bool on)
    {
        float t;
        if (on)
        {
            t = 0;
            while(t < 1)
            {
                t += Time.deltaTime * flickerSpeed;
                lightEffect.color = new Color(1, 1, 1, t);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            t = 1;
            while (t > 0)
            {
                t -= Time.deltaTime * flickerSpeed;
                lightEffect.color = new Color(1, 1, 1, t);
                yield return new WaitForEndOfFrame();
            }
        }
        StartCoroutine(LightFlicker(!on));
    }
}
