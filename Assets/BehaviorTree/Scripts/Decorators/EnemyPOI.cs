using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;
using System;

public class EnemyPOI : DecoratorNode
{

    public EnemyDirector.POIType poi;
    public enum Operator { Is, IsNot };
    public Operator op;
    

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        switch (op) {
            default: return State.Failure;
            case Operator.Is:
                if (context.enemy.director.currentPOI == poi) 
                    return child.Update();
                else {
                    child.Abort();
                    return State.Failure;
                }
            case Operator.IsNot:
                if (context.enemy.director.currentPOI == poi) {
                    child.Abort();
                    return State.Failure;
                }
                else 
                    return child.Update();
        }
    }
}
