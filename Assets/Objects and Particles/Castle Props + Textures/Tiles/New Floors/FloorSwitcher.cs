using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FloorSwitcher : MonoBehaviour
{

    public List<GameObject> GameObjectsMeshesToChange;
    public List<Mesh> meshVariantes;
    public Material newMaterial;
    public bool randomizeMeshes;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (randomizeMeshes)
        {
            Randomize();
            randomizeMeshes = false;
        }   
    }

    void Randomize()
    {
        foreach (var mesh in GameObjectsMeshesToChange)
        {
            Mesh auxMesh = meshVariantes[Random.Range(0, meshVariantes.Count)];

            mesh.GetComponent<MeshFilter>().mesh = auxMesh;
            mesh.GetComponent<Renderer>().material = newMaterial;
        }
    }
}
