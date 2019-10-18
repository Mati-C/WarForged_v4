﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour, ICheckObservable
{
    public GameObject fire;
    public float lightIntensity;
  
    public GameObject textCheck;
    public GameObject buttonRespawn;
    public Transform ph;
    ButtonManager ButtonManager;
    Rune rune;
    Model player;
    public bool checkPointActivated = false;

    public List<CheckPoint> listaChecks = new List<CheckPoint>();
    bool move1;
  
    List<ICheckObserver> _allObservers = new List<ICheckObserver>();

    public IEnumerator Message()
    {
        move1 = true;
        textCheck.SetActive(true);
        fire.SetActive(true);
        StartCoroutine(Light());
        yield return new WaitForSeconds(4);
        move1 = false;
        textCheck.SetActive(false);      
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2);
        buttonRespawn.SetActive(true);
    }

    public IEnumerator Light()
    {
        float t = 0;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            fire.transform.GetChild(0).GetComponent<Light>().intensity = Mathf.Lerp(0, lightIntensity, t / 2);
            yield return new WaitForEndOfFrame();
        }
    }

    void Start ()
    {
        player = FindObjectOfType<Model>();
        ButtonManager = FindObjectOfType<ButtonManager>();
        listaChecks.AddRange(FindObjectsOfType<CheckPoint>());
        rune = GetComponent<Rune>();
        fire.SetActive(false);

        Subscribe(ButtonManager);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetComponent(typeof(Model)) && !checkPointActivated)
        {
            ButtonManager.OnNotify(ph, gameObject.GetComponent<Rune>());
            StartCoroutine(Message());
            checkPointActivated = true;
        }
    }

    public void Subscribe(ICheckObserver observer)
    {
        if (!_allObservers.Contains(observer))
            _allObservers.Add(observer);
    }

    public void Unsubscribe(ICheckObserver observer)
    {
        if (_allObservers.Contains(observer))
            _allObservers.Remove(observer);
    }
}
