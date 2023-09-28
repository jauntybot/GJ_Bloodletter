using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHighlight : MonoBehaviour {

    Interactable interactable;
    [SerializeField] Material highlightMat;
    List<MeshRenderer> mrs = new List<MeshRenderer>();
    bool active;

    void Start() {
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>()) {
            mrs.Add(r);
        }
        interactable = GetComponent<Interactable>();
    }

    void Update() {
        if (interactable.inRange && !active) {
            active = true;
            foreach (MeshRenderer r in mrs) {
                r.SetMaterials(new List<Material>{r.material, highlightMat});
            }
        } else if (active && !interactable.inRange) {
            active = false;
            foreach (MeshRenderer r in mrs) {
                r.SetMaterials(new List<Material>{r.material});
            }
        }    
    } 
}
