using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMunition : MonoBehaviour
{
    public Pool<MagicMissilePlayer> arrowsPool;
    public MagicMissilePlayer arrowPrefab;

    // Use this for initialization
    void Start()
    {

        arrowsPool = new Pool<MagicMissilePlayer>(5, MissileFactory, MagicMissilePlayer.InitializeArrow, MagicMissilePlayer.DisposeArrow, true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public MagicMissilePlayer MissileFactory()
    {
        MagicMissilePlayer missele = Instantiate(arrowPrefab);
        missele.transform.SetParent(this.transform);
        return missele;
    }

    public void ReturnBulletToPool(MagicMissilePlayer missile)
    {
        missile.timer = 0;
        arrowsPool.DisablePoolObject(missile);
    }
}
