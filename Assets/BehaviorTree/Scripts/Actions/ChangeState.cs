using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;
using System.Runtime.ExceptionServices;

public class ChangeState : ActionNode
{

    public EnemyPathfinding.EnemyState _state;
    [SerializeField] bool forceChange;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.enemy.state != _state || forceChange) {
            context.enemy.ChangeState(_state);
            return State.Success;
        }
        return State.Failure;
    }
}
