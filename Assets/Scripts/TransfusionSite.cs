using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class TransfusionSite : Interactable {

    Animator anim;
    public float bloodTotal;
    [HideInInspector] public float bloodContent;
    public float transfusionRate;
    public float infectionHeal, infectionDilution;
    public bool transfusing;

    protected override void Start() {
        base.Start();
        bloodContent = bloodTotal;
        anim = GetComponent<Animator>();
    }
    protected override void Update() {
        if (bloodletter && !locked)
            inRange = Vector3.Distance(transform.position, bloodletter.transform.position) <= interactRadius;
    }
    public override void Interact() {

        Debug.Log("Interacted");
        StartCoroutine(TransfuseBlood(bloodletter));

    }

    public void ExhaustSite() {
        locked = true;
        anim.SetBool("Empty", true);

    }

    public IEnumerator TransfuseBlood(Bloodletter bloodletter) {
        transfusing = true;
        DebugUI.instance.StartCoroutine(DebugUI.instance.DisplayTransfusion(this));
        while (Input.GetMouseButton(0) && transfusing && inRange &&
        bloodContent > 0  && bloodletter.bloodLevel < 100) {
            while (!bloodletter.tick) {
                yield return null;
                if (!Input.GetMouseButton(0)) {
                    transfusing = false;
                    break;
                }
            }
            if (!Input.GetMouseButton(0)) {
                    transfusing = false;
                    break;
            }
            Debug.Log("Transfusing");
            if (bloodletter.bloodLevel < 100)
                bloodletter.bloodLevel += transfusionRate;
            if (bloodletter.infectionPotency > 0)
                bloodletter.infectionPotency -= infectionDilution;
            if (bloodletter.infectionLevel > 0)
                bloodletter.infectionLevel -= infectionHeal;
            bloodContent -= transfusionRate;
            if (!inRange) {
                transfusing = false;
                break;
            }
            yield return null;
        }
// USED ALL BLOOD
        if (bloodContent <= 0) {
            ExhaustSite();
        }
        transfusing = false;    
    }



}
