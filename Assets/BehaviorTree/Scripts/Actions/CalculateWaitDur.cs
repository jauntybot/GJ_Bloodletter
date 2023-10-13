using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class CalculateWaitDur : ActionNode
{


    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.enemy.energyLevel < 50) {

            blackboard.waitDur = Random.Range(2.5f, 10f);

        } else if (context.enemy.energyLevel < 75)
            blackboard.waitDur = Random.Range(0f, 2.5f);
        else
            blackboard.waitDur = Random.Range(0f, 1f);
            
        return State.Success;
    }
}
