using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class EnemyDisappear : ActionNode {

    public bool disappear;

    protected override void OnStart() {           
        context.enemy.ToggleVisibility(disappear);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.enemy.hidden == disappear)
            return State.Success;
        if (context.enemy.waitingToHide) return State.Failure;
        return State.Running;
    }
}
