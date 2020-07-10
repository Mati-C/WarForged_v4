using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Portal : MonoBehaviour
{
    public int ID;
    public Transform phPortal;
    public Transform phSpawn;
    public MeshRenderer portalMR;
    Model_Player _player;
    Material portalShader;
    bool canSpawn;
    public Transform phParticles;
    public List<ClassEnemy> myEnemies = new List<ClassEnemy>();
    public int enemiesAlive;
    public float timerRespawn;
    public float timerRespawnMax;
    public float distanceToSpawnEnemies;
    public bool myRuneDestroy;

    private void Awake()
    {
        myEnemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID));
        enemiesAlive = myEnemies.Count;
        _player = FindObjectOfType<Model_Player>();
    }

    void Start()
    {
        canSpawn = true;
        portalShader = portalMR.material;
    }

    public void PortalRemove()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0) StartCoroutine(ReSpawnEnemies());
    }

    IEnumerator ReSpawnEnemies()
    {
        timerRespawn = timerRespawnMax;
        while(timerRespawn > 0 && !myRuneDestroy)
        {
            timerRespawn -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        var distance = Vector3.Distance(_player.transform.position, transform.position);

        while(distance > distanceToSpawnEnemies)
        {
            distance = Vector3.Distance(_player.transform.position, transform.position);
            yield return new WaitForEndOfFrame();
        }

        foreach (var item in myEnemies)
        {
            item.gameObject.SetActive(true);
            item.transform.position = phSpawn.position;
            item.ReturnToLife();
            item.portalOrder = true;
            item.sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != this));
            yield return new WaitForSeconds(3f);
        }

        enemiesAlive = myEnemies.Count;
    }

    public void TurnOff()
    {
        canSpawn = false;
        StartCoroutine(TurnOffEffect());
    }

    IEnumerator TurnOffEffect()
    {
        float t = 0.1f;
        while (t > 0)
        {
            t -= Time.deltaTime / 10;
            portalShader.SetFloat("_Opacity", t);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, distanceToSpawnEnemies);
    }
}
