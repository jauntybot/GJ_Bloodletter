using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class UpdateLookDir : ActionNode
{

    public bool changeDir;
    public Blackboard.LookDir _lookDir;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
// ONLY RUN UPDATE ONCE AS SUCCESS, CHANGING STATE FAILS LOOP
        if (changeDir && blackboard.lookDir != _lookDir)
            blackboard.lookDir = _lookDir;
        else if (changeDir)
            return State.Failure;

        if (!changeDir) {
            switch (blackboard.lookDir) {
                case Blackboard.LookDir.Forward:
                    context.agent.updateRotation = true;
                break;
                case Blackboard.LookDir.AtPlayer:
                    context.agent.updateRotation = false;
                    Vector3 targetPos = new Vector3(context.enemy.bloodletter.transform.position.x, 
                                       context.gameObject.transform.position.y, 
                                       context.enemy.bloodletter.transform.position.z ) ;
            
                    context.gameObject.transform.LookAt(targetPos);
                    
                break;
                case Blackboard.LookDir.Scanning:
                    context.agent.updateRotation = false;
                break;
            }
        }
        return State.Success;
    }
}
