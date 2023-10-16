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
        context.enemy.ChangeState(_state);
        return State.Success;
    }
}
