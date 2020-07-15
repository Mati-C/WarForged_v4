using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NewEnemyManager : MonoBehaviour
{
    public List<Transform> phList = new List<Transform>();
    public int ID;
    public ClassEnemy learnEnemy;
    public List<ClassEnemy> myEntities = new List<ClassEnemy>();

    private void Awake()
    {
        myEntities.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == ID && x != learnEnemy));
        StartCoroutine(LearnEnemyAlive());
        
    }



    IEnumerator LearnEnemyAlive()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < myEntities.Count; i++)
        {
            myEntities[i].transform.position = phList[i].position;
            myEntities[i].dontMove = true;
        }
        learnEnemy.sameID_Enemies.Clear();
        while (learnEnemy.life >0)
        {
            foreach (var item in myEntities)
            {
                item.viewDistancePersuit = 0;
            }
            learnEnemy.sameID_Enemies.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => x.ID == learnEnemy.ID && x != learnEnemy));
            yield return new WaitForEndOfFrame();
        }

        foreach (var item in myEntities)
        {
            item.dontMove = false;
            item.enemyLayer = item.layersPlayer;
            item.viewDistancePersuit = 100;
            item.angleToPersuit = 360;
            item.angleToSurround = 360;
        }
    }
}
