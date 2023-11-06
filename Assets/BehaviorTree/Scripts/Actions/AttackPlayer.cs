using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;
using System;

public class AttackPlayer : ActionNode
{

    public enum AttackType { Terrorize, Kill, Safezone };
    public AttackType attackType;
    [SerializeField] float attackRadius;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (!context.enemy.attacking) {
            if (attackType != AttackType.Safezone) {
                if (Vector3.Distance(context.transform.position, context.enemy.bloodletter.transform.position) <= attackRadius) {
                    if (!context.enemy.attacking && context.enemy.bloodletter.alive) {
                        // context.agent.updateRotation = false;
                        // blackboard.lookDir = Blackboard.LookDir.AtPlayer;
                        switch (attackType) {
                            default:
                            case AttackType.Terrorize:
                                context.enemy.StartCoroutine(context.enemy.TerrorizePlayer());
                                return State.Running;
                            
                            case AttackType.Kill:
                                context.enemy.StartCoroutine(context.enemy.KillPlayer());
                                return State.Running;
                            
                        }
                        
                    }
                }
            } else { 
                if (Vector3.Distance(context.transform.position, context.enemy.bloodletter.transform.position) <= attackRadius) {
                    if (!context.enemy.attacking) { 
                        context.enemy.StartCoroutine(context.enemy.TerrorizePlayer());
                        if (context.enemy.director.interactables.Contains(context.enemy.safezoneTarget))
                            context.enemy.director.interactables.Remove(context.enemy.safezoneTarget);
                        Destroy(context.enemy.safezoneTarget.gameObject);
                        context.enemy.safezone = false;
                        context.enemy.safezoneTarget = null;
                        DebugUI.instance.textPopUp.DismissMessage();
                    }
                }
            }
        }
        if (context.enemy.attacking) return State.Running;
        if (!context.enemy.bloodletter.alive) return State.Success;
        return State.Failure;
    }
}
