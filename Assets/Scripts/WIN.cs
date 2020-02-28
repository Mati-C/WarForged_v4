using UnityEngine;

public class WIN : MonoBehaviour
{
    public ModelB_Cyclops boss;
    public Model_B_Ogre boss2;
    public GameObject portal;

    public bool isBoss1;
    public bool isBoss2;

    private void Start()
    {
        boss = FindObjectOfType<ModelB_Cyclops>();
    }

    private void Update()
    {
        if (isBoss2)
        {
            if (boss2.life <= 0) portal.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Viewer player = other.GetComponent<Viewer>();

        if (isBoss1)
        {
            if (player && boss.life <= 0)
                StartCoroutine(player.YouWin());
        }


        if (isBoss2)
        {

            if (player && boss2.life <= 0)
                StartCoroutine(player.YouWin());
        }
    }
}
