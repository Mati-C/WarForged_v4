using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class AnimationTimeLineScenes : MonoBehaviour
{

    public Animator CutScene01_ROCKS;
    public PlayableDirector CutScene02;
    public PlayableDirector CutScene02_Bars;



    public GameObject RocksAnim;
    public int value;

    public CinematicActivator cinema;

    private void Start()
    {
        // CutScene01_ROCKS = RocksAnim.GetComponent<Animator>();

        // cinema = GetComponent<CinematicActivator>();
        cinema.DirtParticle.Stop();
    
    }

    public IEnumerator CutSceneRocks()
    {

        yield return new WaitForSeconds(0.5f);
        if (CutScene01_ROCKS)
        {
            CutScene01_ROCKS.SetBool("RockActivate", cinema.RockIsActivate);
            cinema.DirtParticle.Play();

            yield return new WaitForSeconds(0.8f);
            cinema.DirtParticle.Stop();
        }
    }
    public IEnumerator CutSceneLever()
    {

       /* yield return new WaitForSeconds(0.5f);
        if (CutScene02)
        {
            CutScene02.Play(); 
        }
        */
        yield return new WaitForSeconds(4);

        if (CutScene02_Bars)
        {
            CutScene02_Bars.Play();
        }
    }


}
