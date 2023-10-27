using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : HoldInteractable
{

    public enum DoorType { Free, Toll, Blood };
    public DoorType doorType;
    public int cost;
    [SerializeField] float tollStep = 0;
    bool open;


    protected override void Start() {
        Init();
        if (doorType == DoorType.Blood)
            highlight.message = "EXCHANGE BLOOD TO UNLOCK.";
        if (doorType == DoorType.Toll)
            highlight.message = "EXCHANGE " + cost + " TOLL TO UNLOCK.";
    }

    public override void Interact() {
        base.Interact();
        if (doorType == DoorType.Free) OpenCloseDoor(!open);
        else {
            Debug.Log("Open site co");
            StartCoroutine(OpenSite());
        }
    }

    protected override IEnumerator OpenSite() {
        Debug.Log("Open Site");
        if ((doorType == DoorType.Blood && bloodletter.bloodLevel >= cost) 
        || (doorType == DoorType.Toll && bloodletter.tollCount >= cost))
            yield return base.OpenSite();
    }

    protected override IEnumerator PilferSite() {
        Debug.Log("Pilfer Site");
        yield return base.PilferSite();


    }

    protected override void ExhaustSite() {
        doorType = DoorType.Free;
        DebugUI.instance.textPopUp.DismissMessage();
        highlight.message = "PRESS 'LMB' TO " + (open ? "CLOSE." : "OPEN.");
        DebugUI.instance.textPopUp.DisplayMessage(highlight.message);
    }

    public void OpenCloseDoor(bool state) {
        anim.SetBool("Open", state);
        PlaySound(state ? openSFX : closeSFX);
        open = state;
        DebugUI.instance.textPopUp.DismissMessage();
        highlight.message = "PRESS 'LMB' TO " + (open ? "CLOSE." : "OPEN.");
    }

}
