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
    public bool displayMessage;
    public string message;

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
                    List<Material> mats = new List<Material>();
                    foreach (Material mat in r.materials)
                        mats.Add(mat);
                    
                    mats.Add(highlightMat);
                    r.SetMaterials(mats);
                }
                if (displayMessage) {
                    DebugUI.instance.textPopUp.DisplayMessage(message);
                }
            } else if (active && (!interactable.inRange || !interactable.inView)) {
                active = false;
                foreach (MeshRenderer r in mrs) {
                    List<Material> mats = new List<Material>();
                    foreach (Material mat in r.materials) 
                            mats.Add(mat);
                    
                    mats.RemoveAt(mats.Count - 1);
                    r.SetMaterials(mats);
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
                    List<Material> mats = new List<Material>();
                    foreach (Material mat in r.materials) {
                        if (mat != highlightMat)
                            mats.Add(mat);
                    }
                    r.SetMaterials(new List<Material>{r.material, highlightMat});
                }
                if (displayMessage) {
                    DebugUI.instance.textPopUp.DisplayMessage(message);
                }
            } else if (active && (!interactable.inRange || !interactable.inView)) {
                active = false;
                foreach (SkinnedMeshRenderer r in smrs) {
                    List<Material> mats = new List<Material>();
                    foreach (Material mat in r.materials)     
                        mats.Add(mat);
                    
                    mats.Remove(highlightMat);
                    r.SetMaterials(new List<Material>{r.material});
                }
                if (displayMessage) {
                    DebugUI.instance.textPopUp.DismissMessage();
                }
            }
        }
    } 
}
