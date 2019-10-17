using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocksTrigger : MonoBehaviour
{
    bool activated = false;
    public Animator rocks;

    private void OnTriggerEnter(Collider c)
    {
        Viewer player = c.GetComponent<Viewer>();
        if (player != null && !activated)
        {
            activated = true;
            player.ShakeCameraDamage(1, 1.5f, 0.5f);
            rocks.SetBool("RockActivate", true);
        }
    }
}
