using UnityEngine;

public class WIN : MonoBehaviour
{
    public ModelB_Cyclops boss;

    private void Start()
    {
        boss = FindObjectOfType<ModelB_Cyclops>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Viewer player = other.GetComponent<Viewer>();
        if (player && boss.isDead)
            StartCoroutine(player.YouWin());
    }
}
