using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{

    public bool activated;


    public void ActivateSword()
    {
        activated = true;
    }

    public void DesactivateSword()
    {
        activated = false;
    }

    public void OnTriggerStay(Collider c)
    {
        
    }
}
