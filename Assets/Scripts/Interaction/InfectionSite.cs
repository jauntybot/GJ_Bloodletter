using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.Jobs;
using UnityEngine;


public class InfectionSite : HoldInteractable {

    public float infectionHeal, infectionDilution;    

    public override void Interact() {
        if (!hasInteracted) {
            FirstInteractionCallback?.Invoke();
            hasInteracted = true;
        }
        if (bloodletter.bloodLevel > 0)
            StartCoroutine(OpenSite());
        else
            DebugUI.instance.textPopUp.DisplayMessage("CANNOT TAKE MORE MEDICINE NOW.");
    }

    protected override IEnumerator OpenSite() {
        yield return base.OpenSite();
        StartCoroutine(RemedyIllness(bloodletter)); 
    }
    
    public IEnumerator RemedyIllness(Bloodletter bloodletter) {
        interacting = true;
        audioSource.loop = true;
        audioSource.clip = loopSFX.Get();
        audioSource.Play();
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayHoldInteract(this));
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
            
            if (bloodletter.infectionPotency - infectionDilution > bloodletter.potencyRange.x)
                bloodletter.infectionPotency -= infectionDilution;
            else bloodletter.infectionPotency = 0;
            if (bloodletter.infectionLevel - infectionHeal > 0)
                bloodletter.infectionLevel -= infectionHeal;
            else bloodletter.infectionLevel = 0;
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
        if (bloodletter.infectionLevel <= 0)
            DebugUI.instance.textPopUp.DisplayMessage("CANNOT TAKE MORE MEDICINE NOW.");
        PlaySound(closeSFX);
        interacting = false;    
    }



}
