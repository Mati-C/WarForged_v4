using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IA_CombatManager : MonoBehaviour
{

    List<ClassEnemy> enemiesList = new List<ClassEnemy>();
    Model_Player _player;


    private void Start()
    {
        enemiesList.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => !x.isDead));
        _player = FindObjectOfType<Model_Player>();
    }


    public void UpdateEnemyAggressive()
    {
        enemiesList.Clear();

        enemiesList.AddRange(FindObjectsOfType<ClassEnemy>().Where(x => !x.isDead));

        if (enemiesList.Count > 0)
        {
            int count = 0;

            var nearEntites = enemiesList.OrderBy(x =>
            {

                var d = Vector3.Distance(x.transform.position, _player.transform.position);
                return d;
            });


            foreach (var item in nearEntites)
            {
                if (count <= 1) item.aggressiveLevel = 1;

                if (count <= 5 && count > 1) item.aggressiveLevel = 2;

                count++;
            }
        }
    }
}
