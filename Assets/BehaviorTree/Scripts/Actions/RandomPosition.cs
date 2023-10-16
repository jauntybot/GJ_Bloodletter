using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;
using TMPro;

public class RandomPosition : ActionNode {

    public enum Relative { Const, Forward, FromPlayer, FromPOI, }
    public Relative relative;
    public Vector2 range;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector2 delta = Vector2.zero;
        switch (relative) {
            case Relative.Const:
                delta = Random.insideUnitCircle * Random.Range(range.x, range.y);
                blackboard.moveToPosition.x = context.transform.position.x + delta.x;
                blackboard.moveToPosition.z = context.transform.position.z +  delta.y;
            break;
            case Relative.Forward:
                blackboard.moveToPosition = context.gameObject.transform.position + Quaternion.AngleAxis(Random.Range(-context.enemy.detectionCones[2].viewAngle, context.enemy.detectionCones[2].viewAngle)/2, Vector3.up) * context.gameObject.transform.forward * Random.Range(range.x, range.y);

            break;
            case Relative.FromPlayer:
                delta = Random.insideUnitCircle * Random.Range(range.x, range.y);
                blackboard.moveToPosition.x = context.enemy.bloodletter.transform.position.x + delta.x;
                blackboard.moveToPosition.z = context.enemy.bloodletter.transform.position.z + delta.y;
            break;
            case Relative.FromPOI:
                delta = Random.insideUnitCircle * Random.Range(range.x, range.y);
                blackboard.moveToPosition.x = context.enemy.director.poi.position.x + delta.x;
                blackboard.moveToPosition.z = context.enemy.director.poi.position.z + delta.y;
            break;
        }

        return State.Success;
    }
}
