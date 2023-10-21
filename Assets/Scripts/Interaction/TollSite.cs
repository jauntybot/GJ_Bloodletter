using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TollSite : HoldInteractable {


    protected override IEnumerator OpenSite() {
        yield return base.OpenSite();
        StartCoroutine(RifleSite(bloodletter));
    }

    public IEnumerator RifleSite(Bloodletter bloodletter) {
        audioSource.loop = true;
        audioSource.clip = loopSFX.Get();
        audioSource.Play();
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayHoldInteract(this));
        while (Input.GetMouseButton(0) && interacting && inRange &&
        content > 0) {
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
            
            content -= consumptionRate;

            if (!inRange) {
                interacting = false;
                break;
            }
            yield return null;
        }
        if (audioSource.loop == true) {
            audioSource.loop = false;
            audioSource.Stop();
        }
// USED ALL BLOOD
        if (content <= 0) {
            ExhaustSite();
        } 
        
        interacting = false;    
    }

    protected override void ExhaustSite() {
        base.ExhaustSite();
        PlaySound(closeSFX);
        bloodletter.tollCount ++;
        highlight.displayMessage = false;
        DebugUI.instance.textPopUp.DisplayMessage("COLLECTED 1 TOLL.", 3);
    }

}
