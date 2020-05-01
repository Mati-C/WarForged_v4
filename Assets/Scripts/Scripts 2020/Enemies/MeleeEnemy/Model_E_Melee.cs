using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Model_E_Melee : ClassEnemy
{
    [Header("EnemyRoom Distances and Angles:")]
    public float viewDistancePersuit;
    public float angleToPersuit;
    public float viewDistanceAttack;
    public float angleToAttack;


    void Start()
    {
        nodes.AddRange(grid.GetNodesList().Where(x => x.walkable));

        var surround = new N_FSM_State("SURROUND");
        var attack = new N_FSM_State("ATTACK");
        var retreat = new N_FSM_State("RETREAT");
        var persuit = new N_FSM_State("PERSUIT");
        var patrol = new N_FSM_State("PATROL");

        patrol.OnUpdate += () =>
        {

            if (canPersuit) myFSM_EventMachine.ChangeState(persuit);
        };

        persuit.OnUpdate += () =>
        {
            MoveToTarget(player.transform);

            if (canSurround) myFSM_EventMachine.ChangeState(surround);
        };

        surround.OnUpdate += () =>
        {
            if(!canSurround && canPersuit) myFSM_EventMachine.ChangeState(persuit);
        };

        myFSM_EventMachine = new N_FSM_EventMachine(patrol);
    }

    
    void Update()
    {
       
        canPersuit = CanSee(player.transform, viewDistancePersuit, angleToPersuit, layersCanSee);

        canSurround = CanSee(player.transform, viewDistanceAttack, angleToAttack, layersCanSee);

        myFSM_EventMachine.Update();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistancePersuit);

        Vector3 rightLimit = Quaternion.AngleAxis(angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistancePersuit));

        Vector3 leftLimit = Quaternion.AngleAxis(-angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistancePersuit));
    
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit2 = Quaternion.AngleAxis(angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceAttack));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceAttack));

        
    }
}
