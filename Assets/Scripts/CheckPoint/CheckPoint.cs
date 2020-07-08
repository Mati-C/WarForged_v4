using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

public class CheckPoint : MonoBehaviour, ICheckObservable
{
    FireSword _fireSword;
    public ParticleSystem fire;
    public ParticleSystem halo;
    public float lightIntensity;
  
    public GameObject textCheck;
    public GameObject buttonRespawn;
    public Transform ph;
    ButtonManager ButtonManager;
    Model_Player player;
    public bool checkPointActivated = false;

    public ParticleSystem particles;
    CheckPoint myCheckPoint;
    public ParticleSystem runeCircle;
    ButtonManager buttonManager;

    bool move1;
  
    List<ICheckObserver> _allObservers = new List<ICheckObserver>();

    public IEnumerator Message()
    {
        move1 = true;
        //textCheck.SetActive(true);
        fire.emissionRate *= 2;
        fire.startLifetime *= 1.5f;
        halo.startSize *= 2;
        StartCoroutine(Light());
        yield return new WaitForSeconds(3.5f);
        move1 = false;
        //textCheck.SetActive(false);      
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2);
        buttonRespawn.SetActive(true);
    }

    public IEnumerator Light()
    {
        float t = 0;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            fire.transform.GetChild(0).GetComponent<Light>().intensity = Mathf.Lerp(fire.transform.GetChild(0).GetComponent<Light>().intensity, lightIntensity, t / 2);
            yield return new WaitForEndOfFrame();
        }
    }

    void Start ()
    {
        _fireSword = FindObjectOfType<FireSword>();
        player = FindObjectOfType<Model_Player>();
        ButtonManager = FindObjectOfType<ButtonManager>();
        Subscribe(ButtonManager);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetComponent<Model_Player>())
        {
            if (!checkPointActivated)
            {
                ButtonManager.OnNotify(ph);
                StartCoroutine(Message());
                checkPointActivated = true;
                SoundManager.instance.Play(Objects.CHECKPOINT_PASS, new Vector3(), false, 0.4f);
                SoundManager.instance.Play(Objects.CHECKPOINT_IDLE, fire.transform.position, false, 0.3f, true);
            }
            else if (player.life != player.maxLife)
            {
                SoundManager.instance.Play(Objects.CHECKPOINT_PASS, new Vector3(), false, 0.4f);
            }
        }
    }

    void OnTriggerStay(Collider c)
    {
        if (checkPointActivated && c.GetComponent<Model_Player>())
        {
            if (player.life != player.maxLife || player.fireEnergy <= player.fireSword.energyToUseFireSword)
            {
                player.UpdateLife(player.maxLife);
                player.HitEnemyEvent(player.fireSword.energyToUseFireSword);
                StartCoroutine(PlayParticles());
                _fireSword.UpdateSword();
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


    public void Subscribe(ICheckObserver observer)
    {
        if (!_allObservers.Contains(observer))
            _allObservers.Add(observer);
    }

    public void Unsubscribe(ICheckObserver observer)
    {
        if (_allObservers.Contains(observer))
            _allObservers.Remove(observer);
    }
}
