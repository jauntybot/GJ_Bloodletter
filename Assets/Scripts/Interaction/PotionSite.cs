using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.Jobs;
using UnityEngine;


public class PotionSite : HoldInteractable {

    [Range(0,1)]
    public float infectionHealP, infectionDilutionP;    

    public override void Interact() {
        if (!hasInteracted) {
            FirstInteractionCallback?.Invoke();
            hasInteracted = true;
        }
        if (bloodletter.infectionLevel > 0)
            StartCoroutine(OpenSite());
        else
            DebugUI.instance.textPopUp.DisplayMessage("CANNOT TAKE MORE MEDICINE NOW.");
    }

    protected override IEnumerator PilferSite()
    {
        while (bloodletter.infectionLevel > 0)
            yield return base.PilferSite();
        PlaySound(closeSFX);
        if (bloodletter.infectionLevel <= 0) {
            DebugUI.instance.textPopUp.DisplayMessage("CANNOT TAKE MORE MEDICINE NOW.");
        }
    }

    protected override void ExhaustSite() {
        base.ExhaustSite();

        bloodletter.infectionLevel *= infectionHealP;
        bloodletter.infectionPotency *= infectionDilutionP;
    }




}
