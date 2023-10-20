using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : HoldInteractable
{

    public enum DoorType { Free, Toll, Blood };
    public DoorType doorType;

    bool open;


    protected override void Start() {
        Init();
        if (doorType == DoorType.Blood)
            highlight.message = "EXCHANGE BLOOD TO UNLOCK.";
        if (doorType == DoorType.Toll)
            highlight.message = "EXCHANGE " + content + " TOLL TO UNLOCK.";
    }

    public override void Interact() {
        base.Interact();
        switch (doorType) {
            case DoorType.Toll:
            case DoorType.Blood:
                StartCoroutine(SiphonResource());
            break;
            case DoorType.Free:
                OpenCloseDoor(!open);
            break;
        }
    }

    public IEnumerator SiphonResource() {
        interacting = true;
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayHoldInteract(this));
        float tollStep = 0;
        float timer = 0;
        while (Input.GetMouseButton(0) && interacting && inRange &&
        content > 0) {
            if ((doorType == DoorType.Toll && bloodletter.tollCount >= 1) ||
            (doorType == DoorType.Blood && bloodletter.bloodLevel >= consumptionRate)) {
                while (!bloodletter.tick) {
                    yield return null;
                    if (!Input.GetMouseButton(0)) {
                        interacting = false;
                        break;
                    }
                    if (timer >= loopDelay) {
                        audioSource.loop = true;
                        audioSource.clip = loopSFX.Get();
                        audioSource.Play();
                    } else timer += Time.deltaTime;
                }
                if (!Input.GetMouseButton(0)) {
                        interacting = false;
                        break;
                }
                
                content -= consumptionRate;
                if (doorType == DoorType.Blood)
                    bloodletter.bloodLevel -= consumptionRate;
                else if (doorType == DoorType.Toll) {
                    tollStep += consumptionRate;
                    if (Mathf.RoundToInt(tollStep) == tollStep)
                        bloodletter.tollCount -= 1;
                }

                if (!inRange) {
                    interacting = false;
                    break;
                }
            }
            audioSource.loop = false;
            audioSource.Stop();
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
        open = state;
        DebugUI.instance.textPopUp.DismissMessage();
        highlight.message = "PRESS 'LMB' TO " + (open ? "CLOSE." : "OPEN.");
    }

}
