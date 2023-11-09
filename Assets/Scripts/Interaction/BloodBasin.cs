using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BloodBasin : Interactable {

    public float capacity;
    public float bloodLevel;
    public bool filled;

    public override void Init() {
        base.Init();
        bloodLevel = 0;
        filled = false;
    }

    protected override void Update() {
        if (bloodletter && !locked && GameManager.instance.gameState == GameManager.GameState.Running) {
            inRange = Vector3.Distance(transform.position, bloodletter.transform.position) <= interactRadius;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.GetComponentInParent<Interactable>() == this) {
                    inView = true;
                    bloodletter.currentBasin = this;
                }
                else {
                    inView = false;
                    if (bloodletter.currentBasin == this) bloodletter.currentBasin = null;
                }
            } else {
                inView = false;
                if (bloodletter.currentBasin == this) bloodletter.currentBasin = null;
            }
        }
    }

    public virtual void BasinFilled() {
        ExhaustSiteCallback?.Invoke();
        bloodletter.interacting = false;
        bloodletter.interactingWith = null;
        bloodLevel = capacity;
        locked = true; filled = true;
        anim.SetBool("Filled", true);
        DebugUI.instance.textPopUp.DisplayMessage("WELL POISONED.", 3);
    }

}
 