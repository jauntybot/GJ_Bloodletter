using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class EnemyState : DecoratorNode
{
    public EnemyPathfinding.EnemyState enemyState;
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
                if (context.enemy.state == enemyState) 
                    return child.Update();
                else {
                    child.Abort();
                    return State.Failure;
                }
            case Operator.IsNot:
                if (context.enemy.state != enemyState)
                    return child.Update();
                else {
                    child.Abort();
                    return State.Failure;
                }
        }
    }
}
