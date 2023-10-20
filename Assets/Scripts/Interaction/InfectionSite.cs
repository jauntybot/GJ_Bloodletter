using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;


public class InfectionSite : HoldInteractable {

    public float infectionHeal, infectionDilution;    

    public override void Interact() {
        StartCoroutine(RemedyIllness(bloodletter));
    }
    
    public IEnumerator RemedyIllness(Bloodletter bloodletter) {
        interacting = true;
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayHoldInteract(this));
        audioSource.loop = true;
        PlaySound(loopSFX);
        while (Input.GetMouseButton(0) && interacting && inRange &&
        content > 0  && bloodletter.infectionLevel > 0) {
            while (!bloodletter.tick) {
                yield return null;
                if (!Input.GetMouseButton(0)) {
                    interacting = false;
                    break;
                }
            }
            if (!Input.GetMouseButton(0)) {
                    interacting = false;
                    break;
            }
            
            if (bloodletter.infectionPotency > bloodletter.potencyRange.x)
                bloodletter.infectionPotency -= infectionDilution;
            if (bloodletter.infectionLevel > 0)
                bloodletter.infectionLevel -= infectionHeal;
            content -= consumptionRate;

            if (!inRange) {
                interacting = false;
                break;
            }
            yield return null;
        }
        audioSource.loop = false;
        audioSource.Stop();
// USED ALL BLOOD
        if (content <= 0) {
            ExhaustSite();
        }
        interacting = false;    
    }



}
