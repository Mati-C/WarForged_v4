using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IA_CombatManager : MonoBehaviour
{

    List<ClassEnemy> enemiesList = new List<ClassEnemy>();

    Model_Player _player;

    [Range (1,10)]
    public float singleAttack;
    [Range(1, 10)]
    public float tripleAttack;
    [Range(1, 10)]
    public float doubleAttack;

    public bool enemyMeleePermisionAttack;
    public bool enemyRangePermisionAttack;
    public bool decisionOnAttackMelee;
    public bool decisionOnAttackRange;

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

    public void DecisionTakeMelee(bool b)
    {
        decisionOnAttackMelee = b;
    }

    public void DecisionTakeRange(bool b)
    {
        decisionOnAttackRange = b;
    }

    public void SetOrderAttack(ClassEnemy e)
    {
        int option = 0;
        if (e.sameID_Enemies.Count > 1)
        {
            option = StartAttackStrategy();
        }

        switch (option)
        {
            case 0:
                {
                    break;
                }

            case 1:
                {

                    if (e.sameID_Enemies.Count > 0)
                    {
                        var newEnemies = new List<ClassEnemy>();

                        if (e.melee) newEnemies.AddRange(e.sameID_Enemies.Where(x => x.melee && x.life >0));

                        if (e.range) newEnemies.AddRange(e.sameID_Enemies.Where(x => x.range && x.life > 0));

                        if (newEnemies.Count > 0)
                        {
                            int r = Random.Range(0, newEnemies.Count);
                            newEnemies[r].timeToAttack = 0;
                        }
                    }
                    break;
                }

            case 2:
                {
                    if (e.sameID_Enemies.Count > 0)
                    {
                        var newEnemies = new List<ClassEnemy>();

                        if (e.melee) newEnemies.AddRange(e.sameID_Enemies.Where(x => x.melee && x.life > 0));

                        if (e.range) newEnemies.AddRange(e.sameID_Enemies.Where(x => x.range && x.life > 0));

                        if (newEnemies.Count > 0)
                        {
                            int r = Random.Range(0, newEnemies.Count);
                            newEnemies[r].timeToAttack = 0;
                            newEnemies.Remove(newEnemies[r]);
                        }

                        if (newEnemies.Count > 0)
                        {
                            int r = Random.Range(0, newEnemies.Count);
                            newEnemies[r].timeToAttack = 0;
                        }
                    }
                    break;
                }
        }
    }

    int StartAttackStrategy()
    {
        var values = new List<float>();

        values.Add(singleAttack);
        values.Add(doubleAttack);
        values.Add(tripleAttack);

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
