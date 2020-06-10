using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageMissile : MonoBehaviour
{
    public float speed;
    public float damage;
    Model_Player _player;
    public GameObject fireBallParticles;
    public GameObject explosionParticles;
    Rigidbody _rb;
    BoxCollider _box;
    bool colission;

    IEnumerator DestroyMissile()
    {
        speed = 0;
        fireBallParticles.SetActive(false);
        explosionParticles.SetActive(true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _box = GetComponent<BoxCollider>();
        _player = FindObjectOfType<Model_Player>();
        var d = _player.transform.position - transform.position;
        d.y = 0;
        transform.forward = d;
    }

    void Update()
    {
       _rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Model_Player>())
        {
            c.GetComponent<Model_Player>().GetDamage(damage, transform);
            StartCoroutine(DestroyMissile());
        }

        else StartCoroutine(DestroyMissile());


    }
}
