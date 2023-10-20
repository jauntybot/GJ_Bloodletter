using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.Jobs;
using UnityEngine;


public class InfectionSite : HoldInteractable {

    public float infectionHeal, infectionDilution;    

    public override void Interact() {
        base.Interact();
        StartCoroutine(RemedyIllness(bloodletter));
    }
    
    public IEnumerator RemedyIllness(Bloodletter bloodletter) {
        interacting = true;
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayHoldInteract(this));
        float timer = 0;
        while (Input.GetMouseButton(0) && interacting && inRange &&
        content > 0  && bloodletter.infectionLevel > 0) {
            while (!bloodletter.tick) {
                yield return null;
                if (!Input.GetMouseButton(0)) {
                    interacting = false;
                    break;
                }
                if (timer >= loopDelay && !audioSource.isPlaying) {
                    audioSource.loop = true;
                    audioSource.clip = loopSFX.Get();
                    audioSource.Play();
                } else timer += Time.deltaTime;
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
// USED ALL BLOOD
        if (content <= 0) {
            ExhaustSite();
        } else if (audioSource.loop == true) {
            audioSource.loop = false;
            audioSource.Stop();
        }
        interacting = false;    
    }



}
