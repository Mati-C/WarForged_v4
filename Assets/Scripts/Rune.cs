using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour
{
    public ParticleSystem particles;
    particleAttractorLinear particleAttractor;
    CheckPoint myCheckPoint;
    public ParticleSystem runeCircle;
    ButtonManager buttonManager;
    Model player;

    void Start()
    {
        player = FindObjectOfType<Model>();
        buttonManager = FindObjectOfType<ButtonManager>();
        myCheckPoint = GetComponent<CheckPoint>();
    }

    void OnTriggerStay(Collider c)
    {
        if (myCheckPoint.checkPointActivated && c.GetComponent<Model>())
        {
            if (player.life != player.maxLife)
            {
                player.UpdateLife(player.maxLife);
                StartCoroutine(PlayParticles());
            }
        }
    }

    IEnumerator PlayParticles()
    {
        particles.Play();
        runeCircle.Play();
        yield return new WaitForSeconds(1);
        particles.Stop();
        runeCircle.Stop();
    }
}
