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

 
}
