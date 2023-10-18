using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using KiwiBT;

public class Teleport : ActionNode
{



    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        NavMeshHit navHit;
        Debug.Log("try hit");
        if(NavMesh.SamplePosition(blackboard.moveToPosition, out navHit, 100, 5)) {
            context.transform.position = navHit.position - new Vector3(0, 3, 0);
            Debug.Log(navHit.position);
            return State.Success;
        } else return State.Failure;
    }
}
