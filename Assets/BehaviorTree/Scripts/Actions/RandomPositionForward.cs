using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class RandomPositionForward : ActionNode
{
    public Vector2 range;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        blackboard.moveToPosition = context.gameObject.transform.position + Quaternion.AngleAxis(Random.Range(-context.enemy.detectionCones[2].viewAngle, context.enemy.detectionCones[2].viewAngle)/2, Vector3.up) * context.gameObject.transform.forward * Random.Range(range.x, range.y);
        return State.Success;
    }
}
