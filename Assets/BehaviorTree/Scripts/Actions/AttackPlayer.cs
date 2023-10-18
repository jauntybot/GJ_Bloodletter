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
        if (!context.enemy.attacking) {
            if (Vector3.Distance(context.transform.position, context.enemy.bloodletter.transform.position) <= context.enemy.killRadius) {
                if (context.enemy.attacking && context.enemy.bloodletter.alive) {

                    switch (attackType) {
                        default:
                        case AttackType.Terrorize:
                            context.enemy.StartCoroutine(context.enemy.TerrorizePlayer());
                        break;
                        case AttackType.Kill:
                            context.enemy.StartCoroutine(context.enemy.KillPlayer());
                        break;
                    }
                    
                }
            }
        }
        if (context.enemy.attacking) return State.Running;
        if (!context.enemy.bloodletter.alive) return State.Success;
        return State.Failure;
    }
}
