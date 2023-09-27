using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableHighlight))]
public class Interactable : MonoBehaviour {

    InteractableHighlight highlight;
    public bool interactOnce;
    bool hasInteracted;
    public float interactRadius = 2.5f;
    public bool locked = false;


    void Start() {
        Init();
    }

    public void Init() {
        highlight = GetComponent<InteractableHighlight>();
    }
    
// CHECK IF INTERACTION WAS SUCCESSFUL
    public virtual void OnInteract(Transform player) {
// Check if this can be interacted with
        if (!interactOnce || (interactOnce && !hasInteracted)) {
// Check if player can unlock this
            if (locked) {}
            if (!locked) {
                Interact(player.GetComponent<Bloodletter>());
            }
        }
    }

// OVERIDE FUNCTIONALITY IN INHERITED CLASSES
    public virtual void Interact(Bloodletter bloodetter) {}


}
