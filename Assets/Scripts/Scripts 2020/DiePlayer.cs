using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider c)
    {
        
        if (c.GetComponent<Model_Player>())
        {
            var p = c.GetComponent<Model_Player>();
            p.life = 0;

            if (p.life <= 0 && !p.isDead)
            {
                p.DieEvent();
                StartCoroutine(p.DeadCorrutine());
            }
        }

    }
}
