using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;
using System;

public class AttackPlayer : ActionNode
{

    public enum AttackType { Terrorize, Kill };
    public AttackType attackType;
    

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (Vector3.Distance(context.transform.position, context.enemy.bloodletter.transform.position) <= context.enemy.killRadius) {
            if (context.enemy.attacking && context.enemy.bloodletter.alive) {
                context.enemy.StartCoroutine(context.enemy.KillPlayer());
                Debug.Log("Attack co");
            }
            if (context.enemy.bloodletter.gameObject.activeSelf) return State.Running;
            return State.Success;
        }
        return State.Failure;
    }
}
