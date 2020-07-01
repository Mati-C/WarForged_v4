using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActivateEnemies : MonoBehaviour
{
    public int ID;
    public bool activateEnemies;
    public List<ClassEnemy> myEnemies = new List<ClassEnemy>();

    private void Awake()
    {
        myEnemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID));
       
    }

    private void Start()
    {
        foreach (var item in myEnemies) item.gameObject.SetActive(false);
        var mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Model_Player>())
        {
            if (!activateEnemies)
            {
                foreach (var item in myEnemies) if (item.life > 0) item.gameObject.SetActive(true);
            }

            if (activateEnemies)
            {
                foreach (var item in myEnemies) if (item.life > 0) item.gameObject.SetActive(false);
            }

        }
    }
}
