using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTriggers : MonoBehaviour
{
    bool _activated;
    public GameObject Text;
    public Text TextTuto;
    

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
            TextTuto.gameObject.SetActive(true);
            _activated = true;
        }
    }
}
