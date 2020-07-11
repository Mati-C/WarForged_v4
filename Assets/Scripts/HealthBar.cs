using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public ClassEnemy enemy;

    // Update is called once per frame
    void Update()
    {
        if (enemy.isDead)
            gameObject.SetActive(false);
    }
}
