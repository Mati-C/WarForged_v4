using UnityEngine;

public class WIN : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Viewer player = other.GetComponent<Viewer>();
        if (player)
            StartCoroutine(player.YouWin());
    }
}
