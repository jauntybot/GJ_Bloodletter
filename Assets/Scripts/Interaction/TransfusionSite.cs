using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEngine;

public class TransfusionSite : HoldInteractable {

    
    public float bloodLevelGain;
    [Range(0,1)]
    public float infectionHealP, infectionDilutionP;

    public override void Interact() {
        if (!hasInteracted) {
            FirstInteractionCallback?.Invoke();
            hasInteracted = true;
        }
        if (bloodletter.bloodLevel < 100)
            StartCoroutine(OpenSite());
        else
            DebugUI.instance.textPopUp.DisplayMessage("LET MORE BLOOD FIRST.");
    }

    protected override IEnumerator PilferSite() {
        //while (bloodletter.bloodLevel < 100)
            yield return base.PilferSite();
        if (bloodletter.bloodLevel >= 100)
            DebugUI.instance.textPopUp.DisplayMessage("LET MORE BLOOD FIRST."); 
    }
    protected override void ExhaustSite()
    {
        base.ExhaustSite();
        PlaySound(closeSFX);

        bloodletter.bloodLevel += bloodLevelGain;
        bloodletter.infectionLevel *= infectionHealP;
        bloodletter.infectionPotency *= infectionDilutionP;
    }



}
