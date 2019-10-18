using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isOpening;
    Transform obj;

    void Start()
    {
        obj = transform.parent;
    }

    void Update()
    {
        if (isOpening)
            obj.Rotate(0, 0, -180 * Time.deltaTime);
    }

    public void Toggle(bool close)
    {
        if (!close)
            isOpening = true;
        else
            StartCoroutine(Close());
    }

    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.tag == "Obstacle")
            isOpening = false;
    }

    IEnumerator Close()
    {
        float t = 0;
        Vector3 start = obj.localRotation.eulerAngles;
        Vector3 target = new Vector3(90, 0, 0);
        float angle = Vector3.Angle(start, target);

        while (t <= 1)
        {
            t += Time.deltaTime * 180 / angle;
            obj.localRotation = Quaternion.Euler(Vector3.Lerp(start, target, t));
            yield return new WaitForEndOfFrame();
        }
    }
}
