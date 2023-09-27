using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransfusionSite : Interactable {


    public float bloodContent;
    public bool posLock;
    public bool attached, transfusing;

    public override void Interact(Bloodletter bloodletter) {

        Debug.Log("Interacted");
        bloodletter.StartCoroutine(bloodletter.TransfuseBlood(this));

    }

    public void ExhaustSite() {
        locked = true;


    }




}
