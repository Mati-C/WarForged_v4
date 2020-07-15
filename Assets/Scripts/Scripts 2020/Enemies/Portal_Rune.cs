using System.Collections;
using UnityEngine;

public class Portal_Rune : MonoBehaviour
{
    public float hitsToBreak;
    public float expForDestroy;
    float damage;
    public Portal portal;
    MeshRenderer normalMesh;
    Material runeShader;
    public GameObject destructibleMesh;
    public GameObject orb;
    public ParticleSystem particles;
    public bool portalOff;
    FireSword _playerFireSowrd;
    Viewer_Player _playerViewer;
    Camera _cam;
    public GameObject levelUI;
    public PopExpText prefabExpText;

    void Start()
    {
        particles.gameObject.GetComponent<particleAttractorLinear>().target = portal.phParticles;
        damage = 0;
        normalMesh = GetComponent<MeshRenderer>();
        runeShader = normalMesh.material;
        runeShader.SetFloat("_EmissionIntensity", 0);
        _playerFireSowrd = FindObjectOfType<FireSword>();
        _playerViewer = FindObjectOfType<Viewer_Player>();
        _cam = FindObjectOfType<PlayerCamera>().GetComponent<Camera>();
        levelUI = GameObject.Find("LEVEL UI");
    }

    public void Damage()
    {
        damage++;

        StartCoroutine(IncreaseEmission((damage / hitsToBreak) * 3));

        if (damage >= hitsToBreak)
        {
            normalMesh.enabled = false;
            orb.SetActive(false);
            destructibleMesh.SetActive(true);
            particles.Stop();
            portal.TurnOff();
            portal.myRuneDestroy = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(DisableCollision());
            portalOff = true;
            CreateExpPopText(expForDestroy);
            _playerFireSowrd.SwordExp(expForDestroy);
        }
    }

    public void CreateExpPopText(float exp)
    {
        StartCoroutine(ChargeExpFireText(exp));
        PopExpText text = Instantiate(prefabExpText);
        StartCoroutine(FollowEnemyExp(text));
        text.transform.SetParent(levelUI.transform, false);
        text.SetExp(exp);
    }

    public IEnumerator ChargeExpFireText(float exp)
    {
        float t = 0.5f;
        float newExp = _playerFireSowrd.currentExp;
        _playerViewer.timertAlphaSwordExp = 2;
        while (t > 0)
        {
            t -= Time.deltaTime;
            newExp += Time.deltaTime * (exp / 0.5f);
            int n = (int)newExp;
            _playerViewer.swordExp.text = n + "Exp";
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FollowEnemyExp(PopExpText text)
    {
        while (text != null)
        {
            Vector2 screenPos = _cam.WorldToScreenPoint(transform.position - Vector3.up);
            text.transform.position = screenPos;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator IncreaseEmission(float target)
    {
        float t = runeShader.GetFloat("_EmissionIntensity");
        while (t < target)
        {
            t += Time.deltaTime * 2;
            runeShader.SetFloat("_EmissionIntensity", t);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DisableCollision()
    {
        yield return new WaitForSeconds(3);
        Rigidbody[] rb = destructibleMesh.transform.GetComponentsInChildren<Rigidbody>();
        foreach (var i in rb)
            i.isKinematic = true;

        Collider[] col = destructibleMesh.transform.GetComponentsInChildren<Collider>();
        foreach (var i in col)
            i.enabled = false;
    }
}
