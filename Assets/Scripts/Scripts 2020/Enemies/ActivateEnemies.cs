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
        StartCoroutine(StartDesactivate());
        var mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }

    IEnumerator StartDesactivate()
    {
        yield return new WaitForSeconds(1);
        foreach (var item in myEnemies) item.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Model_Player>())
        {
            if (!activateEnemies)
            {
                foreach (var item in myEnemies)
                {
                    if (item.life > 0)
                    {
                        item.gameObject.SetActive(true);
                        item.Resume();

                    }
                }
            }

            if (activateEnemies)
            {
                foreach (var item in myEnemies)
                {
                    if (item.life > 0)
                    {
                        item.ReturnIA_Manager();
                        item.gameObject.SetActive(false);
                    }
                }
            }

        }
    }
}
