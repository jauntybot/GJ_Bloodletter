using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHighlight : MonoBehaviour {

    Interactable interactable;
    [SerializeField] Material highlightMat;
    [SerializeField] bool skinnedMesh;
    List<MeshRenderer> mrs = new List<MeshRenderer>();
    List<SkinnedMeshRenderer> smrs = new List<SkinnedMeshRenderer>();
    bool active;
    [SerializeField] bool displayMessage;
    [SerializeField] string message;

    void Start() {
        if (!skinnedMesh) {
            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>()) 
                mrs.Add(r);
        } else {
            foreach (SkinnedMeshRenderer r in GetComponentsInChildren<SkinnedMeshRenderer>()) 
                smrs.Add(r);
        }
        interactable = GetComponent<Interactable>();
    }

    void Update() {
        if (!skinnedMesh) {
            if (interactable.inRange && interactable.inView && !active) {
                active = true;
                foreach (MeshRenderer r in mrs) {
                    r.SetMaterials(new List<Material>{r.material, highlightMat});
                }
                if (displayMessage) {
                    DebugUI.instance.textPopUp.DisplayMessage(message);
                }
            } else if (active && (!interactable.inRange || !interactable.inView)) {
                active = false;
                foreach (MeshRenderer r in mrs) {
                    r.SetMaterials(new List<Material>{r.material});
                }
                if (displayMessage) {
                    DebugUI.instance.textPopUp.DismissMessage();
                }
            }    
        }
        else {
            if (interactable.inRange && interactable.inView && !active) {
                active = true;
                foreach (SkinnedMeshRenderer r in smrs) {
                    r.SetMaterials(new List<Material>{r.material, highlightMat});
                }
                if (displayMessage) {
                    DebugUI.instance.textPopUp.DisplayMessage(message);
                }
            } else if (active && (!interactable.inRange || !interactable.inView)) {
                active = false;
                foreach (SkinnedMeshRenderer r in smrs) {
                    r.SetMaterials(new List<Material>{r.material});
                }
                if (displayMessage) {
                    DebugUI.instance.textPopUp.DismissMessage();
                }
            }
        }
    } 
}
