using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : HoldInteractable
{

    public enum DoorType { Free, Toll, Blood };
    public DoorType doorType;

    [SerializeField] float tollStep = 0;
    bool open;


    protected override void Start() {
        Init();
        if (doorType == DoorType.Blood)
            highlight.message = "EXCHANGE BLOOD TO UNLOCK.";
        if (doorType == DoorType.Toll)
            highlight.message = "EXCHANGE " + content + " TOLL TO UNLOCK.";
    }

    public override void Interact() {
        if (!hasInteracted) {
            FirstInteractionCallback?.Invoke();
            hasInteracted = true;
        }
        if (doorType == DoorType.Free) OpenCloseDoor(!open);
        else StartCoroutine(OpenSite());
    }

    protected override IEnumerator OpenSite() {
        yield return base.OpenSite();
        StartCoroutine(SiphonResource());
    }

    public IEnumerator SiphonResource() {
        // audioSource.loop = true;
        // audioSource.clip = loopSFX.Get();
        // audioSource.Play();
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayHoldInteract(this));
        while (Input.GetMouseButton(0) && interacting && inRange &&
        content > 0) {
            if ((doorType == DoorType.Toll && bloodletter.tollCount >= 1) ||
            (doorType == DoorType.Blood && bloodletter.bloodLevel >= consumptionRate)) {
                while (!bloodletter.tick) {
                    yield return null;
                    if (!Input.GetMouseButton(0) || bloodletter.tollCount < 1) {
                        interacting = false;
                        break;
                    }
                }
                if (!Input.GetMouseButton(0) || bloodletter.tollCount < 1) {
                        interacting = false;
                        break;
                }
                
                content -= consumptionRate;
                if (doorType == DoorType.Blood)
                    bloodletter.bloodLevel -= consumptionRate;
                else if (doorType == DoorType.Toll) {
                    tollStep += consumptionRate;
                    if (Mathf.Approximately(tollStep, Mathf.RoundToInt(tollStep)))
                        bloodletter.tollCount --;
                    if (content <= 0.1f) content = 0;
                }

                if (!inRange) {
                    interacting = false;
                    break;
                }
            }
            // if (audioSource.loop == true) {
            //     audioSource.loop = false;
            //     audioSource.Stop();
            // }
    // USED ALL BLOOD
            if (content <= 0) {
                ExhaustSite();
            } 
            yield return null;
        }
        interacting = false;    
    }

    protected override void ExhaustSite() {
        doorType = DoorType.Free;
        DebugUI.instance.textPopUp.DismissMessage();
        highlight.message = "PRESS 'LMB' TO " + (open ? "CLOSE." : "OPEN.");
        DebugUI.instance.textPopUp.DisplayMessage(highlight.message);
    }

    public void OpenCloseDoor(bool state) {
        anim.SetBool("Open", state);
        PlaySound(closeSFX);
        open = state;
        DebugUI.instance.textPopUp.DismissMessage();
        highlight.message = "PRESS 'LMB' TO " + (open ? "CLOSE." : "OPEN.");
    }

}
