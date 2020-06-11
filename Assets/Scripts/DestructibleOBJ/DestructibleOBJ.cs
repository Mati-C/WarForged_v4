using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

public class DestructibleOBJ : MonoBehaviour
{
    public GameObject principalMesh;
    public GameObject destructibleMesh;
    public BoxCollider myBox;
    public Animator anim;
    BoxCollider col;
    Material mat;
    float time;
    bool first;
    bool change;
    public bool canDestroy;
    public Rigidbody rb;

    public GameObject healthOrb;
    bool spawnItem = false;
    public LayerMask lm;
    bool alreadySelected = false;
    public float range;
    Vector3 startpos;


    public IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5);
        if(canDestroy) Destroy();
    }

    public IEnumerator startDisolve()
    {
        if (!first)
        {
            SpawnItem();
            first = true;
            principalMesh.SetActive(false);
            destructibleMesh.SetActive(true);
            anim.SetBool("IsHit", true);
            myBox.isTrigger = true;
            if (gameObject.name.Contains("Barrel"))
                SoundManager.instance.Play(Objects.BARREL_BREAK, transform.position, true);
            else
                SoundManager.instance.Play(Objects.JUGS_BREAK, transform.position, true);

            yield return new WaitForSeconds(5);
            col.isTrigger = true;
            StartCoroutine(Destroy());
        }
    }

    public void RecoverDes()
    {
        transform.position = startpos;
        first = false;
        principalMesh.SetActive(true);
        destructibleMesh.SetActive(false);
        anim.SetBool("IsHit", false);
        myBox.isTrigger = false;       
    }

    public void Start()
    {
        startpos = transform.position;
        rb = destructibleMesh.GetComponent<Rigidbody>();
        anim = destructibleMesh.GetComponent<Animator>();
        col = destructibleMesh.GetComponent<BoxCollider>();
        mat = principalMesh.GetComponent<MeshRenderer>().materials[0];
        myBox = GetComponent<BoxCollider>();
        //SetSpawn();
    }

    public void Update()
    {
        if (!change)
        {
            time -= Time.deltaTime;
            if (time <= 0) change = true;
        }
        else
        {
            time += Time.deltaTime;
            if (time >= 1) change = false;
        }

        mat.SetFloat("_Opacity", time);
    }

    public void SetSpawn()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, lm);
        List<Collider> destructiblesInRange = new List<Collider>();
        foreach (var i in colliders)
            if(i.GetComponent<DestructibleOBJ>())
                destructiblesInRange.Add(i);

        int random = Random.Range(0, destructiblesInRange.Count - 1);
        for (int i = 0; i < destructiblesInRange.Count; i++)
        {
            DestructibleOBJ comp = destructiblesInRange[i].GetComponent<DestructibleOBJ>();
            if (!comp.alreadySelected)
            {
                comp.spawnItem = i == random;
                comp.alreadySelected = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(principalMesh.transform.position, range);
    }

    public void SpawnItem()
    {
        float random = Random.Range(1, 100);
        if (random <= 35)
        {
            GameObject item = Instantiate(healthOrb);
            item.transform.position = transform.position + (Vector3.up / 2);
        }
    }
}
