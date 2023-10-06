using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyBehavior {

    
    static EnemyDirector enemy;    
    public static float nrgReq;

    public static bool CanPerform() {

        return enemy.enemy.energyLevel >= nrgReq;
    }

    public static IEnumerator Perform() {

        yield return null;

    }

}
