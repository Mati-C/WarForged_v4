using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IA_CombatManager : MonoBehaviour
{

    List<ClassEnemy> enemiesList = new List<ClassEnemy>();
    public List<ClassEnemy> enemiesListOnAttack = new List<ClassEnemy>();
    Model_Player _player;

    [Range (1,10)]
    public float singleAttack;
    [Range(1, 10)]
    public float doubleDelayAttack;
    [Range(1, 10)]
    public float doubleAttack;

    public bool enemyMeleePermisionAttack;
    public bool enemyRangePermisionAttack;
    public bool decisionOnAttack;

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

    public void PermissionsMelee(bool b)
    {
        enemyMeleePermisionAttack = b;
    }

    public void PermissionsRange(bool b)
    {
        enemyRangePermisionAttack = b;
    }

    public void DecisionTake(bool b)
    {
        decisionOnAttack = b;
    }

    public void SetOrderAttack(ClassEnemy e)
    {
        int option = StartAttackStrategy();

        switch (option)
        {
            case 0:
                {
                    break;
                }

            case 1:
                {
                    enemiesListOnAttack.Add(e);
                    if (e.sameID_Enemies.Count > 0)
                    {
                        int r = Random.Range(0, e.sameID_Enemies.Count);
                        e.sameID_Enemies[r].timeToAttack =0;
                        enemiesListOnAttack.Add(e.sameID_Enemies[r]);
                    }
                    break;
                }

            case 2:
                {
                    enemiesListOnAttack.Add(e);
                    if (e.sameID_Enemies.Count > 0)
                    {
                        int r = Random.Range(0, e.sameID_Enemies.Count);
                        StartCoroutine(DelayAttackEnemy(e.sameID_Enemies[r]));
                        enemiesListOnAttack.Add(e.sameID_Enemies[r]);
                    }
                    break;
                }
        }
    }

    IEnumerator DelayAttackEnemy(ClassEnemy e)
    {
        yield return new WaitForSeconds(1f);
        e.timeToAttack = 0;
    }

    int StartAttackStrategy()
    {
        var values = new List<float>();

        values.Add(singleAttack);
        values.Add(doubleAttack);
        values.Add(doubleDelayAttack);

        var coefList = new List<float>();

        var sum = values.Sum();

        foreach (var item in values)
        {
            coefList.Add(item / sum);
        }

        int rndPercent = Random.Range(0, 100);
        float r = rndPercent / 100f;

        float sumCoef = 0;

        for (int i = 0; i < values.Count; i++)
        {
            sumCoef += coefList[i];

            if (sumCoef > r)
            {
                return i;
            }
        }

        return -1;
    }
}
