using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using KiwiBT;

public class Teleport : ActionNode
{

    public enum TeleportType { DistAwayFromPlayer };
    public TeleportType teleportType;
    public bool outsideOfVision;
    public float value;
    public bool byPlaytime;


    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        NavMeshHit navHit;
        Debug.Log("try hit");
        if(NavMesh.SamplePosition(blackboard.moveToPosition, out navHit, 100, 5)) {
            Debug.Log(navHit.position);
            switch(teleportType) {
                case TeleportType.DistAwayFromPlayer:
                    Debug.Log(context.enemy.director.interactedCount/(context.enemy.director.interactables.Count - 1) * value);
                    if (Vector3.Distance(navHit.position, context.bloodletter.transform.position) <= (byPlaytime ? context.enemy.director.interactedCount/(context.enemy.director.interactables.Count - 1) * value : value))
                        return State.Failure;
                break;
            }
            if (outsideOfVision) {
                if (Vector3.Distance(navHit.position, context.bloodletter.transform.position) <= context.bloodletter.fovCone.dist) {
                    Vector3 dir = (context.bloodletter.transform.position - navHit.position).normalized;
                    float angleDelta = Vector3.Angle(context.bloodletter.transform.forward, dir);
                    if (angleDelta < context.bloodletter.fovCone.viewAngle / 2f) {
                        if (!Physics.Linecast(context.bloodletter.transform.position, navHit.position, context.bloodletter.fovCone.viewMask)) {
                            return State.Failure;
                        }
                    }
                }
            }
            context.transform.position = navHit.position;
            return State.Success;
        } 
        return State.Failure;
    }
}
