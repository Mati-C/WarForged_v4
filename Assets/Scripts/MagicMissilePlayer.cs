using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissilePlayer : MonoBehaviour
{
    Model player;
    public GameObject prefabExplosion;
    public GameObject fireBallParticles;
    public float timer;
    PlayerMunition ammoAmount;
    BoxCollider myCol;
    IEnumerator DestroyExplotion()
    {
      
        myCol.isTrigger = true;
        fireBallParticles.SetActive(false);
        var explotion = Instantiate(prefabExplosion);
        explotion.transform.position = transform.position;
        yield return new WaitForSeconds(3);
        myCol.isTrigger = false;
        fireBallParticles.SetActive(true);
        ammoAmount.ReturnBulletToPool(this);
        Destroy(explotion);

    }

    private void Awake()
    {
        player = FindObjectOfType<Model>();
        ammoAmount = FindObjectOfType<PlayerMunition>();
        myCol = GetComponent<BoxCollider>();
    }

    void Start()
    {
       
    }

    public void Initialize()
    {

    }

    public void Dispose()
    {

    }
    public static void InitializeArrow(MagicMissilePlayer bulletObj)
    {
        bulletObj.gameObject.SetActive(true);
        bulletObj.Initialize();
    }

    public static void DisposeArrow(MagicMissilePlayer bulletObj)
    {
        bulletObj.Dispose();
        bulletObj.gameObject.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5) ammoAmount.ReturnBulletToPool(this);

        transform.position += transform.forward * 17 * Time.deltaTime;
    }

    public void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.GetComponent<EnemyEntity>())
        {
            c.gameObject.GetComponent<EnemyEntity>().GetDamage(player.magicMissileDamage, EnemyEntity.DamageType.Proyectile, 1);
        }

        StartCoroutine(DestroyExplotion());
    }
}
