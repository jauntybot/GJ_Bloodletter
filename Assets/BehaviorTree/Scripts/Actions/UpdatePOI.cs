using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiwiBT;

public class UpdatePOI : ActionNode
{
    public enum POIType { Random, RandomCloseToPlayer }
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
                    if (inters.Count < 4) { inters.Add(inter); continue; }
                    foreach(Interactable _inter in inters) {
                        if (Vector3.Distance(context.bloodletter.transform.position, _inter.transform.position) >
                        Vector3.Distance(context.bloodletter.transform.position, inter.transform.position)) {
                            inters.Remove(_inter);
                            inters.Add(inter);
                        }
                    }
                }
                context.enemy.director.UpdatePOI(inters[Random.Range(0, inters.Count - 1)].transform.position);

            break;
        }
        return State.Success;
    }
}
