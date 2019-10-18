using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatArea : MonoBehaviour
{
    public List<EnemyEntity> myNPCs = new List<EnemyEntity>();
    public List<GameObject> walls = new List<GameObject>();
    Model player;
    public int myEntities;
    public bool aux;
    public bool startArea;
    public bool firstPass;
    EnemyCombatManager cm;
    public int EnemyID_Area;
    bool endArea;
    public bool puzzleblock;

    public List<GameObject> doors = new List<GameObject>();
    bool isLevel2;

    bool numAdded;

    private void Awake()
    {
        cm = FindObjectOfType<EnemyCombatManager>();
        player = FindObjectOfType<Model>();
        var enemies = FindObjectsOfType<EnemyEntity>().Where(x => x.EnemyID_Area == EnemyID_Area);
        myNPCs.Clear();
        myNPCs.AddRange(enemies);
        numAdded = false;
    }

    void Start()
    {
        isLevel2 = doors.Count != 0;
        myEntities = myNPCs.Count;
        if (startArea == true)
            ToggleBlock(false);
    }

    void Update()
    {
        var auxMyEntites = 0;

        foreach (var item in myNPCs)
            if (item.isDead) auxMyEntites++;

        if (player.isDead && !endArea)
        {
            ToggleBlock(false);
            myEntities = myNPCs.Count;
            firstPass = false;
        }

        if (myEntities <= 0 && !endArea)
        {
            if (auxMyEntites == myNPCs.Count && !numAdded)
            {
                numAdded = true;
                Model player = FindObjectOfType<Model>();
                player.combatIndex++;
                player.isInCombatArea = false;
                if (player.combatIndex == 4 && !isLevel2)
                    player.combatIndex++;
            }

            ToggleBlock(false);
            cm.times = 2;
            foreach (var item in myNPCs)
            {
                item.cantRespawn = true;
            }
            aux = true;
            endArea = true;
        }

        if (auxMyEntites == myNPCs.Count && !aux)
        {
            ToggleBlock(false);
            cm.times = 2;
            aux = true;
        }
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Model>() && !firstPass)
        {
            if (!puzzleblock)
            {
                //c.GetComponent<Model>().enemiesToLock.Clear();


                var enemies = myNPCs.OrderBy(x =>
                {
                    var d = Vector3.Distance(x.transform.position, player.transform.position);
                    return d;

                }).Where(x => x.life > 0);

                //c.GetComponent<Model>().enemiesToLock.AddRange(enemies);
            }

            foreach (var item in myNPCs) item.target = player;
            c.GetComponent<Model>().isInCombatArea = true;

            if (myEntities > 0 && !aux)
                ToggleBlock(true);

            firstPass = true;
        }
    }

    void ToggleBlock (bool state)
    {
        if (isLevel2)
            foreach (var item in doors)
                StartCoroutine(DoorMovement(item, state));
        else
            foreach (var item in walls)
                item.SetActive(state);
    }

    IEnumerator DoorMovement(GameObject door, bool state)
    {
        float t = 0;
        Transform obj;
        Vector3 start;
        Vector3 target;

        if (door.name.Contains("Internal"))
        {
            obj = door.transform.GetChild(0);
            start = obj.localRotation.eulerAngles;
            target = state ? new Vector3(90, 0, 0) : new Vector3(90, 0, -175);

            while (t <= 1)
            {
                t += Time.deltaTime;
                obj.localRotation = Quaternion.Euler(Vector3.Lerp(start, target, t));
                yield return new WaitForEndOfFrame();
            }
        }
        else if (door.name.Contains("Iron") || door.name.Contains("Wall"))
        {
            if (door.name.Contains("Iron"))
            {
                obj = door.transform.GetChild(0);
                target = state ? new Vector3(0, 0, 0.76f) : new Vector3(0, 0, 1.76f);
                start = obj.localPosition;
            }
            else
            {
                obj = door.transform;
                target = state ? new Vector3(-17.359f, -1.28f, 6.4242f) : new Vector3(-17.359f, -3.718f, 6.4242f);
                start = obj.position;
            }

            while (t <= 1)
            {
                t += Time.deltaTime;
                obj.localPosition = Vector3.Lerp(start, target, t);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
