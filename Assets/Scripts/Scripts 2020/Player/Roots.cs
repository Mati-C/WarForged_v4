using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour
{
    public MeshRenderer mesh;
    Material _mat;
    BoxCollider _box;

    private void Awake()
    {
        _mat = mesh.materials[0];
        _box = GetComponent<BoxCollider>();
    }

    public void StartDissolve()
    {
        StartCoroutine(DissolveRoot());
    }

    IEnumerator DissolveRoot()
    {
        float t = 3;
        float dissolve = 0;
        while (t >0)
        {
            t -= Time.deltaTime;
            dissolve += Time.deltaTime /3;
            _mat.SetFloat("_DissolveAmount", dissolve);
            yield return new WaitForEndOfFrame();
        }
        _box.isTrigger = true;
    }
}
