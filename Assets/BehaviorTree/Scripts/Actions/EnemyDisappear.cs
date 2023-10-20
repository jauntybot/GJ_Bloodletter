using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class EnemyDisappear : ActionNode {

    public bool disappear;

    protected override void OnStart() {
        if (context.enemy.energyLevel >= 10)
            context.enemy.energyLevel -= 10;
            
        context.enemy.ToggleVisibility(!disappear);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.enemy.hidden == disappear)
            return State.Success;
        return State.Running;
    }
}
