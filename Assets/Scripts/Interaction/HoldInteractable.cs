using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HoldInteractable : Interactable {

    Animator anim;
    public float maxContent;
    public float content;
    public float consumptionRate;
    public bool interacting;

    protected override void Start() {
        base.Start();
        content = maxContent;
        anim = GetComponent<Animator>();
    }

    protected override void Update() {
        if (bloodletter && !locked) {
            inRange = Vector3.Distance(transform.position, bloodletter.transform.position) <= interactRadius;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.GetComponentInParent<Interactable>() == this) inView = true;
                else inView = false;
            } else inView = false;
        }
    }

    protected virtual void ExhaustSite() {
        locked = true;
        inRange = false; inView = false;
        anim.SetBool("Empty", true);
    }

}
