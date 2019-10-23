using System.Collections;
using UnityEngine;

public class LevelKey : MonoBehaviour
{
    GameObject lightObject;
    SpriteRenderer lightEffect;
    public float flickerSpeed;
    Transform cam;
    bool isInCanvas;
    public float rotationSpeed;
    GameObject UIKey;

    void Start()
    {
        if (transform.parent == null)
        {
            lightObject = transform.GetChild(0).gameObject;
            lightEffect = lightObject.GetComponent<SpriteRenderer>();
            StartCoroutine(LightFlicker(true));
            cam = FindObjectOfType<CamController>().transform;
            UIKey = cam.GetChild(0).gameObject;
            isInCanvas = false;
        }
        else
            isInCanvas = true;
    }

    void Update()
    {
        if (!isInCanvas)
            lightObject.transform.LookAt(cam);
        else
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider c)
    {
        Model player = c.GetComponent<Model>();
        if (player)
        {
            UIKey.SetActive(true);
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
