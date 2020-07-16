using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour, ICheckObservable
{
    FireSword _fireSword;
    Camera _cam;
    public Text expText;
    public Text expTextPrefab;
    public Text swordLevelText;
    public Text swordLevelTextPrefab;
    public Text swordInfoText;
    public Text swordInfoTextPrefab;
    public ParticleSystem fire;
    public ParticleSystem halo;
    public float lightIntensity;
  
    public GameObject textCheck;
    public GameObject buttonRespawn;
    GameObject _levelUI;
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
        _cam = FindObjectOfType<PlayerCamera>().GetComponent<Camera>();
        ButtonManager = FindObjectOfType<ButtonManager>();
        expText = Instantiate(expTextPrefab);
        swordLevelText = Instantiate(swordLevelTextPrefab);
        swordInfoText = Instantiate(swordInfoTextPrefab);
        expText.gameObject.SetActive(false);
        swordLevelText.gameObject.SetActive(false);
        swordInfoText.gameObject.SetActive(false);
        _levelUI = GameObject.Find("LEVEL UI");
        expText.transform.SetParent(_levelUI.transform, false);
        swordLevelText.transform.SetParent(_levelUI.transform, false);
        swordInfoText.transform.SetParent(_levelUI.transform, false);
        StartCoroutine(FollowText());
        Subscribe(ButtonManager);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetComponent<Model_Player>())
        {
            player.revivePos = ph.position;
            player.reviveForward = ph.forward;
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
        if (c.GetComponent<Model_Player>())
        {
            _fireSword.UpdateSword();

            if (!expText.IsActive()) expText.gameObject.SetActive(true);

            if (!swordLevelText.IsActive()) swordLevelText.gameObject.SetActive(true);

            if (!swordInfoText.IsActive()) swordInfoText.gameObject.SetActive(true);

            int expInt = (int)_fireSword.expEarned;

            expText.text = expInt + "-Exp / " + _fireSword.expForEachLevel[_fireSword.fireSwordLevel] + "-Exp";

            int l = _fireSword.fireSwordLevel + 1;

            swordLevelText.text = "Level-" + l;

            swordInfoText.text = "Next Level: " + _fireSword.LevelsInfo[_fireSword.fireSwordLevel];
        }

        if (checkPointActivated && c.GetComponent<Model_Player>())
        {
                     
            if (player.life != player.maxLife || player.fireEnergy <= player.fireSword.energyToUseFireSword)
            {
                player.UpdateLife(player.maxLife);
                player.fireEnergy += 0.3f;
                if (player.fireEnergy > player.fireSword.energyToUseFireSword) player.fireEnergy = player.fireSword.energyToUseFireSword;
                player.HitEnemyEvent(player.fireSword.energyToUseFireSword);
                StartCoroutine(PlayParticles());               
            }
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if (expText.IsActive()) expText.gameObject.SetActive(false);
        if (swordLevelText.IsActive()) swordLevelText.gameObject.SetActive(false);
        if (swordInfoText.IsActive()) swordInfoText.gameObject.SetActive(false);
    }

    IEnumerator FollowText()
    {
        while (true)
        {
            Vector2 screenPos = _cam.WorldToScreenPoint(transform.position + Vector3.up * 1.4f);
            Vector2 screenPos2 = _cam.WorldToScreenPoint(transform.position + Vector3.up * 1.9f);
            Vector2 screenPos3 = _cam.WorldToScreenPoint(transform.position + Vector3.up * 1.6f);
            expText.transform.position = screenPos;
            swordLevelText.transform.position = screenPos2;
            swordInfoText.transform.position = screenPos3;
            yield return new WaitForEndOfFrame();
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
