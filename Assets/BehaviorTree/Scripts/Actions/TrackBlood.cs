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
                }
            }
        }
        int closePools = 0;
        int idlePools = 0;
        foreach (BloodPool bp in context.enemy.bloodPools) {
            if (Vector3.Distance(bp.transform.position, context.transform.position) < context.enemy.detectionCones[2].dist) {
                if (!bp.inspected) idlePools++;
                closePools++;
            }
        }
        
        if (context.enemy.bloodPools.Count > 0 && idlePools/closePools >= 0.5f) return State.Success;
        return State.Failure;
    }
}
