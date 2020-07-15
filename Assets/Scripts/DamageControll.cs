using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageControll : MonoBehaviour
{
    public bool teleport1;
    public bool teleport2;
    public bool teleport3;
    public bool teleport4;
    public Transform pivot1;
    public Transform pivot2;
    public Transform pivot3;
    public Transform pivot4;

    public List<EnemyEntity> enemies = new List<EnemyEntity>();

    Model_Player _player;


    private void Awake()
    {
        _player = FindObjectOfType<Model_Player>();
    }
    // Update is called once per frame
    void Update()
    {

        if (teleport1)
        {
            _player.transform.position = pivot1.position;
            teleport1 = false;
        }

        if (teleport2)
        {
            _player.transform.position = pivot2.position;
            teleport2 = false;
        }

        if (teleport3)
        {
            _player.transform.position = pivot3.position;
            teleport3 = false;
        }

        if (teleport4)
        {
            _player.transform.position = pivot4.position;
            teleport4 = false;
        }
    }

    public void SetMeDead(EnemyEntity e)
    {
        enemies.Add(e);
    }
}
