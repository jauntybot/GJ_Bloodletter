using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class ChangeState : ActionNode
{

    public EnemyPathfinding.EnemyState _state;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.enemy.state != _state) {
            context.enemy.ChangeState(_state);
            return State.Success;
        }
        return State.Failure;
    }
}
