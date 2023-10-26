using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TollSite : HoldInteractable {

    protected override void ExhaustSite() {
        base.ExhaustSite();
        PlaySound(closeSFX);
        bloodletter.tollCount ++;
        highlight.displayMessage = false;
        DebugUI.instance.textPopUp.DisplayMessage("COLLECTED 1 TOLL.", 3);
    }

}
