using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTutorial : MonoBehaviour
{
    Transform _cam;
    public Vector3 plusPos;
    public Transform target;

    void Start()
    {
        _cam = FindObjectOfType<PlayerCamera>().GetComponent<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        var dir = (_cam.position - transform.position).normalized;
        transform.forward = -dir;
        transform.position = target.position + target.right/1.7f + plusPos;
    }
}
