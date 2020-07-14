using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sound;

public class Roots : MonoBehaviour
{
    public MeshRenderer mesh;
    public bool burned;
    public bool tutoRoot;
    Material _mat;
    Material _mat2;
    BoxCollider _box;

    private void Awake()
    {
        _mat = mesh.materials[0];
        _mat2 = mesh.materials[1];
        _box = GetComponent<BoxCollider>();
    }

    public void StartDissolve()
    {
        if (!burned && !tutoRoot)
        {
            burned = true;
            StartCoroutine(DissolveRoot());
        }
    }

    IEnumerator DissolveRoot()
    {
        float t = 3;
        float dissolve = 0;
        SoundManager.instance.Play(Objects.ROOT_BURNED, transform.position, true, 1.2f);
        while (t >0)
        {
            t -= Time.deltaTime;
            dissolve += Time.deltaTime /3;
            if(t<=1.5f) _box.isTrigger = true;
            _mat.SetFloat("_DissolveAmount", dissolve);
            _mat2.SetFloat("_Dissolve", dissolve);
            yield return new WaitForEndOfFrame();
        }
       
    }
}
