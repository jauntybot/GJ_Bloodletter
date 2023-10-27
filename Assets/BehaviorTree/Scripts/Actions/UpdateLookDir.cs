using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class UpdateLookDir : ActionNode
{

    public bool changeDir;
    public Blackboard.LookDir _lookDir;

    public float turnDur, turnTimer;

    Vector3 targetPos;
    Quaternion startRot, targetRot;

    protected override void OnStart() {
        if (!changeDir) {
            switch (blackboard.lookDir) {
                case Blackboard.LookDir.Forward:
                    context.agent.updateRotation = true;
                break;
                case Blackboard.LookDir.AtPlayer:
                    context.agent.updateRotation = false;
                break;
                case Blackboard.LookDir.Scanning:
                    context.agent.updateRotation = false;

                    turnTimer = 0f;

                    float rnd = Random.Range(80, 200);
                    if (Random.Range(0, 1) != 0) rnd = -rnd;

                    startRot = context.transform.rotation;
                    targetRot = Quaternion.AngleAxis(rnd, Vector3.up);
                break;
            }
        }
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
                break;
                case Blackboard.LookDir.AtPlayer:
                    targetPos = new Vector3(context.enemy.bloodletter.transform.position.x, 
                                       context.gameObject.transform.position.y, 
                                       context.enemy.bloodletter.transform.position.z ) ;        
                    context.gameObject.transform.LookAt(targetPos);
                break;
                case Blackboard.LookDir.AtSafezone:
                    if (context.enemy.safezoneTarget == null) return State.Failure;
                    targetPos = new Vector3(context.enemy.safezoneTarget.transform.position.x, 
                                       context.gameObject.transform.position.y, 
                                       context.enemy.safezoneTarget.transform.position.z ) ;        
                    context.gameObject.transform.LookAt(targetPos);
                break;
                case Blackboard.LookDir.Scanning:
                    if (turnTimer < turnDur) {
                        turnTimer += Time.deltaTime;
                        context.transform.rotation = Quaternion.Lerp(startRot, targetRot, turnTimer/turnDur);
                        return State.Running;
                    }
                break;
            }
        }
        return State.Success;
    }
}
