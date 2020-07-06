using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggers : MonoBehaviour
{
    bool _activated;
    public GameObject Text;
    

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
        
    }

    private void Start()
    {
        Text.SetActive(false);
    }

    private void Update()
    {
       
        
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetComponent<Model_Player>() && !_activated)
        {
            Text.SetActive(true);
            _activated = true;
        }
    }
}
