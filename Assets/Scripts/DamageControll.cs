using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageControll : MonoBehaviour
{
    public bool teleport1;
    public Transform pivot1;

    public List<EnemyEntity> enemies = new List<EnemyEntity>();

    Model _player;


    private void Awake()
    {
        _player = FindObjectOfType<Model>();
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var item in enemies)
        {
            item.gameObject.SetActive(false);
        }

        if (teleport1)
        {
            _player.transform.position = pivot1.position;
            teleport1 = false;
        }
    }

    public void SetMeDead(EnemyEntity e)
    {
        enemies.Add(e);
    }
}
