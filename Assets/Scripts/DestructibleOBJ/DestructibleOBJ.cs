using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

public class DestructibleOBJ : MonoBehaviour
{
    Camera cam;
    public GameObject mainMesh;
    public GameObject destructibleMesh;
    public GameObject prefabHealthOrb;
    public PopExpText prefabExpTextDamage;
    public GameObject levelUI;
    BoxCollider myBox;
    Animator anim;
    public bool destroyed;
    Model_Player _player;
    Viewer_Player _playerView;
    FireSword _sowrd;

    Vector3 startpos;
    public float xp;

    public void Break()
    {
        if (!destroyed)
        {
            destroyed = true;
            destructibleMesh.SetActive(true);
            var orb = Instantiate(prefabHealthOrb, transform.position, transform.rotation, transform);
            anim.SetBool("IsHit", true);
            myBox.isTrigger = true;
            CreateExpPopText(xp);
            _player.fireSword.SwordExp(xp);
            if (gameObject.name.Contains("Barrel") || gameObject.name.Contains("Jugs"))
            {
                mainMesh.SetActive(false);
                if (gameObject.name.Contains("Barrel"))
                    SoundManager.instance.Play(Objects.BARREL_BREAK, transform.position, true);
                else
                    SoundManager.instance.Play(Objects.JUGS_BREAK, transform.position, true);
            }
            else
                SoundManager.instance.Play(Objects.JUGS_BREAK, transform.position, true);
    }
    }

    public void RecoverDes()
    {
        transform.position = startpos;
        destroyed = false;
        if (gameObject.name.Contains("Barrel") || gameObject.name.Contains("Jugs"))
        {
            mainMesh.SetActive(true);
            destructibleMesh.SetActive(false);
        }
        anim.SetBool("IsHit", false);
        myBox.isTrigger = false;       
    }

    public void Start()
    {
        _player = FindObjectOfType<Model_Player>();
        _playerView = FindObjectOfType<Viewer_Player>();
        _sowrd = FindObjectOfType<FireSword>();
        levelUI = GameObject.Find("LEVEL UI");
        startpos = transform.position;
        anim = destructibleMesh.GetComponent<Animator>();
        myBox = GetComponent<BoxCollider>();
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
            cam = FindObjectOfType<PlayerCamera>().GetComponent<Camera>();
    }

    private void OnCollisionEnter(Collision c)
    {
        if(!gameObject.name.Contains("Barrel"))
            if (c.gameObject.GetComponent<Model_Player>())
                if (c.gameObject.GetComponent<Model_Player>().run)
                    Break();
    }

    public void CreateExpPopText(float exp)
    {
        StartCoroutine(ChargeExpFireText(exp));
        Quaternion rot = Quaternion.LookRotation(Vector3.zero, Vector3.up);
        Vector2 screenPos = cam.WorldToScreenPoint(transform.position + Vector3.up);
        PopExpText text = Instantiate(prefabExpTextDamage, screenPos, rot);
        StartCoroutine(FollowEnemyExp(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetExp(exp);
    }

    IEnumerator ChargeExpFireText(float exp)
    {
        float t = 0.5f;
        float newExp = _sowrd.currentExp;
        _playerView.timertAlphaSwordExp = 2;
        while (t > 0)
        {
            t -= Time.deltaTime;
            newExp += Time.deltaTime * (exp / 0.5f);
            int n = (int)newExp;
            _playerView.swordExp.text = n + "Exp";
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FollowEnemyExp(PopExpText text)
    {
        while (text != null)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(transform.position + Vector3.up);
            text.transform.position = screenPos;
            yield return new WaitForEndOfFrame();
        }
    }
}
