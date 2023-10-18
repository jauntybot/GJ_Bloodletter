using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class TrackBlood : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {

        Collider[] hitColliders = Physics.OverlapSphere(context.transform.position, context.enemy.detectionCones[2].dist, context.enemy.bloodPoolMask);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.GetComponent<BloodPool>()) {
                BloodPool bp = hitCollider.GetComponent<BloodPool>();
                if (context.enemy.currentPool == null || bp.age > context.enemy.currentPool.age) 
                    context.enemy.currentPool = bp;
                if (!context.enemy.bloodPools.Contains(bp)) {
                    context.enemy.bloodPools.Add(bp);
                    return State.Success;
                }
            }
        }
        return State.Failure;
    }
}
