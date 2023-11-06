using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;


public class BloodBasin : Interactable {

    public float capacity;
    public float bloodLevel;

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

    protected virtual void BasinFilled() {
        bloodLevel = capacity;


    }

}
