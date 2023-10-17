using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class MoveToPosition : ActionNode
{
    public enum Pos { bbMoveTo, POI, Player };
    public Pos pos;
    public float speed = 5;
    public float stoppingDistance = 0.1f;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    protected override void OnStart() {
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        context.agent.acceleration = acceleration;

        switch (pos) {

            case Pos.bbMoveTo:
                context.agent.destination = blackboard.moveToPosition;
            break;
            case Pos.POI:
            break;
            case Pos.Player:
                context.agent.destination = context.enemy.bloodletter.transform.position;
            break;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.agent.pathPending) {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance) {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || 
        context.enemy.energyLevel < context.enemy.energyDrainRate * speed) {
            return State.Failure;
        }

        if (context.enemy.energyLevel >= context.enemy.energyDrainRate * speed)
            context.enemy.energyLevel -= context.enemy.energyDrainRate * speed;
        else return State.Failure;
        
        return State.Running;
    }


// WAIT FOR PATH TO FINISH
        // bool finished = false;
        // if (agent.hasPath) {
        //     while (!finished) {
        //         if (!agent.pathPending) {
        //             if (agent.remainingDistance <= agent.stoppingDistance) {
        //                 if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
        //                     finished = true;
        //                 }
        //             }
        //         }
        //         yield return null;
        //     }
        // }

}
