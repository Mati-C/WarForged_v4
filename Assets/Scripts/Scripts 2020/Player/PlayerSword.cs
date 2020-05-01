using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{

    public bool activated;
    Model_Player _player;
    public List<ClassEnemy> allEnemies = new List<ClassEnemy>();

    private void Awake()
    {
        allEnemies.AddRange(FindObjectsOfType<ClassEnemy>());
        _player = FindObjectOfType<Model_Player>();
    }

    public void ActivateSword()
    {
        activated = true;

        foreach (var item in allEnemies) item.ChangeCanGetDamage(true);
    }

    public void DesactivateSword()
    {
        activated = false;
        foreach (var item in allEnemies) item.ChangeCanGetDamage(false);
    }

    public void OnTriggerStay(Collider c)
    {
        if (c.GetComponent<ClassEnemy>()) c.GetComponent<ClassEnemy>().GetDamage(_player.AttackDamage);
    }
}
