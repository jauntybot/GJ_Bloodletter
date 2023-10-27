using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TollSite : HoldInteractable {

    public bool safezoneActive;
    [SerializeField] float safezoneRadius;

    protected override void ExhaustSite() {
        base.ExhaustSite();
        PlaySound(closeSFX);
        bloodletter.tollCount ++;
        highlight.displayMessage = false;
        DebugUI.instance.textPopUp.DisplayMessage("COLLECTED 1 TOLL.", 3);
    }

    public IEnumerator ActivateSafeZone() {
        DebugUI.instance.textPopUp.DisplayMessage("TAINTED MONSTER BAIT.", 3);
        yield return new WaitForSeconds(4);
        safezoneActive = true;
    }

    bool active = false;
    protected override void Update() {
        base.Update();    
        if (safezoneActive) {
            inRange = Vector3.Distance(transform.position, bloodletter.transform.position) <= safezoneRadius;
            if (inRange && !active) {
                active = true;
                DebugUI.instance.textPopUp.DisplayMessage("INSIDE SAFE ZONE.");
                EnemyPathfinding.instance.safezone = true;
                EnemyPathfinding.instance.safezoneTarget = this;
            } else if (!inRange && active) {
                active = false;
                DebugUI.instance.textPopUp.DismissMessage();
                EnemyPathfinding.instance.safezone = false;
                EnemyPathfinding.instance.safezoneTarget = null;
            }
        }
    }

}
