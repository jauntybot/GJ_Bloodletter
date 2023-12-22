using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : HoldInteractable
{

    public enum DoorType { Free, Key, Blood, Well };
    public DoorType doorType;
    public int cost;
    [SerializeField] float tollStep = 0;
    bool open;
    public bool goal;

    protected override void Start() {
        Init();
        if (doorType == DoorType.Blood)
            highlight.message = "EXCHANGE BLOOD TO UNLOCK.";
        if (doorType == DoorType.Key)
            highlight.message = "EXCHANGE " + cost + " KEY" + ((cost > 1) ? "S" : "")+ " TO UNLOCK.";
        if (doorType == DoorType.Well)
            highlight.message = "POISON ALL WELLS TO ADVANCE.";
    }

    public override void Interact() {
        base.Interact();
        if (doorType == DoorType.Free) OpenCloseDoor(!open);
        else if (doorType != DoorType.Well) {
            StartCoroutine(OpenSite());
        }
    }

    protected override IEnumerator OpenSite() {
        if ((doorType == DoorType.Blood && bloodletter.bloodLevel >= cost) 
        || (doorType == DoorType.Key && bloodletter.tollCount >= cost))
            yield return base.OpenSite();
    }

    public override void ExhaustSite() {
        base.ExhaustSite();
        switch (doorType) {
            case DoorType.Key:
                bloodletter.tollCount -= cost;
            break;
            case DoorType.Blood:
                bloodletter.bloodLevel -= cost;
            break;  
        }
        bloodletter.interacting = false;
        bloodletter.interactingWith = null;
        OpenCloseDoor(true);
    }

    public void OpenCloseDoor(bool state) {
        anim.SetBool("Open", state);
        PlaySound(state ? openSFX : closeSFX);
        open = state;
    }

}
