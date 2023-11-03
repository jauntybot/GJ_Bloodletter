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
        turnTimer = 0f;
        startRot = context.transform.rotation;
        if (!changeDir) {
            switch (blackboard.lookDir) {
                case Blackboard.LookDir.Forward:
                    context.agent.updateRotation = true;
                break;
                case Blackboard.LookDir.AtPlayer:
                    context.agent.updateRotation = false;
                    targetRot = Quaternion.LookRotation(context.bloodletter.transform.position - context.transform.position);
                break;
                case Blackboard.LookDir.AtSafezone:
                    context.agent.updateRotation = false;
                    targetRot = Quaternion.LookRotation(context.enemy.safezoneTarget.transform.position - context.transform.position);
                break;
                case Blackboard.LookDir.Scanning:
                    context.agent.updateRotation = false;

                    float rnd = Random.Range(45, 120);
                    if (Random.Range(0, 1) != 0) rnd = -rnd;

                    targetRot = startRot * Quaternion.AngleAxis(rnd, Vector3.up);
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
                    if (turnTimer < turnDur) {
                        turnTimer += Time.deltaTime;
                        context.transform.rotation = Quaternion.Slerp(startRot, targetRot, turnTimer/turnDur);
                        return State.Running;
                    }
                break;
                case Blackboard.LookDir.AtSafezone:
                    if (turnTimer < turnDur) {
                        turnTimer += Time.deltaTime;
                        context.transform.rotation = Quaternion.Slerp(startRot, targetRot, turnTimer/turnDur);
                        return State.Running;
                    }
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
