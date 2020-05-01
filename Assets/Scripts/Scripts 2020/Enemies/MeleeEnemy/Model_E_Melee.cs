using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Model_E_Melee : ClassEnemy
{
    
    void Start()
    {
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));
    }

    
    void Update()
    {
        FindPath(player.transform.position);
        myPathCount = pathfinding.myPath.Count;
    }
}
