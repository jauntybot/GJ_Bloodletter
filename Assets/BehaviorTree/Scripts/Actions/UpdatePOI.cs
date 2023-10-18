using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class UpdatePOI : ActionNode
{
    public enum POIType { Random, RandomCloseToPlayer, BloodPool, Player }
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
            case POIType.RandomCloseToPlayer:
                List<Interactable> inters = new List<Interactable>();
                foreach (Interactable inter in context.enemy.director.interactables) {
                    if (inters.Count < 5) { inters.Add(inter); continue; }
                    
                    for (int i = inters.Count - 1; i >= 0; i--) {
                        if (Vector3.Distance(context.bloodletter.transform.position, inters[i].transform.position) >
                        Vector3.Distance(context.bloodletter.transform.position, inter.transform.position)) {
                            inters.RemoveAt(i);
                            inters.Add(inter);
                        }
                    }
                }
                context.enemy.director.UpdatePOI(inters[Random.Range(0, inters.Count - 1)].transform.position);
            break;
            case POIType.BloodPool:

                context.enemy.director.UpdatePOI(context.enemy.currentPool.transform.position);
            break;
            case POIType.Player:
                context.enemy.director.UpdatePOI(context.bloodletter.transform.position);
            break;
        }
        return State.Success;
    }
}
