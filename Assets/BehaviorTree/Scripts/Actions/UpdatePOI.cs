using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class UpdatePOI : ActionNode
{
    public enum POIType { Random, }
    public POIType poiType;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {

        switch (poiType) {
            case POIType.Random:
                context.enemy.director.UpdatePOI(context.enemy.director.interactables[Random.Range(0,context.enemy.director.interactables.Count - 1)].transform.position);
            break;

        }
        return State.Success;
    }
}
